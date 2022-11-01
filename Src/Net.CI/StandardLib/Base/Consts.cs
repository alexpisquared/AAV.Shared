namespace StandardLib.Base;

public class Consts
{
#if DEBUG
  public static string SqlServerCSV => DevOps.IsDevMachineH ? @".\sqlexpress .\SQLEXPRESS" : "MtDEVSQLDB,1625 MtUATSQLDB MTPRDSQLDB";
  public static string DtBsNameCSV => DevOps.IsDevMachineH ? @"QSTATSDBG qstatsrls" : "QStatsDbg QStatsRls";
#else
  public static string SqlServerCSV = "MTDEVSQLDB,1625 MTUATSQLDB MTPRDSQLDB";
  public static string DtBsNameCSV = "QStatsDbg QStatsRls";
#endif

  public const string ___ = "MTDEVSQLDB MTUATSQLDB MTPRDSQLDB";

  public static List<string> SqlServerList = SqlServerCSV.Split(' ').ToList();
}
