using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AAV.Sys.Helpers;

public static class SqlConStrHelper
{
  public enum DbLocation { Azure, Local, DbIns };

  public static string? DbNameFind(string conStr) => conStr?.Split(';').ToList().FirstOrDefault(r => r.Split('=')[0].Equals("initial catalog"
    , StringComparison.OrdinalIgnoreCase
    ) || r.Split('=')[0].Equals("AttachDbFilename"
      , StringComparison.OrdinalIgnoreCase
      ))?.Split('=')[1];

  public static string ConStr(string dbName, string dbgRLs, DbLocation dbLocation, string username, string password)
  {
    var conStr = "°°°";
    try
    {
      switch (dbLocation)
      {
        case DbLocation.DbIns: return conStr = $@"data source=.\sqlexpress;initial catalog={dbName}{dbgRLs}; integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
        case DbLocation.Azure: return conStr = $"data source=sqs.database.windows.net;initial catalog={dbName}{dbgRLs};persist security info=True;user id={username};password=\"{password}\";MultipleActiveResultSets=True;App=EntityFramework";
        case DbLocation.Local: var dbpathfilename = OneDrive.Folder($@"Public\AppData\TimeTrack\{dbName}{dbgRLs}.mdf"); Debug.Assert(File.Exists(dbpathfilename)); return conStr = $@"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename={dbpathfilename};Integrated Security=True;Connect Timeout=10;";
        default: throw new ArgumentOutOfRangeException(dbLocation.GetType().Name);
      }
    }
    finally { Trace.WriteLine($">> {conStr}"); }
  }
}