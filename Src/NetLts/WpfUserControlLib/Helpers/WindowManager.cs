﻿using System;
using System.Diagnostics;
using System.Linq;

namespace WpfUserControlLib.Helpers
{
  public class WindowManager
  {
    public void Arrange() { listProcesses();Trace.Write("°"); }

    void listProcesses()
    {
      var i = 0;
      var processCollection = Process.GetProcesses(".").Where(p => p.MainWindowHandle != IntPtr.Zero
        //&& p.ProcessName != "explorer"
        ).OrderBy(p => p.ProcessName);
      Trace.WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] {processCollection.Count(),3}  total:");

      foreach (var p in processCollection)
        Trace.WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] {++i,3}  {p.ProcessName,-22}  {p.MainWindowTitle}");

      Trace.WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] {processCollection.Count(),3}  total:");
    }
  }
}
