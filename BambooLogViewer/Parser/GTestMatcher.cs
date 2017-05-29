using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BambooLogViewer.Model;

namespace BambooLogViewer.Parser
{
  public class GTestMatcher : Matcher
  {
    private static Regex regexGTestRunStarted = new Regex(@"^\[==========\] Running (?<TestCount>\d+) test[s|] from (?<CaseCount>\d+) test cases?.$");
    private static Regex regexGTestRunFinished = new Regex(@"^\[==========\] (?<TestCount>\d+) tests? from (?<CaseCount>\d+) test cases? ran. \((?<Milliseconds>\d+) ms total\)$");
    private static Regex regexGTestCaseStarted = new Regex(@"^\[----------\] (?<TestCount>\d+) tests? from (?<Name>_\w+|[\w-[0-9_]]\w*)$");
    private static Regex regexGTestCaseFinished = new Regex(@"^\[----------\] (?<TestCount>\d+) tests? from (?<Name>_\w+|[\w-[0-9_]]\w*) \((?<Milliseconds>\d+) ms total\)$");
    private static Regex regexGTestParamStarted = new Regex(@"^\[----------\] (?<TestCount>\d+) tests? from (?<Name>_\w+|[\w-[0-9_]]\w*)/(?<Index>\d+), where (?<Parameter>.+)$");
    private static Regex regexGTestParamFinished = new Regex(@"^\[----------\] (?<TestCount>\d+) tests? from (?<Name>_\w+|[\w-[0-9_]]\w*)/(?<Index>\d+) \((?<Milliseconds>\d+) ms total\)$");
    private static Regex regexGTestStarted = new Regex(@"^\[ RUN      \] (?<CaseName>_\w+|[\w-[0-9_]]\w*)(/\d+)?.(?<Name>_\w+|[\w-[0-9_]]\w*)$");
    private static Regex regexGTestFinished = new Regex(@"\[(?<Result>       OK |  FAILED  )\] (?<CaseName>_\w+|[\w-[0-9_]]\w*)(/\d+)?.(?<Name>_\w+|[\w-[0-9_]]\w*) \((?<Milliseconds>\d+) ms\)$");

    private bool matchGTestRunStarted(BambooLogParser parser, Row row)
    {
      var match = regexGTestRunStarted.Match(row.Message);
      if (match.Success)
      {
        var task = parser.Stack.Peek() as Task;
        var test = new GTestRun();
        test.Time = row.Time;
        setMatchedProperties(test, match.Groups, regexGTestRunStarted.GetGroupNames());
        task.Add(test);
        parser.Stack.Push(test);
      }
      return match.Success;
    }

    private bool matchGTestRunFinished(BambooLogParser parser, Row row)
    {
      var match = regexGTestRunFinished.Match(row.Message);
      if (match.Success)
      {
        var test = parser.Stack.Peek() as GTestRun;
        test.Milliseconds = match.Groups["Milliseconds"].Value;
        parser.Stack.Pop();
      }
      return match.Success;
    }

    private bool matchGTestCaseStarted(BambooLogParser parser, Row row)
    {
      var match = regexGTestCaseStarted.Match(row.Message);
      if (match.Success)
      {
        var test = new GTestCase();
        test.Time = row.Time;
        setMatchedProperties(test, match.Groups, regexGTestCaseStarted.GetGroupNames());
        parser.Stack.Peek().Add(test);
        parser.Stack.Push(test);
      }
      return match.Success;
    }

    private bool matchGTestCaseFinished(BambooLogParser parser, Row row)
    {
      var match = regexGTestCaseFinished.Match(row.Message);
      if (match.Success)
      {
        var test = parser.Stack.Peek() as GTestCase;
        test.Milliseconds = match.Groups["Milliseconds"].Value;
        parser.Stack.Pop();
      }
      return match.Success;
    }

    private bool matchGTestParamStarted(BambooLogParser parser, Row row)
    {
      var match = regexGTestParamStarted.Match(row.Message);
      if (match.Success)
      {
        var test = new GTestCaseParametrized();
        test.Time = row.Time;
        setMatchedProperties(test, match.Groups, regexGTestParamStarted.GetGroupNames());
        parser.Stack.Peek().Add(test);
        parser.Stack.Push(test);
      }
      return match.Success;
    }

    private bool matchGTestParamFinished(BambooLogParser parser, Row row)
    {
      var match = regexGTestParamFinished.Match(row.Message);
      if (match.Success)
      {
        var test = parser.Stack.Peek() as GTestCaseParametrized;
        test.Milliseconds = match.Groups["Milliseconds"].Value;
        parser.Stack.Pop();
      }
      return match.Success;
    }

    private bool matchGTestStarted(BambooLogParser parser, Row row)
    {
      var match = regexGTestStarted.Match(row.Message);
      if (match.Success)
      {
        var test = new GTest();
        test.Time = row.Time;
        setMatchedProperties(test, match.Groups, regexGTestStarted.GetGroupNames());
        parser.Stack.Peek().Add(test);
        parser.Stack.Push(test);
      }
      return match.Success;
    }

    private bool matchGTestFinished(BambooLogParser parser, Row row)
    {
      var match = regexGTestFinished.Match(row.Message);
      if (match.Success)
      {
        var test = parser.Stack.Peek() as GTest;
        test.Result = match.Groups["Result"].Value;
        test.Milliseconds = match.Groups["Milliseconds"].Value;
        parser.Stack.Pop();
      }
      return match.Success;
    }

    public override bool Match(BambooLogParser parser, Row row)
    {
      return 
        matchGTestRunStarted(parser, row) ||
        matchGTestRunFinished(parser, row) ||
        matchGTestCaseStarted(parser, row) ||
        matchGTestCaseFinished(parser, row) ||
        matchGTestParamStarted(parser, row) ||
        matchGTestParamFinished(parser, row) ||
        matchGTestStarted(parser, row) ||
        matchGTestFinished(parser, row);
    }
  }
}
