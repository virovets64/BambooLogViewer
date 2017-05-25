using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BambooLogViewer.ViewModel
{
  public class BambooLog: GroupRecord
  {
    public BambooLog(Model.BambooLog record): base(record)
    {
      var context = new Context();
      Update(context);
    }

    public class Context
    {
      public bool FirstErrorFound = false;
    }
  }
}
