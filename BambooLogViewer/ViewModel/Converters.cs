using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using BambooLogViewer.Model;
using System.Windows;

namespace BambooLogViewer.ViewModel
{
  [ValueConversion(typeof(MessageSeverity), typeof(Color))]
  public class SeverityToColorConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      switch((MessageSeverity)value)
      {
        case MessageSeverity.Error:
          return new SolidColorBrush(Colors.Red);
        case MessageSeverity.Warning:
          return new SolidColorBrush(Colors.Yellow);
      }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(MessageSeverity), typeof(Visibility))]
  public class SeverityToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      switch ((MessageSeverity)value)
      {
        case MessageSeverity.Error:
        case MessageSeverity.Warning:
          return Visibility.Visible;
      }
      return Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(int), typeof(Visibility))]
  public class CountToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (int)value == 0 ? Visibility.Hidden : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

}