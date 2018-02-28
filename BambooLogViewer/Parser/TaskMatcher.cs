using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BambooLogViewer.Model;

namespace BambooLogViewer.Parser
{
  public class TaskMatcher : Matcher
  {
    private static Regex regexTaskStarted = new Regex(@"^Starting task '(?<Name>.+)' of type '(?<Type>.+)'$");
    private static Regex regexTaskFinished = new Regex(@"^Finished task '(?<Name>.+)' with result: (?<Result>.+)$");

    bool matchTaskStarted(BambooLogParser parser, Row row)
    {
      var match = regexTaskStarted.Match(row.Message);
      if (match.Success)
      {
        var task = new Task();
        task.Time = row.Time;
        setMatchedProperties(task, match.Groups, regexTaskStarted.GetGroupNames());
        parser.Stack.Peek().Add(task);
        parser.Stack.Push(task);
      }
      return match.Success;
    }

    bool matchTaskFinished(BambooLogParser parser, Row row)
    {
      var match = regexTaskFinished.Match(row.Message);
      if (match.Success)
      {
        var task = (Task)parser.Stack.Peek();
        if (task.Name != match.Groups["Name"].Value)
          throw new Exception("Task name mismatch");
        task.FinishTime = row.Time;
        task.Result = match.Groups["Result"].Value;
        parser.Stack.Pop();
      }
      return match.Success;
    }

    public override bool Match(BambooLogParser parser, Row row)
    {
      return matchTaskStarted(parser, row) || matchTaskFinished(parser, row);
    }
  }
}
