namespace WpfUserControlLib.Helpers;

public static class UnhandledExceptionHndlrUI // Core 3
{
  public static ILogger? Logger { get; set; }

  public static void OnCurrentDispatcherUnhandledException(object s, DispatcherUnhandledExceptionEventArgs ea)
  {
    ea.Handled = true;

    var innerMessages = $"  {ea.Exception.InnerMessages()}\n";

    try
    {
      if (Debugger.IsAttached && DevOps.IsDbg)
      {
        WriteLine($"\n██\n{innerMessages}\n██\n");
        Beep.Play();
        //Debugger.Break();
        return;
      }

      Logger?.LogError(ea.Exception, "■");
      Clipboard.SetText(innerMessages);

      ea.Exception.Pop("Unhandled Exception - ?Auto continue if not aborted?"); //redundant: Pop is doing it: ExnLogr.OpenVsOnTheCulpritLine(ea.Exception.StackTrace?.Split('\n').Where(r => r.Contains(".cs")).ToList().FirstOrDefault() ?? "■ 321");
    }
    catch (Exception ex)
    {
      Logger?.LogError(ex, innerMessages);
      WriteLine($"\n██\n{ex.Message}\n██\n");
      Environment.FailFast(innerMessages, ex); //tu: http://blog.functionalfun.net/2013/05/how-to-debug-silent-crashes-in-net.html // Capturing dump files with Windows Error Reporting: Db a key at HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\Windows Error Reporting\LocalDumps\[Your Application Exe FileName]. In that key, create a string value called DumpFolder, and set it to the folder where you want dumps to be written. Then create a DWORD value called DumpType with a value of 2.
    }
  }
}