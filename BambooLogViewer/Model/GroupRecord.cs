using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public TimeSpan Duration { get; set; }
    
    public DateTime FinishTime 
    { 
      get
      {
        return Time + Duration;
      }
      set
      {
        Duration = value - Time;
      }
    }

    public void Add(Record record)
    {
      Records.Add(record);
    }

    private List<Record> records;
  }
}
