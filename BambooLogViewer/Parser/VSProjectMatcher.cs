using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BambooLogViewer.Model;

namespace BambooLogViewer.Parser
{
  public class VSProjectMatcher : Matcher
  {
    private static Regex regexVSProjectStarted = new Regex(@"^(?<Number>\d+)\>-+ (?<Target>Build|Rebuild All) started: Project: (?<Name>[a-zA-Z0-9_.]+), Configuration: (?<Configuration>.+) -+$");
    private static Regex regexVSBuildFinished = new Regex(@"^========== (?<Target>Build|Rebuild All): (?<Counters>.*) ==========$");
    private static Regex regexCounter = new Regex(@"^(?<Value>\d+) (?<Name>.*)$");
    private static Regex regexBuildMessage = new Regex(@"^(?<Number>\d+)\>(?<Message>.*)$");
    private static Regex regexError = new Regex(@"^.+: (?<Severity>warning|error|fatal error) (\w+):");

    VSBuild getCurrentBuild(BambooLogParser parser)
    {
      if (parser.Stack.Count == 0)
        return null;
      return parser.Stack.Peek() as VSBuild;
    }
    
    bool matchProjectStarted(BambooLogParser parser, Row row)
    {
      var match = regexVSProjectStarted.Match(row.Message);
      if (match.Success)
      {
        var build = getCurrentBuild(parser);
        if (build == null)
        {
          build = new VSBuild();
          parser.Stack.Peek().Add(build);
          parser.Stack.Push(build);
        }

        var project = new VSProject();
        project.Time = row.Time;
        setMatchedProperties(project, match.Groups, regexVSProjectStarted.GetGroupNames());
        build.Projects.Add(project.Number, project);
        build.Add(project);
      }
      return match.Success;
    }

    bool matchProjectRecord(BambooLogParser parser, Row row)
    {
      var build = getCurrentBuild(parser);
      if (build == null)
        return false;

      var match = regexBuildMessage.Match(row.Message);
      if (match.Success)
      {
        var record = new SimpleRecord();
        record.Kind = row.Kind;
        record.Time = row.Time;
        record.Message = match.Groups["Message"].Value;
        var projectNumber = match.Groups["Number"].Value;
        var project = build.Projects[projectNumber];
        var errorMatch = regexError.Match(record.Message);
        if (errorMatch.Success)
        {
          switch (errorMatch.Groups["Severity"].Value)
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

    bool matchBuildFinished(BambooLogParser parser, Row row)
    {
      var build = getCurrentBuild(parser);
      if (build == null)
        return false;
      var match = regexVSBuildFinished.Match(row.Message);
      if (match.Success)
      {
        var counters = match.Groups["Counters"].Value.Split(new string[] { ", " }, StringSplitOptions.None);
        foreach(var counter in counters)
        {
          var counterMatch = regexCounter.Match(counter);
          if(counterMatch.Success)
          {
            string value = counterMatch.Groups["Value"].Value;
            switch(counterMatch.Groups["Name"].Value)
            {
              case "succeeded":
                build.SucceededCount = value;
                break;
              case "failed":
                build.FailedCount = value;
                break;
              case "skipped":
                build.SkippedCount = value;
                break;
              case "up-to-date":
                build.UpToDateCount = value;
                break;
            }
          }
        }
        parser.Stack.Pop();
      }
      return match.Success;
    }

    public override bool Match(BambooLogParser parser, Row row)
    {
      return 
        matchProjectStarted(parser, row) || 
        matchProjectRecord(parser, row) ||
        matchBuildFinished(parser, row);
    }
  }
}
