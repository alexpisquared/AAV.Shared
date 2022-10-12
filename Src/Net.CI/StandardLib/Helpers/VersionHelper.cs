namespace StandardLib.Helpers;
public static class VersionHelper
{
  public static string CurVerStr(string fmt) => $"v{TimedVer.ToString(fmt)}  {CompileMode}";
  public static string CurVerStrYYMMDD => $"v{TimedVer:yy.MM.dd}  {CompileMode}";
  public static string CurVerStrYMd => $"v{TimedVer:yy.M.d}  {CompileMode}";
  public static string DotNetCoreVersion { get { try { return Environment.Version.ToString(); } catch (Exception ex) { return ex.Message; } } } // Gets a System.Version object that describes the major, minor, build, and revision numbers of the CLR (common language runtime). ---sadly, for/of the app only.
  public static string DevDbgAudit(IConfigurationRoot cfg, [CallerMemberName] string? cmn = "") => $"{CurVerStrYYMMDD}  " +
      @$"{Environment.MachineName}.{Environment.UserDomainName}\{Environment.UserName}  " +
      $"exe:{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}  " +  // $"log:{cfg?["LogFolder"]}  " +     //nogo: no point
      $".dn:{DotNetCoreVersion}  " +                                                // $"·{DotNetCoreVersionCmd()}·  " +  //todo: flickers CMD window; remove when ..ready.
      $"wai:{cfg?["WhereAmI"]}  " +
      $"{cmn}  " +
      $"arg:[{string.Join("|", Environment.GetCommandLineArgs().Skip(1))}]";        // $"cur:{Environment.CurrentDirectory}  ";

  public static string Env()
  {
    var ary = Assembly.GetExecutingAssembly().Location.Split('\\');
    return ary.Length > 2 ? ary[2] : ary.Length.ToString();
  }

  public static (bool isObsolete, DateTime curExeTime) CheckForNewVersion(string pathToSetupExe, int buildTimeOffsetInMin = 3)
  {
    var curExeTime = File.GetLastWriteTime(pathToSetupExe);
    var isObslete = (curExeTime - TimedVer).TotalMinutes > buildTimeOffsetInMin; // build takes 0.7-2.0 min.
    return (isObslete, curExeTime);
  }

  public static string CompileMode =>
    IsDbg ?
    (Debugger.IsAttached ? "·Dbg-Atchd" : "Dbg!!!") :
    (Debugger.IsAttached ? "·Rls-Atchd" : "");

  public static string TimeAgo(TimeSpan timespan, bool small = false, bool versionMode = false, string ago = "", string since = "") =>
    timespan < TimeSpan.Zero          /**/ ? "Never" :
    timespan.TotalMilliseconds < 1    /**/ ? $"{timespan.TotalMilliseconds * 1000:N0} {(small ? "mks" : "microseconds")}{ago}" :
    timespan.TotalMilliseconds < 10   /**/ ? $"{timespan.TotalMilliseconds:N2} {(small ? "ms" : "millseconds")}{ago}" :
    timespan.TotalMilliseconds < 100  /**/ ? $"{timespan.TotalMilliseconds:N0} {(small ? "ms" : "millseconds")}{ago}" :
    timespan.TotalSeconds < 1         /**/ ? $"{timespan.TotalSeconds:N2} {(small ? "sec" : "seconds")}{ago}" :
    timespan.TotalSeconds < 10        /**/ ? $"{timespan.TotalSeconds:N1} {(small ? "sec" : "seconds")}{ago}" :
    timespan.TotalSeconds < 100       /**/ ? $"{timespan.TotalSeconds:N0} {(small ? "sec" : "seconds")}{ago}" :
    timespan.TotalMinutes < 60        /**/ ? $"{timespan.TotalMinutes:N0} {(small ? "min" : "minutes")}{ago}" :
    timespan.TotalHours < 24          /**/ ? $"{timespan.TotalHours:N1} {(small ? "hr" : "hours")}{ago}" :
    timespan.TotalDays < 10           /**/ ? $"{timespan.TotalDays:N1} {(small ? "day" : "days")}{ago}" :
                                      /**/ (versionMode ? $"{since}{DateTime.Now - timespan:yyyy.M.d}" : $"{since}{DateTime.Now - timespan:yyyy-MM-dd}");

  public static DateTime TimedVer => new DateTime(2000, 1, 1).AddDays(CurVer.Build).AddSeconds(CurVer.Revision * 2); //AutoBuildVer: needs [assembly: AssemblyVersion("0.27.*")] in AssemblyInfo.cs and these 2 lines to CsProj:
                                                                                                                     //    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
                                                                                                                     //    <Deterministic>false</Deterministic>
                                                                                                                     //  </PropertyGroup>
  static string TimedVerStringTimeAgo => TimeAgo(DateTime.Now - new DateTime(2000, 1, 1).AddDays(CurVer.Build).AddSeconds(CurVer.Revision * 2), small: true, ago: " ago"); // based on [assembly: AssemblyVersion("0.8.*")] in AssemblyInfo.cs
  static string TimedVerStringFromPhysicalBinaries
  {
    get
    {
      var max = Math.Max(Math.Max(
        new FileInfo(Assembly.GetEntryAssembly()?.Location).LastWriteTime.ToOADate(),
        new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime.ToOADate()),
        new FileInfo(Assembly.GetCallingAssembly().Location).LastWriteTime.ToOADate());

      return TimeAgo(DateTime.Now - DateTime.FromOADate(max), ago: " ago");
    }
  }

  public static Version CurVer => Assembly.GetEntryAssembly()?.GetName()?.Version ?? new Version("No Kidding...");

  public static bool IsDbgAndRBD => IsDbg && IsRBD;
  public static bool IsRBD => Environment.UserName.StartsWith("api") || Environment.UserName.EndsWith("exp"); // ran by dev.
  public static bool IsDbg
  {
    get
    {
#if DEBUG
      return true;
#else
      return false;
#endif
    }
  }

  public static string DotNetCoreVersionCmd() // https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.useshellexecute?view=net-5.0
  {
    try
    {
      if (_dnv == null)
      {
        using var process = new Process
        {
          StartInfo = new ProcessStartInfo
          {
            FileName = "dotnet.exe",
            Arguments = "--version",
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            RedirectStandardOutput = true
          }
        };

        _ = process.Start();
        _dnv = process.StandardOutput.ReadToEnd().Replace("\r", "").Replace("\n", "");

        process.WaitForExit();
      }

      return _dnv;
    }
    catch (Exception ex) { return ex.Message; }
  }
  static string? _dnv = null;
}