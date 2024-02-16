namespace StandardContractsLib;

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

public class GenderApiConst
{
  public static string[] Retries = [
    "[country[0] is null]", // 0
    "[country[0].country_name is null]", // 1
    "[root is null]",       // 2
    "_limit reached_",      // 3
    "[no idea]",            // 4
    "*no name*",            // 5
    "_NoCache_",            // 6
    "]*IsBadName*[",        // 7
    "limit reached."        // 8
  ];

  public static bool IsLimitReached(string? val) => val == Retries[3] || val == Retries[8];
}

