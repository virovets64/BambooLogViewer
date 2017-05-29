using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BambooLogViewer.Model;
using System.Reflection;

namespace BambooLogViewer.Parser
{
  public class BambooLogParser
  {
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

    private BambooLogParser()
    {
      matchers = Assembly.GetExecutingAssembly().GetTypes()
        .Where(x => x.IsSubclassOf(typeof(Matcher)) && !x.IsAbstract)
        .Select(x => (Matcher)Activator.CreateInstance(x))
        .ToList();
    }

    public BambooLog Log { get { return log; } }
    public Stack<GroupRecord> Stack { get { return groupStack; } }

    private List<Matcher> matchers;
    private BambooLog log = new BambooLog();
    private Stack<GroupRecord> groupStack = new Stack<GroupRecord>();

    private void run(IEnumerable<string> lines)
    {
      var tabDelimiter = new char[] { '\t' };
      DateTime? lastRecordTime = null;
      foreach (string line in lines)
      {
        if (line.Trim() == "")
          continue;
        var columns = line.Split(tabDelimiter, 3);
        if(columns[2].Trim() == "")
          continue;
        var row = new Row { Kind = columns[0], Time = DateTime.Parse(columns[1]), Message = columns[2] };
        lastRecordTime = row.Time;

        bool matched = matchers.Any(x => x.Match(this, row));
        if (matched)
          continue;

        if(groupStack.Count != 0)
        {
          var record = new SimpleRecord();
          record.Kind = row.Kind;
          record.Time = row.Time;
          record.Message = row.Message;
          groupStack.Peek().Add(record);
        }
      }
      if(lastRecordTime.HasValue)
        log.fixFinishTime(lastRecordTime.Value);
    }
  }
}
