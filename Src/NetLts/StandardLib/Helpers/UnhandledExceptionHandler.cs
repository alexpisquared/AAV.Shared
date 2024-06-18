namespace StandardLib.Helpers;

public static class UnhandledExceptionHandler
{
  public static ILogger? Logger { get; set; }

  public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
  {
    var eo = (Exception)e.ExceptionObject;
        
    try
    {
      Logger?.LogError(eo, eo.Message);
      _ = eo.Log(); //redundant: Log is doing it: ExnLogr.OpenVsOnTheCulpritLine(ea.Exception.StackTrace?.Split('\n').Where(r => r.Contains(".cs")).ToList().FirstOrDefault() ?? "■ 321");
    }
    catch (Exception ex)
    {
      Logger?.LogError(ex, "■ Exception reporting exception");
      WriteLine($"\n██\n{ex.Message}\n{"■ Exception reporting exception"}██\n");
      Environment.FailFast("■ Exception reporting exception", ex); //tu: http://blog.functionalfun.net/2013/05/how-to-debug-silent-crashes-in-net.html // Capturing dump files with Windows Error Reporting: Db a key at HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\Windows Error Reporting\LocalDumps\[Your Application Exe FileName]. In that key, create a string value called DumpFolder, and set it to the folder where you want dumps to be written. Then create a DWORD value called DumpType with a value of 2.
    }
  }
}