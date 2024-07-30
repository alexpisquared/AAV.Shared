namespace StandardLib.Extensions;

public static class ExnLogr // the one and only .net core 3 (Dec2019)
{
  public const string CRLF = "\n   ";
  public static TraceSwitch AppTraceLevelCfg => new("CfgTraceLevelSwitch", "Switch in config file:  <system.diagnostics><switches><!--0-off, 1-error, 2-warn, 3-info, 4-verbose. --><add name='CfgTraceLevelSwitch' value='3' /> ");

  public static string Log(this Exception ex, string? optl = null, [CallerMemberName] string? cmn = "", [CallerFilePath] string cfp = "", [CallerLineNumber] int cln = 0)
  {
    var culpritLine = ex.StackTrace?.Split('\n').Where(r => r.Contains(".cs")).ToList().FirstOrDefault() ?? $"{cfp}:line {333}";
    var (csFilename, csFileline) = GetCulpritLineDetails(culpritLine);

    var msgForPopup = $"{ex?.InnerMessages()}  {ex?.GetType().Name} at {CRLF}{csFilename} ({csFileline}):    {CRLF}{cmn}()    {CRLF}{optl}{CRLF}try {{  }} catch ({ex?.GetType().Name} ex) {{ ex.Log(); }} // insert around {Path.GetFileName(csFilename)} ({csFileline})";

    WriteLine(msgForPopup); // WriteLine($"{DateTimeOffset.Now:HH:mm:ss.f}  {msgForPopup.Replace("\n", "  ").Replace("\r", "  ")}"); // .TraceError just adds the "ProgName.exe : Error : 0" line <= no use.

    TraceStackIfVerbose(ex);

    if (VersionHelper.IsDbg)
      Console.Beep(4567, 750); // BprKernel32Internal.ErrorFaF();

    if (Debugger.IsAttached)
      Debugger.Break();
    else if (VersionHelper.IsDbgOrRBD && (culpritLine is not null || cfp is not null))
      msgForPopup += OpenVsOnTheCulpritLine(culpritLine ?? cfp);

    return msgForPopup; //todo: catch (Exception fatalEx) { Environment.FailFast("An error occured whilst reporting an error.", fatalEx); }//tu: http://blog.functionalfun.net/2013/05/how-to-debug-silent-crashes-in-net.html //tu: Capturing dump files with Windows Error Reporting: Db a key at HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\Windows Error Reporting\LocalDumps\[Your Application Exe FileName]. In that key, create a string value called DumpFolder, and set it to the folder where you want dumps to be written. Then create a DWORD value called DumpType with a value of 2.
  }

  public static string OpenVsOnTheCulpritLine(string callStackEntryLine)
  {
    var (csFilename, csFileline) = GetCulpritLineDetails(callStackEntryLine);
    return OpenVsOnTheCulpritLine(csFilename, csFileline);
  }

  private static (string csFilename, int csFileline) GetCulpritLineDetails(string callStackEntryLine)
  {
    var parts = callStackEntryLine.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length < 2)
      return ($"■ Bad callStackEntryLine: {callStackEntryLine}", -1);
    else
      return (parts[parts.Length - 3], int.Parse(parts.Last().Trim('\r')));
  }

  static string OpenVsOnTheCulpritLine(string filename, int fileline)
  {
#if DotNet4
    EnvDTE80.DTE2 dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.17.0");
    dte2.MainWindow.Activate();
    EnvDTE.Window w = dte2.ItemOperations.OpenFile(filename, EnvDTE.Constants.vsViewKindTextView);
    ((EnvDTE.TextSelection)dte2.ActiveDocument.Selection).GotoLine(fileline, true);

    /* also see:
    https://docs.microsoft.com/en-us/visualstudio/extensibility/launch-visual-studio-dte?view=vs-2022
    https://github.com/diimdeep/VisualStudioFileOpenTool
    */
#elif false
    const string _dotnet4exe = """C:\g\Util\Src\OpenInVsOnTheCulpritLine6\bin\Release\net8.0-windows8.0\OpenInVsOnTheCulpritLine6.exe""";
    var result = CliWrap.Cli.Wrap(_dotnet4exe)  //tu: process.start alternative
        .WithArguments([filename, fileline.ToString()])
        //.WithWorkingDirectory("work/dir/path")
        .ExecuteAsync();
#elif true
    VsOpenerAttacher.OpenVsOnTheCulpritLine(filename, fileline);     //todo: test this call of the opener directly - not through EXE (Jun 2024)
#else
    const string _dotnet4exe = """C:\g\Util\Src\OpenInVsOnTheCulpritLine6\bin\Release\net8.0-windows8.0\OpenInVsOnTheCulpritLine6.exe""";
    if (File.Exists(_dotnet4exe))
      Process.Start(_dotnet4exe, $"{filename} {fileline}");
    else
      return $"  Missing VS opener EXE: {_dotnet4exe}";
#endif

    return $"";
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
