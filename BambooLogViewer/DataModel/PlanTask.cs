using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BambooLogViewer.DataModel
{
  public class PlanTask : GroupRecord
  {
    public PlanTask()
    {
      VSProjects = new Dictionary<string, VSProject>();
      Records = new ObservableCollection<Record>();
    }

    public string Name { get; set; }
    public string Result { get; set; }
    public Dictionary<string, VSProject> VSProjects { get; set; }
    public ObservableCollection<Record> Records { get; set; }

    public bool Failed
    {
      get 
      {
        return !Result.Equals("success", StringComparison.InvariantCultureIgnoreCase);
      }
    }
  }
}
