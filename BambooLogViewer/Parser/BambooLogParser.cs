using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BambooLogViewer.DataModel;
using System.Net;
using System.Text.RegularExpressions;

namespace BambooLogViewer.Parser
{
  public class BambooLogParser
  {
    public static IEnumerable<string> downloadFile(string url)
    {
      using (WebClient client = new WebClient())
      {
        return client.DownloadString(url).Split('\n');
      }
    }

    public static BambooLog Parse(IEnumerable<string> lines)
    {
      var parser = new BambooLogParser();
      parser.run(lines);
      return parser.log;
    }

    private BambooLog log = new BambooLog();
    private Build currentBuild = null;
    private PlanTask currentTask = null;
    private static Regex regexBuildStarted = new Regex(@"^Build (?<Name>.+) - Build #(?<Number>[0-9]+) \((?<Id>.+)\) started building on agent (?<Agent>.+)$");
    private static Regex regexBuildFinished = new Regex(@"^Finished building (?<Id>.+).$");
    private static Regex regexTaskStarted = new Regex(@"^Starting task '(?<Name>.+)' of type '(?<Type>.+)'$");
    private static Regex regexTaskFinished = new Regex(@"^Finished task '(?<Name>.+)' with result: (?<Result>.+)$");
    private static Regex regexVSProjectStarted = new Regex(@"^(?<Number>\d+)\>-+ (?<Target>Build|Rebuild All) started: Project: (?<Name>[a-zA-Z0-9_.]+), Configuration: (?<Configuration>.+) -+$");
    private static Regex regexProjectNumber = new Regex(@"^(?<Number>\d+)\>(?<Message>.*)$");
    private static Regex regexError = new Regex(@"^.+: (?<Severity>warning|error|fatal error) (\w+):");

    private void run(IEnumerable<string> lines)
    {
      var tabDelimiter = new char[] { '\t' };

      foreach (string line in lines)
      {
        if (line.Trim() == "")
          continue;
        var columns = line.Split(tabDelimiter, 3);
        var row = new Row { Kind = columns[0], Time = DateTime.Parse(columns[1]), Message = columns[2] };

        if (matchBuildStarted(row, regexBuildStarted) ||
            matchBuildFinished(row, regexBuildFinished) ||
            matchTaskStarted(row, regexTaskStarted) ||
            matchTaskFinished(row, regexTaskFinished) ||
            matchVSProjectStarted(row, regexVSProjectStarted) ||
            matchVSProjectRecord(row, regexProjectNumber))
          continue;

        if(currentTask != null)
        {
          var record = new Record();
          record.Kind = row.Kind;
          record.Time = row.Time;
          record.Message = row.Message;
          currentTask.Records.Add(record);
        }
      }
    }

    private bool matchBuildStarted(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        currentBuild = new Build();
        currentBuild.Time = row.Time;
        setMatchedProperties(currentBuild, match.Groups, regex.GetGroupNames());
        log.Builds.Add(currentBuild);
      }
      return match.Success;
    }

    private bool matchBuildFinished(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        if (currentBuild == null)
          throw new Exception("Current build is null");
        if (currentBuild.Id != match.Groups["Id"].Value)
          throw new Exception("Build id mismatch");
        currentBuild.FinishTime = row.Time;
        currentBuild = null;
      }
      return match.Success;
    }

    private bool matchTaskStarted(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        currentTask = new PlanTask();
        currentTask.Time = row.Time;
        setMatchedProperties(currentTask, match.Groups, regex.GetGroupNames());
        currentBuild.Tasks.Add(currentTask);
      }
      return match.Success;
    }

    private bool matchTaskFinished(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        if (currentTask == null)
          throw new Exception("Current task is null");
        if (currentTask.Name != match.Groups["Name"].Value)
          throw new Exception("Task name mismatch");
        currentTask.FinishTime = row.Time;
        currentTask.Result = match.Groups["Result"].Value;
        currentTask = null;
      }
      return match.Success;
    }

    private bool matchVSProjectStarted(Row row, Regex regex)
    {
      var match = regex.Match(row.Message);
      if (match.Success)
      {
        if (currentTask == null)
          throw new Exception("Current task is null");
        var project = new VSProject();
        project.Time = row.Time;
        setMatchedProperties(project, match.Groups, regex.GetGroupNames());
        currentTask.VSProjects.Add(project.Number, project);
        currentTask.Records.Add(project);
      }
      return match.Success;
    }

    private bool matchVSProjectRecord(Row row, Regex regex)
    {
      if (currentTask == null)
        return false;

      var match = regex.Match(row.Message);
      if (match.Success)
      {
        var record = new Record();
        record.Kind = row.Kind;
        record.Time = row.Time;
        record.Message = match.Groups["Message"].Value;
        var projectNumber = match.Groups["Number"].Value;
        var project = currentTask.VSProjects[projectNumber];
        var errorMatch = regexError.Match(record.Message);
        if (errorMatch.Success)
        {
          switch(errorMatch.Groups["Severity"].Value)
          {
            case "warning":
              record.Severity = MessageSeverity.Warning;
              project.Warnings++;
              break;
            case "error":
            case "fatal error":
              record.Severity = MessageSeverity.Error;
              project.Errors++;
              break;
          }
        }
        project.Records.Add(record);
        project.FinishTime = row.Time;
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
