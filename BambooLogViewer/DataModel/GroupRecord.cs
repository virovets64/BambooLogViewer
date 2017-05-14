using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.DataModel
{
  public class GroupRecord: Record
  {
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
  }
}
