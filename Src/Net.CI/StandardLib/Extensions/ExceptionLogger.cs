namespace StandardLib.Extensions;

public static class ExnLogr // the one and only .net core 3 (Dec2019)
{
  public static TraceSwitch AppTraceLevelCfg => new("CfgTraceLevelSwitch", "Switch in config file:  <system.diagnostics><switches><!--0-off, 1-error, 2-warn, 3-info, 4-verbose. --><add name='CfgTraceLevelSwitch' value='3' /> ");

  public static string Log(this Exception ex, string? optl = null, [CallerMemberName] string? cmn = "", [CallerFilePath] string cfp = "", [CallerLineNumber] int cln = 0)
  {
    var msgForPopup = $"{ex?.InnerMessages()}\r\n{ex?.GetType().Name} at {cfp}({cln}): {cmn}() {optl}";

    WriteLine($"[xx:xx:xx Trc] {DateTimeOffset.Now:yy.MM.dd HH:mm:ss.f} ██ {msgForPopup.Replace("\n", "  "/*, StringComparison.Ordinal*/).Replace("\r", "  "/*, StringComparison.Ordinal*/)}"); // .TraceError just adds the "ProgName.exe : Error : 0" line <= no use.

    TraceStackIfVerbose(ex);

    if (VersionHelper.IsDbg)
      BprKernel32Internal.ErrorFaF();

    if (Debugger.IsAttached)
      Debugger.Break();

    return msgForPopup; //todo: catch (Exception fatalEx) { Environment.FailFast("An error occured whilst reporting an error.", fatalEx); }//tu: http://blog.functionalfun.net/2013/05/how-to-debug-silent-crashes-in-net.html //tu: Capturing dump files with Windows Error Reporting: Db a key at HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\Windows Error Reporting\LocalDumps\[Your Application Exe FileName]. In that key, create a string value called DumpFolder, and set it to the folder where you want dumps to be written. Then create a DWORD value called DumpType with a value of 2.
  }

  static void TraceStackIfVerbose(Exception? ex)
  {
    if (AppTraceLevelCfg.TraceVerbose)
    {
      var prevLv = IndentLevel;
      var prevSz = IndentSize;
      IndentLevel = 2;
      IndentSize = 2;
      WriteLineIf(AppTraceLevelCfg.TraceVerbose, ex?.StackTrace);
      IndentLevel = prevLv;
      IndentSize = prevSz;
    }
  }

  public static string InnerMessages(this Exception? ex, char delimiter = '\n')
  {
    StringBuilder sb = new();
    while (ex != null)
    {
      _ = sb.Append($"{ex.Message}{delimiter}");
      ex = ex.InnerException;
    }

    return sb.ToString();
  }
  public static string InnermostMessage(this Exception ex)
  {
    while (ex != null)
    {
      if (ex.InnerException == null)
        return ex.Message;

      ex = ex.InnerException;
    }

    return "This is very-very odd.";
  }

  #region Proposals - cop
  #endregion
}
