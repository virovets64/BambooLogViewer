using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.ViewModel
{
  public class BambooLog: GroupRecord
  {
    public BambooLog(Model.BambooLog record): base(record)
    {
      Update();
    }
  }
}
