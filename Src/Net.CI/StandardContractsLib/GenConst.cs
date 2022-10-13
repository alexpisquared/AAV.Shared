namespace StandardContractsLib;
public class Deployment
{
  const string
    DeplName = "CI.SPM",
    DeplExe = DeplName + ".App.exe";
  public static string DeplSrcDir => @$"Z:\Dev\_Redis_MTDEV\{DeplName}\bin\Phase0";
  public static string DeplTrgDir => @$"C:\CIIS-Apps\DEV\{DeplName}";
  public static string DeplSrcExe => @$"{DeplSrcDir}\{DeplExe}";
  public static string DeplTrgExe => @$"{DeplTrgDir}\{DeplExe}";
}

public class CfgName
{
  public const string
    SqlVerIpm = "SqlConStrFormat", // limited by IpmRole access: to fully impersonate IpmRole-defined access.                                   
    SqlVerIp_ = "SqlConStrForma_", // limited by IpmRole access: to fully impersonate IpmRole-defined access.                                   
    WhereAmAy = "WhereAmI",
    LogFolder = "LogFolder",
    ServerLst = "ServerLst";
}