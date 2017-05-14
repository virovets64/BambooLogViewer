using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.DataModel
{
  public class BambooLog
  {
    public BambooLog()
    {
      Builds = new ObservableCollection<Build>();
    }
    public ObservableCollection<Build> Builds { get; set; }
  }
}
