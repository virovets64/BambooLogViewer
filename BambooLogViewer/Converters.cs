using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace BambooLogViewer.View
{
  [ValueConversion(typeof(String), typeof(Color))]
  public class MessageColorConverter : IValueConverter
  {
    private static Dictionary<string, Color> map = new Dictionary<string, Color>()
    {
      { "warning", Color.FromRgb(255, 255, 224) },
      { "error", Color.FromRgb(255, 224, 224) },
      { "fatal error", Color.FromRgb(255, 224, 224) },
    };

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Color color;
      if (!map.TryGetValue((string)value, out color))
        color = Colors.White;
      return new SolidColorBrush(color);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return "";
    }
  }
}