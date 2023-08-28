namespace StandardLib.Consts;
public class ScrSvrPresets
{
  public static double MinToPcSleep =>
    DevOps.IsDbg ? .125: // 1 min for: testing, etc....     
    Environment.MachineName == "BEELINK1" ? 16 : // see the logs if 10 min limit kicks in here
    Environment.MachineName == "RAZER1" ? 56 :   // 1hr for: PerfectMind registration, etc....
    Environment.MachineName == "ASUS2" ? 16 :    // see the logs if 10 min limit kicks in here
    Environment.MachineName == "YOGA1" ? 16 :    // see the logs if 10 min limit kicks in here
    8; //todo: something closes the app exactly on 10 min mark?!?!?!?! => keep it under 10 > 9.5=8+1+.5
}