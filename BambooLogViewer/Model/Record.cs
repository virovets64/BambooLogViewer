using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BambooLogViewer.Model
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
    public MessageSeverity Severity { get; set; }
  }
}
