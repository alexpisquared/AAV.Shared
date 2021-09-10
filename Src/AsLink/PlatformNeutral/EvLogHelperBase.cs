using AAV.Sys.Ext;
using System;
using System.Diagnostics;
using System.Security.Principal;

namespace AsLink
{
  public static partial class EvLogHelper //this is the latest copy as of Sep 9, 2021. 
  {
    const string _app = "Application", _sec = "Security", _sys = "System", _aavSource = "AavSource", _aavLogName = "AavNewLog";
    public static string CheckCreateLogChannel(string src = _aavSource, string log = _aavLogName) => !IsAdministrator() ? "Restart as Admin!!!  (...to query/create event log)" : safeCreateEventSource(src, log);
    public static bool IsAdministrator()
    {
      var identity = WindowsIdentity.GetCurrent();
      if (identity != null)
      {
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
      }

      return false;
    }
    public static void LogScrSvrBgn(int App_Ssto_Gp, string msg = "ScrSvr - Up. ScreenSaveTimeOut + GracePeriod 1 min") { try { EventLog.WriteEntry(_aavSource, $"{msg}(sec) ={App_Ssto_Gp}", EventLogEntryType.Information, 7101); } catch (Exception ex) { ex.Log(); } }// AavSource finds the log named AavNewLog    
    public static void LogScrSvrEnd(DateTime actualIdleIeStartApp, string msg) { try { EventLog.WriteEntry(_aavSource, $"Idle since {actualIdleIeStartApp:HH:mm:ss} (=App.Start) for {DateTime.Now - actualIdleIeStartApp:h\\:mm\\:ss}    {msg}", EventLogEntryType.Information, 7102); } catch (Exception ex) { ex.Log(); } }// AavSource finds the log named AavNewLog

    static string safeCreateEventSource(string src, string log)
    {
      if (!EventLog.SourceExists(src))
      {
        EventLog.CreateEventSource(new EventSourceCreationData(src, log)); // replacing: EventLog.CreateEventSource(src, log, Environment.MachineName);
        return "Created the log";
      }

      return $"{src}\\{log} already exists";
    }
  }
}