using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    protected int childErrorCount = 0;
    protected int childWarningCount = 0;

    public int ChildErrorCount { get { return childErrorCount; } }
    public int ChildWarningCount { get { return childWarningCount; } }

    public Visibility ErrorBoxVisibility
    {
      get
      {
        return ChildErrorCount == 0? Visibility.Hidden : Visibility.Visible;
      }
    }

    public Visibility WarningBoxVisibility
    {
      get
      {
        return ChildWarningCount == 0 ? Visibility.Hidden : Visibility.Visible;
      }
    }

    public Visibility BulletVisibility
    {
      get
      {
        return model.Failed && ChildErrorCount == 0 ? Visibility.Visible : Visibility.Hidden;
      }
    }

    public override int getWarningCount()
    {
      return childWarningCount;
    }
    public override int getErrorCount()
    {
      return model.Failed && childErrorCount == 0 ? 1 : childErrorCount;
    }


    public override void Update(BambooLog.Context context)
    {
      records = model.Records.Select(x => createViewModel(x)).ToList();

      childErrorCount = 0;
      childWarningCount = 0;
      TimeSpan maxChildDuration = TimeSpan.Zero;

      bool firstErrorFound = context.FirstErrorFound;

      foreach (var record in Records)
      {
        record.Update(context);
        childErrorCount += record.getErrorCount();
        childWarningCount += record.getWarningCount();
        if (record.Duration > maxChildDuration)
          maxChildDuration = record.Duration;
      }
      if (maxChildDuration != TimeSpan.Zero)
      {
        foreach (var record in Records)
        {
          record.RelativeDuration = (double)record.Duration.Ticks / maxChildDuration.Ticks;
        }
      }
      if (!firstErrorFound && childErrorCount > 0)
      {
        IsExpanded = true;
      }
    }

    public bool IsExpanded { get { return isExpanded; } set { isExpanded = value; } }
    private bool isExpanded = false;
  }
}
