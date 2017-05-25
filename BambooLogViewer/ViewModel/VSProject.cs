using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BambooLogViewer.ViewModel
{
  [Model.TargetType(typeof(Model.VSProject))]
  public class VSProject : GroupRecord
  {
    public VSProject(Model.VSProject record)
      : base(record)
    { }
    private Model.VSProject model { get { return (Model.VSProject)modelRecord; } }

    public string Number { get { return model.Number; } }
    public string Name { get { return model.Name; } }
    public string Target { get { return model.Target; } }
    public string Configuration { get { return model.Configuration; } }
  }

}
