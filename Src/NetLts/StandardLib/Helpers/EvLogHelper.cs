using System.Security.Principal;
using StandardLib.Extensions;

namespace AsLink;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class EvLogHelperBase //this is the latest copy as of 2023-08-18. 
{
protected  const string _app = "Application", _sec = "Security", _sys = "System", _aavSource = "AavSource", _aavLogName = "AavNewLog";

  public  void LogScrSvrBgn(int App_Ssto_Gp, string msg = "ScrSvr - Up. ScreenSaveTimeOut + GracePeriod 1 min") { try { EventLog.WriteEntry(_aavSource, $"{msg}(sec) ={App_Ssto_Gp}", EventLogEntryType.Information, 7101); } catch (Exception ex) { _ = ex.Log(); } }// AavSource finds the log named AavNewLog    
  public  void LogScrSvrEnd(DateTime actualIdleIeStartApp, string msg = "------------------------------------") { try { EventLog.WriteEntry(_aavSource, $"Idle since {actualIdleIeStartApp:HH:mm:ss} (=App.Start) for {DateTime.Now - actualIdleIeStartApp:h\\:mm\\:ss}    {msg}", EventLogEntryType.Information, 7102); } catch (Exception ex) { _ = ex.Log(); } }// AavSource finds the log named AavNewLog

  public  bool IsAdministrator() => WindowsIdentity.GetCurrent() != null && new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
  public  string CheckCreateLogChannel(string src = _aavSource, string log = _aavLogName) => !IsAdministrator() ? "Restart as Admin!!!  (...to query/create event log)" : safeCreateEventSource(src, log);
   string safeCreateEventSource(string src, string log)
  {
    if (EventLog.SourceExists(src))
      return $"{src}\\{log} already exists";

    EventLog.CreateEventSource(new EventSourceCreationData(src, log));
    return "Created the log";
  }
}