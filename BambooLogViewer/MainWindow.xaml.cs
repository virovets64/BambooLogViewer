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
  }
}
