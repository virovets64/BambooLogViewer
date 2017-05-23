using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.ViewModel
{
  [Model.TargetType(typeof(Model.Build))]
  public class Build: GroupRecord
  {
    public Build(Model.Build record): base(record)
    { }
    private Model.Build model { get { return (Model.Build)modelRecord; }}

    public string Id { get { return model.Id; } }
    public string Name { get { return model.Name; } }
    public string Number { get { return model.Number; } }
    public string Agent { get { return model.Agent; } }
  }
}
