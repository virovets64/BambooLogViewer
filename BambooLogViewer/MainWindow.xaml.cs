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
//using System.Windows.Shapes;
using System.Windows.Threading;
using BambooLogViewer.Model;
using System.Windows.Controls.Primitives;
using System.Threading;
using Microsoft.Win32;

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
      tcSource.SelectedIndex = Properties.Settings.Default.SourceTabIndex;
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      Properties.Settings.Default.SourceTabIndex = tcSource.SelectedIndex;
      Properties.Settings.Default.Save();
    }

    private void btnOpen_Click(object sender, RoutedEventArgs e)
    {
      Properties.Settings.Default.LastPath = editPath.Text;
      string text;
      using(new ProgressIndicator(this, "Reading the file..."))
      {
        text = File.ReadAllText(editPath.Text);
      }
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
      using (new ProgressIndicator(this, "Parsing the log..."))
      {
        var logFile = Parser.BambooLogParser.Parse(text);
        var logFileView = new ViewModel.BambooLog(logFile);
        trvLog.ItemsSource = logFileView.Records;
        trvLog.Focus();
      }
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

    private void treeViewItem_Expanded(object sender, RoutedEventArgs e)
    {
      TreeViewItem tvi = e.OriginalSource as TreeViewItem;
      if (tvi != null)
      {
        if (tvi.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
        {
          EventHandler itemsGenerated = null;
          itemsGenerated = delegate(object s, EventArgs args)
          {
            if ((s as ItemContainerGenerator).Status == GeneratorStatus.ContainersGenerated)
            {
              (s as ItemContainerGenerator).StatusChanged -= itemsGenerated;
              Dispatcher.BeginInvoke(new Action(() => Mouse.OverrideCursor = null), DispatcherPriority.ContextIdle, null);
            }
          };
          tvi.ItemContainerGenerator.StatusChanged += itemsGenerated;
          Mouse.OverrideCursor = Cursors.Wait;
        }
      }
    }

    public class ProgressIndicator : IDisposable
    {
      MainWindow mainWindow;
      public ProgressIndicator(MainWindow window, string text)
      {
        mainWindow = window;
        Mouse.OverrideCursor = Cursors.Wait;
        mainWindow.progressText.Text = text;
        refreshStatusBar();
      }

      public void Dispose()
      {
        Mouse.OverrideCursor = null;
        mainWindow.progressText.Text = "Ready";
        refreshStatusBar();
      }
      private void refreshStatusBar()
      {
        mainWindow.statusBar.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
      }
      private static Action EmptyDelegate = delegate() { };
    }

    private void btnBrowse_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      var path = editPath.Text;
      if(!String.IsNullOrEmpty(path))
      {
        var dir = Path.GetDirectoryName(path);
        if(Directory.Exists(dir))
          openFileDialog.InitialDirectory = dir;
      }
      if (openFileDialog.ShowDialog() == true)
        editPath.Text = openFileDialog.FileName;
    }
  }
}
