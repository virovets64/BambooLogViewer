using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BambooLogViewer.ViewModel
{
  [Model.TargetType(typeof(Model.Task))]
  public class Task: GroupRecord
  {
    public Task(Model.Task record): base(record)
    { }
    private Model.Task model { get { return (Model.Task)modelRecord; } }

    public string Name { get { return model.Name; } }
    public string Result { get { return model.Result; } }
    public bool Failed { get { return model.Failed; } }
  }
}
