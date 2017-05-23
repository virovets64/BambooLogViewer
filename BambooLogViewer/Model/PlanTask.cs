using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BambooLogViewer.Model
{
  public class PlanTask : GroupRecord
  {
    public PlanTask()
    {
      VSProjects = new Dictionary<string, VSProject>();
    }

    public string Name { get; set; }
    public string Result { get; set; }
    public Dictionary<string, VSProject> VSProjects { get; set; }

    public bool Failed
    {
      get 
      {
        return !Result.Equals("success", StringComparison.InvariantCultureIgnoreCase);
      }
    }
  }
}
