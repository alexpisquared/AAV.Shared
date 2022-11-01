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
    DtBsNmLst = "DtBsNmLst",
    ServerLst = "ServerLst";
}

public class DeplConstDpl
{
  const string
    DeplName = "CI.DPL",
    DeplExe = DeplName + ".exe";
  public static string DeplSrcDir => @$"Z:\Dev\_Redis_MTDEV\{DeplName}\bin\Phase0";
  public static string DeplTrgDir => @$"C:\CIIS-Apps\DEV\{DeplName}";
  public static string DeplSrcExe => @$"{DeplSrcDir}\{DeplExe}";
  public static string DeplTrgExe => @$"{DeplTrgDir}\{DeplExe}";
}
public class DeplConstSpm
{
  const string
    DeplName = "CI.SPM",
    DeplExe = DeplName + ".App.exe";
  public static string DeplSrcDir => @$"Z:\Dev\_Redis_MTDEV\{DeplName}\bin\Phase0";
  public static string DeplTrgDir => @$"C:\CIIS-Apps\DEV\{DeplName}";
  public static string DeplSrcExe => @$"{DeplSrcDir}\{DeplExe}";
  public static string DeplTrgExe => @$"{DeplTrgDir}\{DeplExe}";
}
public class DeplConstIpm
{
  const string DeplName = "IncomePaymentManagement";
  const string DeplExe = "CI.IPM.exe";
  public static string IpmDevDbgUser => "IpmDevDbgUser";
  public static string IpmDevDbgOpen => "IpmDevDbgUser";

  public static string DeplSrcDir => @$"Z:\Dev\_Redis_MTDEV\BMS\{DeplName}\bin";
  public static string DeplTrgDir => @$"C:\CIIS-Apps\DEV\BMS\{DeplName}";
  public static string DeplSrcExe => @$"{DeplSrcDir}\{DeplExe}";
  public static string DeplTrgExe => @$"{DeplTrgDir}\{DeplExe}";
}