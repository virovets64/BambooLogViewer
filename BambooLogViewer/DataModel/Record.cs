using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.DataModel
{
  public class Record
  {
    public string Kind { get; set; }
    public DateTime Time { get; set; }
    public string Message { get; set; }
  }
}
