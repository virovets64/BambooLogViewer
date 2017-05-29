using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.Model
{
  public class GTestBase : GroupRecord
  {
    public string Name { get; set; }
    public string Milliseconds
    {
      get
      {
        return milliseconds.ToString();
      }
      set
      {
        milliseconds = int.Parse(value);
        Duration = TimeSpan.FromMilliseconds(milliseconds);
      }
    }
    private int milliseconds;
  }


  public class GTest : GTestBase
  {
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
