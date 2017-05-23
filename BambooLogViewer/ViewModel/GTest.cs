using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.ViewModel
{
  [Model.TargetType(typeof(Model.GTest))]
  public class GTest : GroupRecord
  {
    public GTest(Model.GTest record)
      : base(record)
    { }
    private Model.GTest model { get { return (Model.GTest)modelRecord; } }

    public string Name { get { return model.Name; } }
    public string Result { get { return model.Result; } }
  }

}
