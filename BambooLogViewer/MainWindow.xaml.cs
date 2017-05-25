using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BambooLogViewer.Model;

namespace BambooLogViewer
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      Model.TypeMap.Initialize();
      InitializeComponent();
      editPath.Text = Properties.Settings.Default.LastPath;
      setScaleFactor(Properties.Settings.Default.ScaleFactor);
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      Properties.Settings.Default.Save();
    }

    private void btnOpen_Click(object sender, RoutedEventArgs e)
    {
      Properties.Settings.Default.LastPath = editPath.Text;
      var text = File.ReadAllText(editPath.Text);
      initializeFromString(text);
      Properties.Settings.Default.Save();
    }

    private void btnPaste_Click(object sender, RoutedEventArgs e)
    {
      string text = Clipboard.GetText();
      if(!String.IsNullOrEmpty(text))
      {
        initializeFromString(text);
      }
    }

    private void initializeFromString(string text)
    {
      var logFile = Parser.BambooLogParser.Parse(text);
      var logFileView = new ViewModel.BambooLog(logFile);
      trvLog.ItemsSource = logFileView.Records;
      trvLog.Focus();
    }

    private double scaleFactor = 0;
    private const double scaleStep = 0.1;
    private const double scaleMin = -2;
    private const double scaleMax = 3;

    private void setScaleFactor(double value)
    {
      var scaleTransform = mainPanel.LayoutTransform as ScaleTransform;
      if (scaleTransform == null)
      {
        scaleTransform = new ScaleTransform(1, 1);
        mainPanel.LayoutTransform = scaleTransform;
      }
      if(value >= scaleMin && value <= scaleMax)
      {
        scaleFactor = value;
        var scale = Math.Pow(2, scaleFactor);
        scaleTransform.ScaleX = scale;
        scaleTransform.ScaleY = scale;
      }
    }

    private void mainPanel_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
      {
        if (e.Delta > 0)
        {
          setScaleFactor(scaleFactor + scaleStep);
        }
        else if (e.Delta < 0)
        {
          setScaleFactor(scaleFactor - scaleStep);
        }
        Properties.Settings.Default.ScaleFactor = scaleFactor;
        e.Handled = true;
      }
    }
  }
}
