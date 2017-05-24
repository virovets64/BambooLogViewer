using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.Model
{
  public enum MessageSeverity { Normal, Warning, Error };

  public class SimpleRecord : Record
  {
    public SimpleRecord()
    {
      Severity = MessageSeverity.Normal;
    }
    public MessageSeverity Severity { get; set; }
  }
}
