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
    public string Milliseconds { get; set; }
    public string Result
    {
      get
      {
        return Failed? "FAILED" : "OK";
      }
      set
      {
        Failed = value.Trim() != "OK";
      }
    }
  }
}
