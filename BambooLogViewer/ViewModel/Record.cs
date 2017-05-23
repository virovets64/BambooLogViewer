using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace BambooLogViewer.ViewModel
{
  public class Record
  {
    public Model.Record modelRecord { get; private set; }

    public Record(Model.Record record)
    {
      modelRecord = record;
    }
    public string Kind { get { return modelRecord.Kind; } }
    public DateTime Time { get { return modelRecord.Time; } }
    public string Message { get { return modelRecord.Message; } }
    public Model.MessageSeverity Severity { get { return modelRecord.Severity; } }

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

    public virtual void Update()
    { }

    public static Record createViewModel(Model.Record modelRecord)
    {
      var mappedType = Model.TypeMap.Get(modelRecord.GetType());
      return (mappedType != null) ?
        (Record)Activator.CreateInstance(mappedType, modelRecord) :
        new Record(modelRecord);
    }
  }
}
