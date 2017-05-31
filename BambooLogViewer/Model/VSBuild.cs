using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BambooLogViewer.Model
{
  public class VSBuild : GroupRecord
  {
    public string Target { get; set; }
    public string SucceededCount { get; set; }
    public string FailedCount { get; set; }
    public string SkippedCount { get; set; }

    public Dictionary<string, VSProject> Projects { get {  return projects; } }

    private Dictionary<string, VSProject> projects = new Dictionary<string, VSProject>();
  }
}
