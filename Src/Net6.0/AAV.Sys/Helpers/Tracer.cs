namespace AAV.Sys.Helpers;

public static partial class Tracer // .NET Core 3.*
{
  static string LogFolder_OneDrive => Path.Combine(OneDrive.Root, @"Public\Logs");
  static string LogFolder_FallbackZ => @".\Logs";

  public static string RemoteLogFolder => LogFolder_FallbackZ; // backward compat.
  public static string SetupTracingOptions(string appName, TraceSwitch appTraceLvl, bool is4wk = false)
  {
    var logFilename = GetLogPathFileName(appName, is4wk);

    try
    {
      var listener = new TextWriterTraceListener(logFilename) { Filter = new ErrorFilter() };
      //WriteLine($" *** IsThreadSafe={listener.IsThreadSafe}.   UseGlobalLock={UseGlobalLock}.   Logging to '{logFilename}'."); => always this: "*** IsThreadSafe=False.   UseGlobalLock=True.   Logging to 'C:\Users\alexp\OneDrive\Public\Logs\AAV-WPF-le@RAZ~XPa.txt'."
      Listeners.Add(listener);
      AutoFlush = true;

#if NotPhasedOut // phased out in favour of DI's on Core 3.
      ReportErrorLevel(appTraceLvl, "* Current *");
      ReportErrorLevel(ExnLogr.AppTraceLevelCfg, "AAV.Sys.CFG");
#endif
    }
    catch { Listeners.Clear(); throw; } // https://www.codeguru.com/csharp/.net/article.php/c19405/Tracing-in-NET-and-Implementing-Your-Own-Trace-Listeners.htm

    return logFilename;
  }
#if NotPhasedOut // phased out in favour of DI's on Core 3.
  public static void ReportErrorLevel(TraceSwitch appTraceLvl, string src)
  {
    src = appTraceLvl.ToString();
    Write($" *** TraceLevel by  {src,-14}  includes:  ");
    WriteIf(true == appTraceLvl?.TraceError,     /**/ $"{TraceLevel.Error}   ");
    WriteIf(true == appTraceLvl?.TraceWarning,   /**/ $"{TraceLevel.Warning}   ");
    WriteIf(true == appTraceLvl?.TraceInfo,      /**/ $"{TraceLevel.Info}   ");
    WriteIf(true == appTraceLvl?.TraceVerbose,   /**/ $"{TraceLevel.Verbose}");
    Write("\n");
  }
#endif

  public static string GetLogPathFileName(string appName, bool is4wk)
  {
    var filename = getLogPathFileName(appName, false, is4wk);

    return FileExistAndIsLocked(new FileInfo(filename)) ? getLogPathFileName(appName, true, is4wk) : filename;
  }

  static string getLogPathFileName(string appName, bool isRandom, bool is4wk, bool is1FilePerSession = false)
  {
    var len = Environment.UserName.Length;
    var nm2 = len > 4 ? Environment.UserName.Substring(3, 2) : Environment.UserName[(len - 2)..];
    var filename = Path.Combine(getLogPath(is4wk), appName +
      (is1FilePerSession ? $"-{DateTimeOffset.Now:MMdd.HHmm}" : "") +
      $"-{Environment.UserName.Substring(1, 2)}@{Environment.MachineName[..3]}~{nm2.ToUpperInvariant()}{Environment.UserName[..1].ToLowerInvariant()}~{(isRandom ? Path.GetRandomFileName().Replace(".", "") : "")}-.log"); // .log extension has better color coding in VSCode (2021-03)
    return filename;
  }

  static bool FileExistAndIsLocked(FileInfo file)
  {
    try
    {
      if (!file.Exists)
        return false;

      using var stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
      stream.Close();

      return false;
    }
    catch (IOException)
    {
      //the file is unavailable because it is:
      //still being written to
      //or being processed by another thread
      //or does not exist (has already been processed)
      return true;
    }
  }

  static string getLogPath(bool isApp4wk)
  {
    try
    {
      //if (DevOps.IsDbg)        return StandardLib.Helpers.OneDrive.Folder(@"Public\Logs");

      if (!isApp4wk && Environment.MachineName == "RAZER1") { } else { }

      var path = LogFolder_OneDrive; // Apr3: I think this cause err on Zoe's for AlexPi.Scr: LogFolder_FallbackZ;
      if (FSHelper.ExistsOrCreated(path)) return path;

      path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DevLog");
      if (FSHelper.ExistsOrCreated(path)) return path;

      path = LogFolder_FallbackZ;
      if (FSHelper.ExistsOrCreated(path)) return path;
    }
    catch (DirectoryNotFoundException ex) { _ = ex.Log(); }
    catch (IOException ex) { _ = ex.Log(); }
    catch (Exception ex) { _ = ex.Log(); throw; }

    return @".\";
  }
}

//public static Exception GetInnermostException(Exception ex) => ex.InnerException == null ? ex : GetInnermostException(ex.InnerException);

// under construction: <== and has no effect on filtering lower level messages !!!
// https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.tracefilter.shouldtrace?f1url=https%3A%2F%2Fmsdn.microsoft.com%2Fquery%2Fdev15.query%3FappId%3DDev15IDEF1%26l%3DEN-US%26k%3Dk(System.Diagnostics.TraceFilter.ShouldTrace)%3Bk(TargetFrameworkMoniker-.NETFramework%2CVersion%3Dv4.7)%3Bk(DevLang-csharp)%26rd%3Dtrue%26f%3D255%26MSPPError%3D-2147217396&view=netframework-4.7.1#Examples
public class CrtclFilter : TraceFilter { public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data, object[] dataArray) => eventType == TraceEventType.Critical; }
public class ErrorFilter : TraceFilter { public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data, object[] dataArray) => eventType == TraceEventType.Error; }
public class WarngFilter : TraceFilter { public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data, object[] dataArray) => eventType == TraceEventType.Warning; }
public class InfonFilter : TraceFilter { public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data, object[] dataArray) => eventType == TraceEventType.Information; }
public class VerbsFilter : TraceFilter { public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data, object[] dataArray) => eventType == TraceEventType.Verbose; }