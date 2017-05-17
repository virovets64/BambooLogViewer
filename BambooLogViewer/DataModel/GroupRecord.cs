using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.DataModel
{
  public class GroupRecord: Record
  {
    public ObservableCollection<Record> Records
    {
      get
      {
        if (records == null)
          records = new ObservableCollection<Record>();
        return records;
      }
    }
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
    public int ErrorCount
    {
      get
      {
        return errorCount;
      }
    }
    public int WarningCount
    {
      get
      {
        return warningCount;
      }
    }
    public bool HasWarnings
    {
      get
      {
        return warningCount > 0;
      }
    }
    public bool HasErrors
    {
      get
      {
        return errorCount > 0;
      }
    }


    public void Add(Record record)
    {
      Records.Add(record);
    }

    public override void Update()
    {
      errorCount = 0;
      warningCount = 0;
      foreach(var record in Records)
      {
        record.Update();
        var group = record as GroupRecord;
        if(group != null)
        {
          errorCount += group.errorCount;
          warningCount += group.warningCount;
        }
        else
        {
          if (record.Severity == MessageSeverity.Error)
            errorCount++;
          if (record.Severity == MessageSeverity.Warning)
            warningCount++;
        }
      }
    }

    private ObservableCollection<Record> records;
    protected int errorCount = 0;
    protected int warningCount = 0;
  }
}
