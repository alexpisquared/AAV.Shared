namespace EF.DbHelper.Lib;

public class SqlPermissionsManager
{
  const string _add = "ADD", _drp = "DROP";
  readonly ILogger _lgr;

  public SqlPermissionsManager(ILogger lgr) => _lgr = lgr;

  public async Task<bool> DoesUserHaveDbLogin(string loginm, DbConnection connection) => (await SqlSpRuner.ExecuteScalarAsync(connection, @$"SELECT 1 FROM sys.server_principals WHERE name = '{loginm}'", null, _lgr)) is not null;
  public async Task<bool> IsUserMemberOfRole(string loginm, string dbrole, DbConnection connection) => (await SqlSpRuner.ExecuteScalarAsync(connection,
    $"SELECT 1 " +
    $"FROM sys.database_role_members AS DRM RIGHT OUTER JOIN sys.database_principals AS DP1 ON DRM.role_principal_id = DP1.principal_id LEFT OUTER JOIN sys.database_principals AS DP2 ON DRM.member_principal_id = DP2.principal_id " +
    $"WHERE (DP1.type = 'R') AND (DP2.name = '{loginm}') AND (DP1.name = '{dbrole}') ", null, _lgr))
    is not null;
  public async Task<int> CreateSqlLoginAndDbUser(string loginm, DbConnection connection)
  {
    var dbName = connection.ConnectionString?.Split(";")?.FirstOrDefault(r => r.StartsWith("database", StringComparison.OrdinalIgnoreCase))?.Split("=")[1] ?? "Inventory";

    var a = await SqlSpRuner.ExecuteNonQueryAsync(connection, $"CREATE LOGIN [{loginm}] FROM WINDOWS WITH DEFAULT_DATABASE=[{dbName}] ", null, _lgr); //note: only  UPDATE, INSERT, and DELETE return 0++ rows affected; the rest - -1.
    var b = await SqlSpRuner.ExecuteNonQueryAsync(connection, $"CREATE USER  [{loginm}] FOR LOGIN[{loginm}] WITH DEFAULT_SCHEMA=[dbo] ", null, _lgr); //presuming convention: db user name == sql server login name.
    return a + b;
  }
  public async Task<int> DropDbUserAndSqlLogin(string loginm, DbConnection connection)
  {
    var a = await SqlSpRuner.ExecuteNonQueryAsync(connection, $"DROP USER  [{loginm}]", null, _lgr); //note: only  UPDATE, INSERT, and DELETE return 0++ rows affected; the rest - -1.
    var b = await SqlSpRuner.ExecuteNonQueryAsync(connection, $"DROP LOGIN [{loginm}]", null, _lgr); //note: only  UPDATE, INSERT, and DELETE return 0++ rows affected; the rest - -1.
    return a + b;
  }
  async Task<int> AlterRoleSafe(bool isAdd, string loginm, string dbrole, DbConnection connection)
  {
    await MakeSureExistsDbRole(dbrole, connection);
    await MakeSureExistsSLogin(loginm, connection);
    await MakeSureExistsDbUser(loginm, connection);
    return await AlterRole(isAdd, loginm, dbrole, connection);
  }
  public async Task<int> AlterRole(bool isAdd, string loginm, string dbrole, DbConnection connection) => await SqlSpRuner.ExecuteNonQueryAsync(connection, $"ALTER ROLE [{dbrole}] {(isAdd ? _add : _drp)} MEMBER [{loginm}]", null, _lgr); //note: only  UPDATE, INSERT, and DELETE return 0++ rows affected; the rest - -1.
  public async Task<int> AlterRolE(bool isAdd, string loginm, string dbrole, DbConnection connection) => await SqlSpRuner.ExecuteNonQueryAsync(connection, $"ALTER ROLE [@Role00] {(isAdd ? _add : _drp)} MEMBER [@User00]", new[] { new SqlParameter("@Role00", dbrole), new SqlParameter("@User00", loginm) }.ToList(), _lgr); //note: only  UPDATE, INSERT, and DELETE return 0++ rows affected; the rest - -1.
  public async Task<List<string>> GetSvrLgns(DbConnection connection) => await GetStringList(connection, "SELECT name FROM sys.server_principals  WHERE (is_disabled = 0) AND (type NOT IN ('G', 'R', 'C')) AND (name NOT LIKE 'NT %') ORDER BY name");
  public async Task<List<string>> GetDbRoles(DbConnection connection) => await GetStringList(connection, "SELECT name FROM sys.sysusers           WHERE (issqlrole = 1) AND (gid < 16384) AND (gid <> 0)  ORDER BY name");
  public async Task<List<string>> GetDbUsers(DbConnection connection) => await GetStringList(connection, "SELECT name FROM sys.sysusers           WHERE (issqlrole = 0) ORDER BY name");
  public async Task<List<string>> GetSqlDtBs(DbConnection connection) => await GetStringList(connection, "SELECT name FROM sys.databases          WHERE (database_id > 4)  ORDER BY name");
  public async Task<List<string>> GetUsersForRole(DbConnection connection, string dbrole) => await GetStringList(connection,
      $"SELECT DP2.principal_id, ISNULL(DP2.name, 'No members') AS name " +
      $"FROM sys.database_role_members AS DRM RIGHT OUTER JOIN sys.database_principals AS DP1 ON DRM.role_principal_id = DP1.principal_id LEFT OUTER JOIN sys.database_principals AS DP2 ON DRM.member_principal_id = DP2.principal_id " +
      $"WHERE (DP1.type = 'R') AND (DP1.name = '{dbrole}') ORDER BY name");

  async Task<List<string>> GetStringList(DbConnection connection, string sql)
  {
    var stringList = new List<string>();
    (await SqlSpRuner.ExecuteReaderAsync(connection, sql, null, _lgr)).ToList().ForEach(database => stringList.Add(database.name));
    return stringList;
  }

  public async Task<int> AddUserToRole(string loginm, string dbrole, DbConnection connection) => await AlterRoleSafe(true, loginm, dbrole, connection);
  public async Task<int> RmvUserFrRole(string loginm, string dbrole, DbConnection connection) => await AlterRoleSafe(false, loginm, dbrole, connection);

  public async Task MakeSureExistsSLogin(string loginm, DbConnection connection)
  {
    if (await DoesUserHaveDbLogin(loginm, connection)) return;

    var dbName = connection.ConnectionString?.Split(";")?.FirstOrDefault(r => r.StartsWith("database", StringComparison.OrdinalIgnoreCase))?.Split("=")[1] ?? "Inventory";

    _ = await SqlSpRuner.ExecuteNonQueryAsync(connection, $"CREATE LOGIN [{loginm}] FROM WINDOWS WITH DEFAULT_DATABASE=[{dbName}] ", null, _lgr); //note: only  UPDATE, INSERT, and DELETE return 0++ rows affected; the rest - -1.
  }
  public async Task MakeSureExistsDbRole(string dbrole, DbConnection connection)
  {
    if ((await GetDbRoles(connection)).Any(r => r.Equals(dbrole, StringComparison.OrdinalIgnoreCase))) return;

    _ = await SqlSpRuner.ExecuteNonQueryAsync(connection, $"CREATE ROLE [{dbrole}] AUTHORIZATION [dbo]", null, _lgr);
  }
  public async Task MakeSureExistsDbUser(string dbUser, DbConnection connection)
  {
    if ((await GetDbUsers(connection)).Any(r => r.Equals(dbUser, StringComparison.OrdinalIgnoreCase))) return;

    _ = await SqlSpRuner.ExecuteNonQueryAsync(connection, $"CREATE USER [{dbUser}] FOR LOGIN [{dbUser}] WITH DEFAULT_SCHEMA=[dbo]", null, _lgr); //presuming convention: db user name == sql server login name.
  }
}