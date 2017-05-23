﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BambooLogViewer.Model;
using System.Net;
using System.Text.RegularExpressions;

namespace BambooLogViewer.Parser
{
  public class BambooLogParser
  {
    public static string downloadFile(string url)
    {
      using (WebClient client = new WebClient())
      {
        return client.DownloadString(url);
      }
    }

    public static BambooLog Parse(string text)
    {
      return Parse(text.Split(new string[] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries));
    }

    public static BambooLog Parse(IEnumerable<string> lines)
    {
      var parser = new BambooLogParser();
      parser.run(lines);
      return parser.log;
    }

    private BambooLog log = new BambooLog();
    private Stack<GroupRecord> groupStack = new Stack<GroupRecord>();
    private GTestCase lastTestGroup = null;

    private static Regex regexBuildStarted = new Regex(@"^Build (?<Name>.+) #(?<Number>[0-9]+) \((?<Id>.+)\) started building on agent (?<Agent>.+)$");
    private static Regex regexBuildFinished = new Regex(@"^Finished building (?<Id>.+)\.$");
    private static Regex regexTaskStarted = new Regex(@"^Starting task '(?<Name>.+)' of type '(?<Type>.+)'$");
    private static Regex regexTaskFinished = new Regex(@"^Finished task '(?<Name>.+)' with result: (?<Result>.+)$");
    private static Regex regexVSProjectStarted = new Regex(@"^(?<Number>\d+)\>-+ (?<Target>Build|Rebuild All) started: Project: (?<Name>[a-zA-Z0-9_.]+), Configuration: (?<Configuration>.+) -+$");
    private static Regex regexProjectNumber = new Regex(@"^(?<Number>\d+)\>(?<Message>.*)$");
    private static Regex regexError = new Regex(@"^.+: (?<Severity>warning|error|fatal error) (\w+):");
    private static Regex regexGTestRunStarted = new Regex(@"^\[==========\] Running (?<TestCount>\d+) test[s|] from (?<CaseCount>\d+) test cases?.$");
    private static Regex regexGTestRunFinished = new Regex(@"^\[==========\] (?<TestCount>\d+) tests? from (?<CaseCount>\d+) test cases? ran. \((?<Milliseconds>\d+) ms total\)$");
    private static Regex regexGTestCaseStarted = new Regex(@"^\[----------\] (?<TestCount>\d+) tests? from (?<Name>_\w+|[\w-[0-9_]]\w*)$");
    private static Regex regexGTestCaseFinished = new Regex(@"^\[----------\] (?<TestCount>\d+) tests? from (?<Name>_\w+|[\w-[0-9_]]\w*) \((?<Milliseconds>\d+) ms total\)$");
    private static Regex regexGTestParamStarted = new Regex(@"^\[----------\] (?<TestCount>\d+) tests? from (?<Name>_\w+|[\w-[0-9_]]\w*)/(?<Index>\d+), where (?<Parameter>.+)$");
    private static Regex regexGTestParamFinished = new Regex(@"^\[----------\] (?<TestCount>\d+) tests? from (?<Name>_\w+|[\w-[0-9_]]\w*)/(?<Index>\d+) \((?<Milliseconds>\d+) ms total\)$");
    private static Regex regexGTestStarted = new Regex(@"^\[ RUN      \] (?<CaseName>_\w+|[\w-[0-9_]]\w*)(/\d+)?.(?<Name>_\w+|[\w-[0-9_]]\w*)$");
    private static Regex regexGTestFinished = new Regex(@"\[(?<Result>       OK |  FAILED  )\] (?<CaseName>_\w+|[\w-[0-9_]]\w*)(/\d+)?.(?<Name>_\w+|[\w-[0-9_]]\w*) \((?<Milliseconds>\d+) ms\)$");

    private void run(IEnumerable<string> lines)
    {
      var tabDelimiter = new char[] { '\t' };

      foreach (string line in lines)
      {
        if (line.Trim() == "")
          continue;
        var columns = line.Split(tabDelimiter, 3);
        if(columns[2].Trim() == "")
          continue;
        var row = new Row { Kind = columns[0], Time = DateTime.Parse(columns[1]), Message = columns[2] };

        if (matchBuildStarted(row, regexBuildStarted) ||
            matchBuildFinished(row, regexBuildFinished) ||
            matchTaskStarted(row, regexTaskStarted) ||
            matchTaskFinished(row, regexTaskFinished) ||
            matchVSProjectStarted(row, regexVSProjectStarted) ||
            matchVSProjectRecord(row, regexProjectNumber) ||
            matchGTestRunStarted(row, regexGTestRunStarted) ||
            matchGTestRunFinished(row, regexGTestRunFinished) ||
            matchGTestCaseStarted(row, regexGTestCaseStarted) ||
            matchGTestCaseFinished(row, regexGTestCaseFinished) ||
            matchGTestParamStarted(row, regexGTestParamStarted) ||
            matchGTestParamFinished(row, regexGTestParamFinished) ||
            matchGTestStarted(row, regexGTestStarted) ||
            matchGTestFinished(row, regexGTestFinished))
          continue;

        if(groupStack.Count != 0)
        {
          var record = new Record();
          record.Kind = row.Kind;
          record.Time = row.Time;
          record.Message = row.Message;
          groupStack.Peek().Add(record);
        }
      }
      log.Update();
    }

    private bool matchBuildStarted(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        if (groupStack.Count > 0)
          throw new Exception("groupStack.Count > 0");
        var build = new Build();
        build.Time = row.Time;
        setMatchedProperties(build, match.Groups, regex.GetGroupNames());
        log.Add(build);
        groupStack.Push(build);
      }
      return match.Success;
    }

    private bool matchBuildFinished(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        while(groupStack.Count > 1)
        {
          groupStack.Pop().FinishTime = row.Time;
        }
        var build = groupStack.Peek() as Build;
        if (build.Id != match.Groups["Id"].Value)
          throw new Exception("Build id mismatch");
        build.FinishTime = row.Time;
        groupStack.Pop();
      }
      return match.Success;
    }

    private bool matchTaskStarted(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        var task = new PlanTask();
        task.Time = row.Time;
        setMatchedProperties(task, match.Groups, regex.GetGroupNames());
        groupStack.Peek().Add(task);
        groupStack.Push(task);
      }
      return match.Success;
    }

    private bool matchTaskFinished(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        var task = groupStack.Peek() as PlanTask;
        if (task.Name != match.Groups["Name"].Value)
          throw new Exception("Task name mismatch");
        task.FinishTime = row.Time;
        task.Result = match.Groups["Result"].Value;
        groupStack.Pop();
      }
      return match.Success;
    }

    private bool matchVSProjectStarted(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        var task = groupStack.Peek() as PlanTask;
        var project = new VSProject();
        project.Time = row.Time;
        setMatchedProperties(project, match.Groups, regex.GetGroupNames());
        task.VSProjects.Add(project.Number, project);
        task.Add(project);
      }
      return match.Success;
    }

    private bool matchVSProjectRecord(Row row, Regex regex)
    {
      if(groupStack.Count == 0)
        return false;
      var task = groupStack.Peek() as PlanTask;
      if (task == null)
        return false;

      var match = regex.Match(row.Message);
      if (match.Success)
      {
        var record = new Record();
        record.Kind = row.Kind;
        record.Time = row.Time;
        record.Message = match.Groups["Message"].Value;
        var projectNumber = match.Groups["Number"].Value;
        var project = task.VSProjects[projectNumber];
        var errorMatch = regexError.Match(record.Message);
        if (errorMatch.Success)
        {
          switch(errorMatch.Groups["Severity"].Value)
          {
            case "warning":
              record.Severity = MessageSeverity.Warning;
              break;
            case "error":
            case "fatal error":
              record.Severity = MessageSeverity.Error;
              break;
          }
        }
        project.Add(record);
        project.FinishTime = row.Time;
      }
      return match.Success;
    }

    private bool matchGTestRunStarted(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        var task = groupStack.Peek() as PlanTask;
        var test = new GTestRun();
        test.Time = row.Time;
        setMatchedProperties(test, match.Groups, regex.GetGroupNames());
        task.Add(test);
        groupStack.Push(test);
      }
      return match.Success;
    }

    private bool matchGTestRunFinished(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        var test = groupStack.Peek() as GTestRun;
        test.FinishTime = row.Time;
        groupStack.Pop();
      }
      return match.Success;
    }
    
    private bool matchGTestCaseStarted(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        var test = new GTestCase();
        test.Time = row.Time;
        setMatchedProperties(test, match.Groups, regex.GetGroupNames());
        groupStack.Peek().Add(test);
        groupStack.Push(test);
      }
      return match.Success;
    }

    private bool matchGTestCaseFinished(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        var test = groupStack.Peek() as GTestCase;
        test.FinishTime = row.Time;
        groupStack.Pop();
      }
      return match.Success;
    }

    private bool matchGTestParamStarted(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        var test = new GTestCaseParametrized();
        test.Time = row.Time;
        setMatchedProperties(test, match.Groups, regex.GetGroupNames());
        if(lastTestGroup == null || lastTestGroup.Name != test.Name)
        {
          lastTestGroup = new GTestCase();
          lastTestGroup.Time = row.Time;
          setMatchedProperties(lastTestGroup, match.Groups, regex.GetGroupNames());
          groupStack.Peek().Add(lastTestGroup);
        }
         
        lastTestGroup.Add(test);
        groupStack.Push(lastTestGroup);
        groupStack.Push(test);
      }
      return match.Success;
    }

    private bool matchGTestParamFinished(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        var test = groupStack.Peek() as GTestCaseParametrized;
        test.FinishTime = row.Time;
        groupStack.Pop();
        groupStack.Pop();
      }
      return match.Success;
    }

    private bool matchGTestStarted(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        var test = new GTest();
        test.Time = row.Time;
        setMatchedProperties(test, match.Groups, regex.GetGroupNames());
        groupStack.Peek().Add(test);
        groupStack.Push(test);
      }
      return match.Success;
    }

    private bool matchGTestFinished(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        var test = groupStack.Peek() as GTest;
        test.FinishTime = row.Time;
        test.Result = match.Groups["Result"].Value;
        groupStack.Pop();
      }
      return match.Success;
    }


    static void setMatchedProperties(object obj, GroupCollection groups, string[] groupNames)
    {
      var type = obj.GetType();
      foreach(var groupName in groupNames)
      {
        var group = groups[groupName];
        if(group.Success)
        {
          var propertyInfo = type.GetProperty(groupName);
          if (propertyInfo != null)
            propertyInfo.SetValue(obj, group.Value);
        }
      }
    }

    private struct Row
    {
      public string Kind;
      public DateTime Time;
      public string Message;
    }
  }
}
