namespace StandardLib.Consts;
public class ScrSvrPresets
{
  public static double MinToPcSleep =>
    DevOps.IsDbg ? 0.25 :  // for testing, etc....     
                           //= "BEELINK1" ? 16 : // logs show 10 min limit kicks in here: "inf ╘══10.0 min OnExit"  ==> 8 min must be used
    Environment.MachineName == "RAZER1" ? 56 :   // 1hr for: PerfectMind registration, etc....
    Environment.MachineName == "ASUS2" ? 16 :    // see the logs if 10 min limit kicks in here
    Environment.MachineName == "YOGA1" ? 16 :    // see the logs if 10 min limit kicks in here
    8.5; // something closes the app exactly on 10 min mark: "inf ╘══10.0 min OnExit"  ==> keep it under 10 > 9.5=8.5+1+.5  
}