using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    public TimeSpan Duration { get { return modelRecord.Duration; } }

    public double RelativeDuration { get; set; }

    public bool IsSelected { get { return isSelected; } set { isSelected = value; } }
    private bool isSelected = false;

    public virtual void Update(BambooLog.Context context)
    { }

    public virtual int getWarningCount()
    {
      return 0;
    }
    public virtual int getErrorCount()
    {
      return 0;
    }

    public static Record createViewModel(Model.Record modelRecord)
    {
      var mappedType = Model.TypeMap.Get(modelRecord.GetType());
      return (mappedType != null) ?
        (Record)Activator.CreateInstance(mappedType, modelRecord) :
        new Record(modelRecord);
    }
  }
}
