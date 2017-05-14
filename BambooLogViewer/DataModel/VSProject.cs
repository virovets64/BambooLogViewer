﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BambooLogViewer.DataModel
{
  public class VSProject: GroupRecord
  {
    public VSProject()
    {
      Records = new ObservableCollection<Record>();
      Errors = 0;
      Warnings = 0;
    }
    public string Number { get; set; }
    public string Name { get; set; }
    public string Target { get; set; }
    public string Configuration { get; set; }
    public int Errors { get; set; }
    public int Warnings { get; set; }
    public ObservableCollection<Record> Records { get; set; }
  }
}