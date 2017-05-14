using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.DataModel
{
  public enum MessageSeverity { Normal, Warning, Error };

  public class Record
  {
    public Record()
    {
      Severity = MessageSeverity.Normal;
    }
    public string Kind { get; set; }
    public DateTime Time { get; set; }
    public string Message { get; set; }
    public MessageSeverity Severity;

    public bool IsError
    {
      get
      {
        return Severity == MessageSeverity.Error;
      }
    }
    public bool IsWarning
    {
      get
      {
        return Severity == MessageSeverity.Warning;
      }
    }
  }
}
