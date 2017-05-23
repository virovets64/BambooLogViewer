using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.ViewModel
{
  [Model.TargetType(typeof(Model.PlanTask))]
  public class PlanTask: GroupRecord
  {
    public PlanTask(Model.PlanTask record): base(record)
    { }
    private Model.PlanTask model { get { return (Model.PlanTask)modelRecord; } }

    public string Name { get { return model.Name; } }
    public string Result { get { return model.Result; } }
    public bool Failed { get { return model.Failed; } }
  }
}
