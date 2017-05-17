using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.DataModel
{
  public class VSProject: GroupRecord
  {
    public string Number { get; set; }
    public string Name { get; set; }
    public string Target { get; set; }
    public string Configuration { get; set; }
  }
}
