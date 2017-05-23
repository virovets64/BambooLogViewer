using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BambooLogViewer.ViewModel
{
  [Model.TargetType(typeof(Model.GroupRecord))]
  public class GroupRecord: Record
  {
    public GroupRecord(Model.GroupRecord record): base(record)
    {
    }

    public List<Record> Records { get { return records; }}
    private List<Record> records = new List<Record>();
    private Model.GroupRecord model { get { return (Model.GroupRecord)modelRecord;  } }
    protected int errorCount = 0;
    protected int warningCount = 0;
    private double relativeDiration = 0;

    public TimeSpan Duration { get { return model.Duration; } }
    public double RelativeDuration { get { return relativeDiration; } set { } }
    public int ErrorCount { get { return errorCount; } }
    public int WarningCount { get { return warningCount; } }

    public Visibility ErrorBoxVisibility
    {
      get
      {
        return ErrorCount == 0? Visibility.Hidden : Visibility.Visible;
      }
    }

    public Visibility WarningBoxVisibility
    {
      get
      {
        return WarningCount == 0 ? Visibility.Hidden : Visibility.Visible;
      }
    }

    public override void Update()
    {
      records = model.Records.Select(x => createViewModel(x)).ToList();

      errorCount = 0;
      warningCount = 0;
      TimeSpan maxChildDuration = TimeSpan.Zero;
      foreach (var record in Records)
      {
        record.Update();
        var group = record as GroupRecord;
        if (group != null)
        {
          errorCount += group.errorCount;
          warningCount += group.warningCount;
          if (group.Duration > maxChildDuration)
            maxChildDuration = group.Duration;
        }
        else
        {
          if (record.Severity == Model.MessageSeverity.Error)
            errorCount++;
          if (record.Severity == Model.MessageSeverity.Warning)
            warningCount++;
        }
      }
      if (maxChildDuration != TimeSpan.Zero)
      {
        foreach (var record in Records)
        {
          var group = record as GroupRecord;
          if (group != null)
            group.relativeDiration = (double)group.Duration.Ticks / maxChildDuration.Ticks;
        }
      }
    }
  }
}
