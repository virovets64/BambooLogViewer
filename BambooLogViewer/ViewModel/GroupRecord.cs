﻿using System;
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
    protected int childErrorCount = 0;
    protected int childWarningCount = 0;
    private double relativeDiration = 0;

    public TimeSpan Duration { get { return model.Duration; } }
    public double RelativeDuration { get { return relativeDiration; } set { } }
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


    public override void Update()
    {
      records = model.Records.Select(x => createViewModel(x)).ToList();

      childErrorCount = 0;
      childWarningCount = 0;
      TimeSpan maxChildDuration = TimeSpan.Zero;
      foreach (var record in Records)
      {
        record.Update();
        childErrorCount += record.getErrorCount();
        childWarningCount += record.getWarningCount();
        var group = record as GroupRecord;
        if (group != null)
        {
          if (group.Duration > maxChildDuration)
            maxChildDuration = group.Duration;
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