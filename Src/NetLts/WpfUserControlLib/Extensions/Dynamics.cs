namespace WpfUserControlLib.Extensions;

public static class Dynamics
{
  public static DataTable ToDataTable(this IEnumerable<dynamic> dynamicRows)
  {
    var dataTable = new DataTable();

    try
    {
      if (dynamicRows.Any())
      {
        var drArray = dynamicRows.ToArray();

        foreach (var key in ((IDictionary<string, object>)drArray[0]).Keys)
          _ = dataTable.Columns.Add(key);

        foreach (var d in drArray)
          _ = dataTable.Rows.Add(((IDictionary<string, object>)d).Values.ToArray());
      }
    }
    catch (Exception ex)
    {
      if (VersionHelper.IsDbg)
        WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] Log + Ignore: {ex.InnerMessages()}. ");
      else
        _ = ex.Log($"Exception in ToDataTable()  --  Log + Ignore: {ex.InnerMessages()}.  --  Ignore and return an empty DataTable ... but log for future ideas!");
    }

    return dataTable;
  }
}