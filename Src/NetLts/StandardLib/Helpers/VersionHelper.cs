namespace StandardLib.Helpers;
public static class VersionHelper
{

  public static string CurVerStr => DateTime.Now - TimedVer < TimeSpan.FromHours(24) ? CurVerStrYYMMDDHHmm : CurVerStrYYMMDD;
  static string CurVerStrFmt(string fmt) => $"v{TimedVer.ToString(fmt)}  {CompileMode}";
  static string CurVerStrYYMMDD => $"v{TimedVer:yy.MM.dd}  {CompileMode}";
  static string CurVerStrYYMMDDHH => $"v{TimedVer:yy.MM.dd.HH}  {CompileMode}";
  static string CurVerStrYYMMDDHHmm => $"v{TimedVer:yy.MM.dd.HHmm}  {CompileMode}";
  static string CurVerStrYMd => $"v{TimedVer:y.M.d}  {CompileMode}";
  public static string DotNetCoreVersion { get { try { return Environment.Version.ToString(); } catch (Exception ex) { return ex.Message; } } } // Gets a System.Version object that describes the major, minor, build, and revision numbers of the CLR (common language runtime). ---sadly, for/of the app only.
  public static string DevDbgAudit(IConfigurationRoot cfg, string msg, [CallerMemberName] string? cmn = "") => $"{CurVerStr}  " +
      @$"{Environment.MachineName}.{Environment.UserDomainName}\{Environment.UserName}  " +
      //tmi: $"exe:{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}  " +  // $"log:{cfg?["LogFolder"]}  " +     //nogo: no point
      $".net:{DotNetCoreVersion}  " +                                                // $"·{DotNetCoreVersionCmd()}·  " +  //todo: flickers CMD window; remove when ..ready.
      $"wai:{cfg?["WhereAmI"]}  " +
      $"fus:{cfg?["FromUserSecretsOnly"]}  " +
      $"fac:{cfg?["FromAppSettingsOnly"]}  " +
      $"{msg}  " +
      $"{cmn}  " +
      $"arg:[{string.Join("|", Environment.GetCommandLineArgs().Skip(1))}]";        // $"cur:{Environment.CurrentDirectory}  ";

  public static string Env() { var ary = Assembly.GetExecutingAssembly().Location.Split('\\'); return ary.Length > 2 ? ary[2] : ary.Length.ToString(); } // ???

  public static (bool isObsolete, DateTime curExeTime) CheckForNewVersion(string pathToSetupExe, int buildTimeOffsetInMin = 3)
  {
    var curExeTime = File.GetLastWriteTime(pathToSetupExe);
    var isObslete = (curExeTime - TimedVer).TotalMinutes > buildTimeOffsetInMin; // build takes 0.7-2.0 min.
    return (isObslete, curExeTime);
  }

  public static string CompileMode => Debugger.IsAttached ? (IsDbg ? "·Dbg-Atchd" : "·Rls-Atchd") : (IsDbg ? "Dbg" : "Rls");

  public static string TimeAgo(TimeSpan timespan, bool small = true, bool versionMode = false, string ago = "", string since = "") =>
    timespan < TimeSpan.Zero          /**/ ? "Never" :
    timespan.TotalMilliseconds < 1    /**/ ? $"{timespan.TotalMicroseconds:N0} {(small ? "mks" : "microseconds")}{ago}" :
    timespan.TotalMilliseconds < 10   /**/ ? $"{timespan.TotalMilliseconds:N2} {(small ? "ms" : "milliseconds")}{ago}" :
    timespan.TotalMilliseconds < 100  /**/ ? $"{timespan.TotalMilliseconds:N0} {(small ? "ms" : "milliseconds")}{ago}" :
    timespan.TotalSeconds < 1         /**/ ? $"{timespan.TotalSeconds:N2} {(small ? "sec" : "seconds")}{ago}" :
    timespan.TotalSeconds < 10        /**/ ? $"{timespan.TotalSeconds:N1} {(small ? "sec" : "seconds")}{ago}" :
    timespan.TotalSeconds < 100       /**/ ? $"{timespan.TotalSeconds:N0} {(small ? "sec" : "seconds")}{ago}" :
    timespan.TotalMinutes < 10        /**/ ? $"{timespan.TotalMinutes:N1} {(small ? "min" : "minutes")}{ago}" :
    timespan.TotalMinutes < 60        /**/ ? $"{timespan.TotalMinutes:N0} {(small ? "min" : "minutes")}{ago}" :
    timespan.TotalHours < 24          /**/ ? $"{timespan.TotalHours:N1} {(small ? "hr" : "hours")}{ago}" :
    timespan.TotalDays < 10           /**/ ? $"{timespan.TotalDays:N1} {(small ? "day" : "days")}{ago}" :
                                      /**/ (versionMode ? $"{since}{DateTime.Now - timespan:yyyy.M.d}" : $"{since}{DateTime.Now - timespan:yyyy-MM-dd}");

  static string TimedVerStringTimeAgo => TimeAgo(DateTime.Now - new DateTime(2000, 1, 1).AddDays(CurVer.Build).AddSeconds(CurVer.Revision * 2), small: true, ago: " ago"); // based on [assembly: AssemblyVersion("0.8.*")] in AssemblyInfo.cs
  static string TimedVerStringFromPhysicalBinaries
  {
    get
    {
      var max = Math.Max(Math.Max(
        new FileInfo(Assembly.GetEntryAssembly()?.Location ?? throw new ArgumentNullException("▄▀")).LastWriteTime.ToOADate(),
        new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime.ToOADate()),
        new FileInfo(Assembly.GetCallingAssembly().Location).LastWriteTime.ToOADate());

      return TimeAgo(DateTime.Now - DateTime.FromOADate(max), ago: " ago");
    }
  }

  public static bool IsDbgOrRBD => IsDbg || IsRBD;
  public static bool IsRBD => Environment.UserName.EndsWith("lexp") || Environment.UserName.StartsWith("olepi"); // ran by dev.
  public static bool IsDbg =>
#if DEBUG
      true;
#else
      false;
#endif

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

  public static DateTime TimedVer => CurVer.Build == 0 ? GetLastBuildTime : new DateTime(2000, 1, 1).AddDays(CurVer.Build).AddSeconds(CurVer.Revision * 2); //AutoBuildVer: needs [assembly: AssemblyVersion("0.27.*")] in AssemblyInfo.cs and these 2 lines to CsProj:
                                                                                                                                                            //    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
                                                                                                                                                            //    <Deterministic>false</Deterministic>
                                                                                                                                                            //  </PropertyGroup>
  public static Version CurVer => Assembly.GetEntryAssembly()?.GetName()?.Version ?? new Version(1, 2, 3, 4); //tu: Auto Build Version: add '<GenerateAssemblyInfo>false</GenerateAssemblyInfo><Deterministic>false</Deterministic>' to CsProj ... plus this line to AsseblyInfo.cs:  [assembly: AssemblyVersion("0.0.*")]   
  public static DateTime GetLastBuildTime => new[] {
                new FileInfo(Assembly.GetEntryAssembly()?.Location!).LastWriteTime,             // the startup exe/dll
                new FileInfo(Assembly.GetCallingAssembly().Location).LastWriteTime,             // the passthrough caller
                new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime }.Max();   // this lib - StandardLib
}