using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BambooLogViewer.DataModel
{
  public class Build
  {
    public Build()
    {
      Tasks = new ObservableCollection<PlanTask>();
    }

    public string Name { get; set; }
    public ObservableCollection<PlanTask> Tasks { get; set; }
  }
}
