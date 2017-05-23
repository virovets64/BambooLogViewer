using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.ViewModel
{
  [Model.TargetType(typeof(Model.GTestRun))]
  public class GTestRun : GroupRecord
  {
    public GTestRun(Model.GTestRun record)
      : base(record)
    { }
    private Model.GTestRun model { get { return (Model.GTestRun)modelRecord; } }

    public string TestCount { get { return model.TestCount; } }
    public string CaseCount { get { return model.CaseCount; } }
  }
}
