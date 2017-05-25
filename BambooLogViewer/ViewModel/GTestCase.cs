using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BambooLogViewer.ViewModel
{
  [Model.TargetType(typeof(Model.GTestCase))]
  public class GTestCase : GroupRecord
  {
    public GTestCase(Model.GTestCase record)
      : base(record)
    { }
    private Model.GTestCase model { get { return (Model.GTestCase)modelRecord; } }

    public string Name { get { return model.Name; } }
  }

  [Model.TargetType(typeof(Model.GTestCaseParametrized))]
  public class GTestCaseParametrized : GTestCase
  {
    public GTestCaseParametrized(Model.GTestCaseParametrized record)
      : base(record)
    { }
    private Model.GTestCaseParametrized model { get { return (Model.GTestCaseParametrized)modelRecord; } }

    public string Index { get { return model.Index; } }
    public string Parameter { get { return model.Parameter; } }
  }

}
