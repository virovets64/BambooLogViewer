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
    private static Regex regexProjectNumber = new Regex(@"^(?<Number>\d+)\>(?<Message>.*)$");
    private static Regex regexError = new Regex(@"^.+: (?<Severity>warning|error|fatal error) (\w+):");

    bool matchProjectStarted(BambooLogParser parser, Row row)
    {
      var match = regexVSProjectStarted.Match(row.Message);
      if (match.Success)
      {
        var task = parser.Stack.Peek() as Task;
        var project = new VSProject();
        project.Time = row.Time;
        setMatchedProperties(project, match.Groups, regexVSProjectStarted.GetGroupNames());
        task.VSProjects.Add(project.Number, project);
        task.Add(project);
      }
      return match.Success;
    }

    bool matchProjectRecord(BambooLogParser parser, Row row)
    {
      if (parser.Stack.Count == 0)
        return false;
      var task = parser.Stack.Peek() as Task;
      if (task == null)
        return false;

      var match = regexProjectNumber.Match(row.Message);
      if (match.Success)
      {
        var record = new SimpleRecord();
        record.Kind = row.Kind;
        record.Time = row.Time;
        record.Message = match.Groups["Message"].Value;
        var projectNumber = match.Groups["Number"].Value;
        var project = task.VSProjects[projectNumber];
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

    public override bool Match(BambooLogParser parser, Row row)
    {
      return matchProjectStarted(parser, row) || matchProjectRecord(parser, row);
    }
  }
}
