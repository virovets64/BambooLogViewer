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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BambooLogViewer.DataModel;

namespace BambooLogViewer
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      trvLog.ItemsSource = createSampleData().Builds;
    }

    BambooLog createSampleData()
    {
      var log = new BambooLog();
      {
        var build = new Build() { Name = "Build 1" };
        build.Tasks.Add(new PlanTask() { Name = "Task 11" });
        build.Tasks.Add(new PlanTask() { Name = "Task 12" });
        log.Builds.Add(build);
      }
      {
        var build = new Build() { Name = "Build 2" };
        build.Tasks.Add(new PlanTask() { Name = "Task 21" });
        log.Builds.Add(build);
      }

      return log;
    }
  }
}
