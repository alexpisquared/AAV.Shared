namespace EF.DbHelper.Lib;

public class SqlSpRuner
{
  public static async Task<int> ExecuteNonQueryAsync(DbConnection connection, string commandText, List<SqlParameter>? sqlParameters, ILogger? logger, CommandType commandType = CommandType.Text)
  {
    try
    {
      if (connection.State.Equals(ConnectionState.Closed))
        connection.Open();

      using var cmd = connection.CreateCommand();
      cmd.CommandText = commandText;
      cmd.CommandType = commandType;

      sqlParameters?.ForEach(p => cmd.Parameters.Add(p));

      return await cmd.ExecuteNonQueryAsync();
    }
    catch (Exception ex)
    {
      var msg = $"'{commandText}'    '{((SqlConnection)connection).ConnectionString}'";
      _ = ex.Log(msg);
      logger?.LogError(ex, msg);
      throw;
    }
  }
  public static async Task<object?> ExecuteScalarAsync(DbConnection connection, string commandText, List<SqlParameter>? sqlParameters, ILogger? logger)
  {
    try
    {
      if (connection.State.Equals(ConnectionState.Closed))
        connection.Open();

      using var cmd = connection.CreateCommand();
      cmd.CommandText = commandText;
      cmd.CommandType = sqlParameters is null ? CommandType.Text : CommandType.StoredProcedure;

      sqlParameters?.ForEach(p => cmd.Parameters.Add(p));

      return await cmd.ExecuteScalarAsync();
    }
    catch (SqlException ex)
    {
      logger?.LogError(ex, $"║    '{commandText}'    '{((SqlConnection)connection).ConnectionString}'      ex.Server: '{ex.Server}'(always empty)     [finder:^?E#@%$#&!]");
      throw;
    }
    catch (Exception ex)
    {
      logger?.LogError(ex, $"║    '{commandText}'    '{((SqlConnection)connection).ConnectionString}'      [finder:^?E#@%$#&!]");
      throw;
    }
  }
  public static async Task<IEnumerable<dynamic>> ExecuteReaderAsync(DbConnection connection, string commandText, List<SqlParameter>? sqlParameters, ILogger? logger)
  {
    List<dynamic>? dynamicList = new();

    try
    {
      if (connection.State.Equals(ConnectionState.Closed))
        connection.Open();

      using var cmd = connection.CreateCommand();
      cmd.CommandText = commandText;
      cmd.CommandType = sqlParameters is null ? CommandType.Text : CommandType.StoredProcedure;

      sqlParameters?.ForEach(p => cmd.Parameters.Add(p));

      using var reader = await cmd.ExecuteReaderAsync();
      if (reader.HasRows)
      {
        while (reader.Read())
          dynamicList.Add(SqlSpRuner.GetDynamicData(reader));
      }

      reader.Close();
    }
    catch (Exception ex)
    {
      var msg = $"'{commandText}'    '{((SqlConnection)connection).ConnectionString}'";
      _ = ex.Log(msg);
      logger?.LogError(ex, msg);
      throw;
    }

    return dynamicList;
  }
  public static void AddSqlParam(List<SqlParameter> sqlParamList, string[] param, string svalue)
  {
    sqlParamList.Add(new SqlParameter
    {
      ParameterName = param.First(),
      Value = svalue,
      SqlDbType = CalcDefaultValue(param).sqlDbType, // DbType = param[1] == "int" ? DbType.Int32 : DbType.String, <- seems redundant.
      Direction = param.Last() == "False" ? ParameterDirection.Input : ParameterDirection.Output // .InputOutput is problematic!
    });
  }
  public static (SqlDbType sqlDbType, string svalue) CalcDefaultValue(string[] param)
  {
    string svalue = "";
    if (!Enum.TryParse(param[1], ignoreCase: true, out SqlDbType sqlDbType) && !param[1].Equals("SymbolType", StringComparison.OrdinalIgnoreCase) && !param[1].Equals("CurrencyType", StringComparison.OrdinalIgnoreCase))
    {
      sqlDbType = SqlDbType.NVarChar;
    }
    else
    {
      if ((new[] {
        SqlDbType.Char,
        SqlDbType.NChar,
        SqlDbType.VarChar,
        SqlDbType.NVarChar
      }).Contains(sqlDbType) || param[1].Equals("SymbolType", StringComparison.OrdinalIgnoreCase) || param[1].Equals("CurrencyType", StringComparison.OrdinalIgnoreCase))
      {
        if (int.TryParse(param[2], out var len))
        {
          if (len > 256)
            len = 256;

          svalue = len <= 15 ? new string('*', len) : $"Max len:{param[2],4}  {new string('*', len - 15)}.";
        }
      }
      else if ((new[] {
        SqlDbType.Date,
        SqlDbType.DateTime,
        SqlDbType.DateTime2,
        SqlDbType.DateTimeOffset,
        SqlDbType.SmallDateTime
      }).Contains(sqlDbType))
      {
        svalue = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
      }
      else if (sqlDbType == SqlDbType.Int && param[0].Contains("Date", StringComparison.OrdinalIgnoreCase))
      {
        svalue = $"{DateTime.Today:yyyyMMdd}";
      }
    }

    return (sqlDbType, svalue);
  }
  static dynamic GetDynamicData(DbDataReader reader)
  {
    IDictionary<string, object?> expandoObject = new ExpandoObject(); // '/' - not tested ~~~~!!!
    for (var i = 0; i < reader.FieldCount; i++)
    {
      //WriteLine($"TrWL:> @# {i,3}  {reader.GetName(i),-22} - {reader.GetValue(i).GetType(),-18} - {reader.GetValue(i)} - {reader.GetValue(i)}");
      expandoObject.Add(reader.GetName(i), reader.GetValue(i));
    }

    return expandoObject;
  }
}