using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BambooLogViewer.Model
{
  public class GroupRecord: Record
  {
    public List<Record> Records
    {
      get
      {
        if (records == null)
          records = new List<Record>();
        return records;
      }
    }
    public bool Failed { get; set; }

    public void Add(Record record)
    {
      Records.Add(record);
    }

    public override void fixFinishTime(DateTime time)
    {
      base.fixFinishTime(time);
      
      if(records != null)
      {
        Record previous = null;
        foreach (var record in Records)
        {
          if (previous != null)
            previous.fixFinishTime(record.Time);
          previous = record;
        }
        previous.fixFinishTime(time);
      }
    }

    private List<Record> records;
  }
}
