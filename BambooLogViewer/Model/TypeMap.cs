using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BambooLogViewer.Model
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public class TargetType : Attribute
  {
    public TargetType(Type type)
    {
      this.type = type;
    }
    public Type getType()
    {
      return type;
    }
    private Type type;
  }

  public class TypeMap
  {
    private static Dictionary<Type, Type> map = new Dictionary<Type,Type>();
    public static void Initialize()
    {
      var types = Assembly.GetExecutingAssembly().GetTypes();
      foreach(var type in types)
      {
        var attribute = type.GetCustomAttribute(typeof(TargetType)) as TargetType;
        if (attribute != null)
          map.Add(attribute.getType(), type);
      }
    }

    public static Type Get(Type key)
    {
      Type result;
      if (!map.TryGetValue(key, out result))
        result = null;
      return result;
    }
  }
}
