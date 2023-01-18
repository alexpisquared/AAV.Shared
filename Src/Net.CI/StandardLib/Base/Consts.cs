namespace StandardLib.Base;

public class Consts
{
#if DEBUG
  public static string SqlServerCSV => DevOps.IsDevMachineH ? """.\sqlexpress .\SQLEXPRESS""" : "MtDEVSQLDB,1625 MtUATSQLDB MTPRDSQLDB";
  public static string DtBsNameCSV => DevOps.IsDevMachineH ? "FinDemoDbg qstatsrls" : "FinDemoDbg QStatsRls";
#else
  public static string SqlServerCSV = "MTDEVSQLDB,1625 MTUATSQLDB MTPRDSQLDB";
  public static string DtBsNameCSV = "FinDemoDbg QStatsRls";
#endif

  public const string ___ = "MTDEVSQLDB MTUATSQLDB MTPRDSQLDB";

  public static List<string> SqlServerList = SqlServerCSV.Split(' ').ToList();
}
