namespace CI.Visual.Lib.Helpers;

public static class UnhandledExceptionHndlr // Core 3
{
  public static ILogger? Logger { get; set; }

  public static void OnCurrentDispatcherUnhandledException(object s, DispatcherUnhandledExceptionEventArgs e)
  {
    if (e != null)
      e.Handled = true;

    var details = $"  {e?.Exception.InnerMessages()}\n{e?.Exception.StackTrace}";

    try
    {
      Logger?.LogError(e?.Exception, details);
      Clipboard.SetText(details);

      if (Debugger.IsAttached)
      {
        TraceError(details);
        Debugger.Break();
      }
      else if (MessageBox.Show(details, "Unhandled Exception - Do you want to continue?", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.Yes) == MessageBoxResult.No)
      {
        Logger?.LogInformation("Safe decision: to aborrt execution.");
        Application.Current.Shutdown(44);
      }
      else
      {
        Logger?.LogInformation("Brave decision: to continue execution.");
      }
    }
    catch (Exception ex)
    {
      Environment.FailFast(details, ex); //tu: http://blog.functionalfun.net/2013/05/how-to-debug-silent-crashes-in-net.html // Capturing dump files with Windows Error Reporting: Db a key at HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\Windows Error Reporting\LocalDumps\[Your Application Exe FileName]. In that key, create a string value called DumpFolder, and set it to the folder where you want dumps to be written. Then create a DWORD value called DumpType with a value of 2.
    }
  }
}
