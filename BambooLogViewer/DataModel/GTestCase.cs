using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.DataModel
{
  public class GTestCase: GroupRecord
  {
    public string Name { get; set; }
  }

  public class GTestCaseParametrized : GTestCase
  {
    public string Index { get; set; }
    public string Parameter { get; set; }
  }
}
