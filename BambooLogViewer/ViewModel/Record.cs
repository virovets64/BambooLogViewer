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
