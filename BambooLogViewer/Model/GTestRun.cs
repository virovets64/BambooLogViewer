using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.Model
{
  public class GTestRun: GroupRecord
  {
    public string TestCount { get; set; }
    public string CaseCount { get; set; }
  }
}
