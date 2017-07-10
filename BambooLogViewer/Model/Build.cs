using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BambooLogViewer.Model
{
  public class Build: GroupRecord
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Number { get; set; }
    public string Agent { get; set; }
    public string Job { get; set; }
  }
}
