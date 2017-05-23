using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.Model
{
  public class GTest: GroupRecord
  {
    public string Name { get; set; }
    public string Result
    {
      get
      {
        return errorCount == 0? "OK" : "FAILED";
      }
      set
      {
        errorCount = value.Trim() == "OK" ? 0 : 1;
      }
    }
    public override void Update()
    { }
  }
}
