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
    private ObservableCollection<Record> records;
  }
}
