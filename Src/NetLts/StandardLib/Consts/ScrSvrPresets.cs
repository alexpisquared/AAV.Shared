namespace StandardLib.Consts;
public class ScrSvrPresets
{
  public static double MinToPcSleep =>
    DevOps.IsDbg ? 55 : //0.25 :  // for testing, etc....     
                        //= "BEELINK1" ? 16 :   // logs show 10 min limit kicks in here: "inf ╘══10.0 min OnExit"  ==> 8 min must be used
    Environment.MachineName == "NUC2" ? 26 :    // 
    Environment.MachineName == "YOGA1" ? 16 :   // see the logs if 10 min limit kicks in here
    Environment.MachineName == "ASUS2" ? 16 :   // see the logs if 10 min limit kicks in here
    Environment.MachineName == "RAZER1" ? 56 :  // 1hr for: PerfectMind registration, etc....
    Environment.MachineName == "MINISFORUM1" ? 56 + 60 + 60 + 60 + 60 + 60 + 60 + 60 :  // 8 HR!?!?!
    Environment.MachineName.Contains("33") || Environment.MachineName.Contains("P22") ? 14 :  // Mar 2025: not sure I need to sleep it here. Apr 3: better turn it off .. along with the monitor. Apr 4: ScrSvr puts it to some sort of deep sleep which takes forever to wake up from: let's keep it off for 3 hours at least.
    56 + 60 + 60 + 60 + 60 + 60; // Note: 5 hours is the max available from the system.
}