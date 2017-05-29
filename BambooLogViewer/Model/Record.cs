using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BambooLogViewer.Model
{
  abstract public class Record
  {
    public string Kind { get; set; }
    public DateTime Time { get; set; }
    public string Message { get; set; }
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
    public virtual void fixFinishTime(DateTime time)
    {
      if (Duration == TimeSpan.Zero)
        FinishTime = time;
    }
  }
}
