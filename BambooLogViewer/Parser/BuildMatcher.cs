using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BambooLogViewer.Model;


namespace BambooLogViewer.Parser
{
  public class BuildMatcher: Matcher
  {
    private static Regex regexBuildStarted = new Regex(@"^Build (?<Name>.+) - Build #(?<Number>[0-9]+) \((?<Id>.+)\) started building on agent (?<Agent>.+)$");
    private static Regex regexBuildFinished = new Regex(@"^Finished building (?<Id>.+)\.$");

    bool matchBuildStarted(BambooLogParser parser, Row row)
    {
      var match = regexBuildStarted.Match(row.Message);
      if (match.Success)
      {
        if (parser.Stack.Count > 0)
          throw new Exception("Stack.Count > 0");
        var build = new Build();
        build.Time = row.Time;
        setMatchedProperties(build, match.Groups, regexBuildStarted.GetGroupNames());
        parser.Log.Add(build);
        parser.Stack.Push(build);
      }
      return match.Success;
    }

    bool matchBuildFinished(BambooLogParser parser, Row row)
    {
      var match = regexBuildFinished.Match(row.Message);
      if (match.Success)
      {
        while (parser.Stack.Count > 1)
        {
          parser.Stack.Pop().FinishTime = row.Time;
        }
        var build = parser.Stack.Peek() as Build;
        if (build.Id != match.Groups["Id"].Value)
          throw new Exception("Build id mismatch");
        build.FinishTime = row.Time;
        parser.Stack.Pop();
      }
      return match.Success;
    }

    public override bool Match(BambooLogParser parser, Row row)
    {
      return matchBuildStarted(parser, row) || matchBuildFinished(parser, row);
    }
  }
}
