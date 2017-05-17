using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BambooLogViewer.DataModel
{
  public class Build: GroupRecord
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Number { get; set; }
    public string Agent { get; set; }

    public IEnumerable<PlanTask> Tasks
    {
      get
      {
        return Records.Where(x => x is PlanTask).Select(x => (PlanTask)x);
      }
    }

    public bool Failed
    {
      get
      {
        return Tasks.Any(x => x.Failed);
      }
    }
  }
}
