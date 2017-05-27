using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BambooLogViewer.Parser
{
  public abstract class Matcher
  {
    public abstract bool Match(BambooLogParser parser, Row row);

    public static void setMatchedProperties(object obj, GroupCollection groups, string[] groupNames)
    {
      var type = obj.GetType();
      foreach (var groupName in groupNames)
      {
        var group = groups[groupName];
        if (group.Success)
        {
          var propertyInfo = type.GetProperty(groupName);
          if (propertyInfo != null)
            propertyInfo.SetValue(obj, group.Value);
        }
      }
    }
  }

  public class Row
  {
    public string Kind;
    public DateTime Time;
    public string Message;
  }
}
