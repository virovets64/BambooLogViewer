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

    private void run(IEnumerable<string> lines)
    {
      var tabDelimiter = new char[] { '\t' };

      var regexBuildStarted = new Regex(@"Build (?<Name>.+) - Build #(?<Number>[0-9]+) \((?<Id>.+)\) started building on agent (?<Agent>.+)");
      var regexBuildFinished = new Regex(@"Finished building (?<Id>.+).");
      var regexTaskStarted = new Regex(@"Starting task '(?<Name>.+)' of type '(?<Type>.+)'");
      var regexTaskFinished = new Regex(@"Finished task '(?<Name>.+)' with result: (?<Result>.+)");

      foreach (string line in lines)
      {
        var columns = line.Split(tabDelimiter, 3);
        var row = new Row { Kind = columns[0], Time = DateTime.Parse(columns[1]), Message = columns[2] };

        var match = regexBuildStarted.Match(row.Message);
        if (match.Success)
        {
          currentBuild = new Build();
          setMatchedProperties(currentBuild, match.Groups, regexBuildStarted.GetGroupNames());
          log.Builds.Add(currentBuild);
          continue;
        }

        match = regexBuildFinished.Match(row.Message);
        if (match.Success)
        {
          if (currentBuild == null)
            throw new Exception("Current build is null");
          if (currentBuild.Id != match.Groups["Id"].Value)
            throw new Exception("Build id mismatch");
          currentBuild = null;
          continue;
        }

        match = regexTaskStarted.Match(row.Message);
        if (match.Success)
        {
          currentTask = new PlanTask();
          setMatchedProperties(currentTask, match.Groups, regexTaskStarted.GetGroupNames());
          currentBuild.Tasks.Add(currentTask);
          continue;
        }

        match = regexTaskFinished.Match(row.Message);
        if (match.Success)
        {
          if (currentTask == null)
            throw new Exception("Current task is null");
          if (currentTask.Name != match.Groups["Name"].Value)
            throw new Exception("Task name mismatch");
          currentTask.Result = match.Groups["Result"].Value;
          currentTask = null;
          continue;
        }
      }

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
