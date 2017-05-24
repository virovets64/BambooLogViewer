using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BambooLogViewer.ViewModel
{
  [Model.TargetType(typeof(Model.SimpleRecord))]
  public class SimpleRecord : Record
  {
    public SimpleRecord(Model.SimpleRecord record)
      : base(record)
    { }
    private Model.SimpleRecord model { get { return (Model.SimpleRecord)modelRecord; } }

    public Model.MessageSeverity Severity { get { return model.Severity; } }

    public Brush BulletColor
    {
      get
      {
        switch (Severity)
        {
          case Model.MessageSeverity.Error:
            return new SolidColorBrush(Colors.Red);
          case Model.MessageSeverity.Warning:
            return new SolidColorBrush(Colors.Yellow);
        }
        return null;
      }
    }

    public Visibility BulletVisibility
    {
      get
      {
        return Severity == Model.MessageSeverity.Normal ? Visibility.Hidden : Visibility.Visible;
      }
    }

    public override int getWarningCount()
    {
      return Severity == Model.MessageSeverity.Warning? 1 : 0;
    }
    public override int getErrorCount()
    {
      return Severity == Model.MessageSeverity.Error ? 1 : 0;
    }

  }

}
