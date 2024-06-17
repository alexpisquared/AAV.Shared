namespace WpfUserControlLib.Helpers;

public static class UnhandledExceptionHndlr // Core 3
{
  public static ILogger? Logger { get; set; }

  public static void OnCurrentDispatcherUnhandledException(object s, DispatcherUnhandledExceptionEventArgs ea)
  {
    if (ea != null)
      ea.Handled = true;

    var details = $"  {ea?.Exception.InnerMessages()}\n{ea?.Exception.StackTrace}";

    try
    {
      Logger?.LogError(ea?.Exception, details);
      Clipboard.SetText(details);

      if (Debugger.IsAttached)
      {
        WriteLine($"\n██\n{details}\n██\n");
        Beep.Play();
        Debugger.Break();
        return;
      }

      StandardLib.Extensions.ExnLogr.OpenVsOnTheCulpritLine(ea?.Exception.StackTrace?.Split('\n').Where(r => r.Contains(".cs")).ToList().FirstOrDefault() ?? "■ 321");

      ea?.Exception.Pop("Unhandled Exception - Auto continue if not aborted", Logger);
    }
    catch (Exception ex)
    {
      Environment.FailFast(details, ex); //tu: http://blog.functionalfun.net/2013/05/how-to-debug-silent-crashes-in-net.html // Capturing dump files with Windows Error Reporting: Db a key at HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\Windows Error Reporting\LocalDumps\[Your Application Exe FileName]. In that key, create a string value called DumpFolder, and set it to the folder where you want dumps to be written. Then create a DWORD value called DumpType with a value of 2.
    }
  }
}