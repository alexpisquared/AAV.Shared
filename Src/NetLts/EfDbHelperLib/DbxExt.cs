namespace EF.DbHelper.Lib; // org standalone AsLink for \c\...  for \g\ use C:\g\OneDriveAudit\Src\ODA\AAV.Db.EF\DbContextExt.cs   (2020-12) ...(2021-10)
public static class DbqExt // replacing DbSaveLib and all others!!! (Aug 2018  ...2021-10  ...2022-05)
{
  public static async Task<(bool success, int rowsSavedCnt, string report)> TrySaveReportAsync(this DbContext dbq, string? info = "", [CallerMemberName] string cmn = "")
  {
    var report = $"{info} {cmn} -> {nameof(TrySaveReportAsync)} on {dbq.GetType().Name}.  Rows saved: ";

    try
    {
      var stopwatch = Stopwatch.StartNew();
      var rowsSaved = await dbq.SaveChangesAsync();

      report += stopwatch.ElapsedMilliseconds < 250 ? $"{rowsSaved:N0}. " : $"{rowsSaved:N0} / {VersionHelper.TimeAgo(stopwatch.Elapsed, small: true)} => {rowsSaved / stopwatch.Elapsed.TotalSeconds:N0} rps. ";

      WriteLine(report);

      return (true, rowsSaved, report);
    }
    catch (DbEntityValidationException ex)                          /**/ { report += ex.Log($"\n{ValidationExceptionToString(ex)}"); }
    catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)      /**/ { report += ex.Log($"\n{string.Join("\t", ex.Entries.Select(r => r.ToString()))}  [{ex.Entries.Count,5} rows affected]  :Core"); }
    catch (System.Data.Entity.Infrastructure.DbUpdateException ex)  /**/ { report += ex.Log($"\n{string.Join("\t", ex.Entries.Select(r => r.ToString()))}  [{ex.Entries.Count()} rows affected]  :Infr"); }
    catch (Exception ex)                                            /**/ { report += ex.Log(); }

    return (false, -88, report);
  }
  public static void DiscardChanges(this DbContext db) => db.ChangeTracker.Clear();
  public static bool HasUnsavedChanges(this DbContext db) => db != null && db.ChangeTracker.Entries().Any(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);
  public static string GetDbChangesReport(this DbContext db, int maxLinesToShow = 33)
  {
    var sb = new StringBuilder($"{db.GetType().Name}:  {db.ChangeTracker.Entries().Count(e => e.State == EntityState.Deleted),5} Del  {db.ChangeTracker.Entries().Count(e => e.State == EntityState.Added),5} Ins  {db.ChangeTracker.Entries().Count(e => e.State == EntityState.Modified),5} Upd");

    var lineCounter = 0;
    foreach (var modifieds in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
    {
      foreach (var pn in modifieds.CurrentValues.Properties)
      {
        var org = modifieds.OriginalValues[pn];
        var cur = modifieds.CurrentValues[pn];

        if (!Equals(modifieds.CurrentValues[pn], org))
        {
          if (++lineCounter <= maxLinesToShow)
          {
            _ = sb.Append($"\n{pn?.ToString()?.Replace("Property: ", ""),-17}  \t{SafeValue(org)} → {SafeValue(cur)}");
          }
          else
          {
            _ = sb.Append(" ...");
            break;
          }
        }
      }

      if (lineCounter > maxLinesToShow) break;
    }

    return sb.ToString();
  }

  public static string ValidationExceptionToString(this DbEntityValidationException ex)
  {
    var sb = new StringBuilder();

    foreach (var eve in ex.EntityValidationErrors)
    {
      _ = sb.AppendLine($"""- Entity of type "{eve.Entry.Entity.GetType().FullName}" in state "{eve.Entry.State}" has the following validation errors:""");
      foreach (var ve in eve.ValidationErrors)
      {
        object value;
        if (ve.PropertyName.Contains('.'))
        {
          var propertyChain = ve.PropertyName.Split('.');
          var complexProperty = eve.Entry.CurrentValues.GetValue<DbPropertyValues>(propertyChain.First());
          value = GetComplexPropertyValue(complexProperty, propertyChain.Skip(1).ToArray());
        }
        else
        {
          value = eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName);
        }

        _ = sb.AppendLine($"-- Property: \"{ve.PropertyName}\", Value: \"{value}\", Error: \"{ve.ErrorMessage}\"");
      }
    }

    const int maxCombinedErrorMessageLength = 4000;
    return sb.Length < maxCombinedErrorMessageLength ? sb.ToString() : (sb.ToString()[..maxCombinedErrorMessageLength] + " ...");
  }
  public static string ServerDatabase(this DbContext dbq)
  {
    var constr = dbq.Database.GetConnectionString() ?? "";
    var kvpLst = constr.Split(';').ToList();
    return
      GetConStrValue(kvpLst, "data source") ??
      GetConStrValue(kvpLst, "server") ??
      $"No server name found in the con. string  {constr} :(";
    //return $"{(server.Equals(@"(localdb)\MSSQLLocalDB", StringComparison.OrdinalIgnoreCase) ? "" : server.Contains("database.windows.net") ? "Azure\\" : server)}{getConStrValue(kvpList, "AttachDbFilename")}{getConStrValue(kvpList, "initial catalog")}";
  }
  public static string SqlConStrValues(this string constr, int firstN = 10) => string.Join("·", constr.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList().Take(firstN).Select(r => r.Split('=').LastOrDefault() ?? "°"));

  static string? GetConStrValue(List<string> lst, string key) => lst.FirstOrDefault(r => r.Split('=')[0].Equals(key, StringComparison.OrdinalIgnoreCase))?.Split('=')[1];
  static object GetComplexPropertyValue(DbPropertyValues propertyValues, string[] propertyChain)
  {
    var propertyName = propertyChain.First();

    return propertyChain.Length == 1
        ? propertyValues[propertyName]
        : GetComplexPropertyValue((DbPropertyValues)propertyValues[propertyName], propertyChain.Skip(1).ToArray());
  }
  static string SafeValue(object? val) => val is string str ?
    str.Length <= _maxWidth ? str : $"\r\n  {str[.._maxWidth].Replace("\n", " ").Replace("\r", " ")}...{str.Length:N0}\r\n" :
    val?.ToString() ?? "Null";

  const int _maxWidth = 42;
}