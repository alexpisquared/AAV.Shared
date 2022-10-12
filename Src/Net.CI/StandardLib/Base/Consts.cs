namespace StandardLib.Base;

public class Consts
{
#if DEBUG
  public static string SqlServerCSV => DevOps.IsDevMachineH ? @".\sqlexpress .\SQLEXPRESS" : "MtDEVSQLDB,1625 MtUATSQLDB MTPRDSQLDB";
#else
  public static string SqlServerCSV = "MTDEVSQLDB,1625 MTUATSQLDB MTPRDSQLDB";
#endif

  public const string ___ = "MTDEVSQLDB MTUATSQLDB MTPRDSQLDB";

  public static List<string> SqlServerList = SqlServerCSV.Split(' ').ToList();
}
