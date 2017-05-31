using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BambooLogViewer.ViewModel
{
  [Model.TargetType(typeof(Model.VSBuild))]
  public class VSBuild : GroupRecord
  {
    public VSBuild(Model.VSBuild record)
      : base(record)
    { }
    private Model.VSBuild model { get { return (Model.VSBuild)modelRecord; } }

    public string Target { get { return model.Target; } }
    public string SucceededCount { get { return model.SucceededCount; } }
    public string FailedCount { get { return model.FailedCount; } }
    public string SkippedCount { get { return model.SkippedCount; } }
  }

}
