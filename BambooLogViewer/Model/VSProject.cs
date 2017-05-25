using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BambooLogViewer.Model
{
  public class VSProject: GroupRecord
  {
    public string Number { get; set; }
    public string Name { get; set; }
    public string Target { get; set; }
    public string Configuration { get; set; }
  }
}
