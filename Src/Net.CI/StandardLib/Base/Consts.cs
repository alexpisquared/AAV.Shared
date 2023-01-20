namespace StandardLib.Base;

public class Consts
{
#if BackAtCI
  public static string SqlServerCSV => DevOps.IsDevMachineH ? """.\sqlexpress .\SQLEXPRESS""" : "MtDEVSQLDB,1625 MtUATSQLDB MTPRDSQLDB";
  public static string DtBsNameCSV  => DevOps.IsDevMachineH ? "QStatsDbg QStatsRls" : "QStatsDbg QStatsRls";
#else
  public static string SqlServerCSV = @".\sqlexpress .\SQLEXPRESS";
  public static string DtBsNameCSV = "QStatsDbg QStatsRLS";
#endif

  public static List<string> SqlServerList = SqlServerCSV.Split(' ').ToList();
  public static List<string> DtBsNameList = DtBsNameCSV.Split(' ').ToList();
}
