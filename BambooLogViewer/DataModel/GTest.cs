using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.DataModel
{
  public class GTest: GroupRecord
  {
    public string Name { get; set; }
    public string Result { get; set; }
  }
}
