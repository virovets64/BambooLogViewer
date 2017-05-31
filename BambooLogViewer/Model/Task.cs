using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BambooLogViewer.Model
{
  public class Task : GroupRecord
  {
    public string Name { get; set; }
    public string Result
    {
      get
      {
        return Failed ? "fail" : "success";
      }
      set
      {
        Failed = value.Trim().ToLower() != "success";
      }
    }
  }
}
