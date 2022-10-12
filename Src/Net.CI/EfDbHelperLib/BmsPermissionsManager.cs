using DB.Inventory.Models;

namespace EF.DbHelper.Lib;

public class BmsPermissionsManager // a synch to BMS changes of native SQL perms effort.
{
  const string _ipmAppName = "Income Payment Management V2";
  readonly ILogger _lgr;
  readonly DB.Inventory.Models.InventoryContext _dbx;
  int? _ipmAppId = null;

  public BmsPermissionsManager(ILogger lgr, DB.Inventory.Models.InventoryContext dbx) => (_lgr, _dbx) = (lgr, dbx);

  public async Task<int> AlterRole(bool isAdd, string loginm, string dbrole)
  {
    var ipmAppId = await GetCreateIpmAppId();
    var ipmUsrId = await GetCreateIpmUsrId(loginm);
    var ipmPrmId = await GetCreateIpmPrmId(ipmAppId, dbrole);

    _ = isAdd ? await Assign(ipmUsrId, ipmPrmId) : await Revoke(ipmUsrId, ipmPrmId);

    var (_, rowsSavedCnt, report) = await _dbx.TrySaveReportAsync();
    _lgr.Log(LogLevel.Debug, $"DB Save of: {_ipmAppName}  {loginm}  {dbrole}  {(isAdd ? "Added" : "removed")}  ({report})");

    return rowsSavedCnt;
  }

  async Task<int> GetCreateIpmAppId()
  {
    if (_ipmAppId is not null)
      return _ipmAppId.Value;

    var ipmAppRow = await _dbx.Applications.FirstOrDefaultAsync(r => r.AppName == _ipmAppName);
    if (ipmAppRow is null)
    {
      ipmAppRow = (await _dbx.Applications.AddAsync(new DB.Inventory.Models.Application { AppId = 1 + await _dbx.Applications.MaxAsync(r => r.AppId), AppName = _ipmAppName })).Entity;
      var (_, _, report) = await _dbx.TrySaveReportAsync();
      _lgr.Log(LogLevel.Trace, $"New Appn added: {_ipmAppName}  ({report})");
    }

    _ipmAppId = ipmAppRow.AppId;

    return _ipmAppId.Value;
  }
  async Task<int> GetCreateIpmUsrId(string loginm)
  {
    var ipmUsrRow = await _dbx.Users.FirstOrDefaultAsync(r => r.UserId == loginm);
    if (ipmUsrRow is null)
    {
      ipmUsrRow = (await _dbx.Users.AddAsync(new DB.Inventory.Models.User { UserId = loginm, AdminAccess = 0, Type = "U", Status = "A" })).Entity;
      var (_, _, report) = await _dbx.TrySaveReportAsync();
      _lgr.Log(LogLevel.Trace, $"New user added: {loginm}  ({report})");
    }

    return ipmUsrRow.UserIntId;
  }
  async Task<int> GetCreateIpmPrmId(int ipmAppId, string dbrole)
  {
    var ipmPrmRow = await _dbx.Permissions.FirstOrDefaultAsync(r => r.AppId == ipmAppId && r.Name == dbrole);
    if (ipmPrmRow is null)
    {
      ipmPrmRow = (await _dbx.Permissions.AddAsync(new DB.Inventory.Models.Permission { AppId = ipmAppId, Name = dbrole })).Entity;
      var (_, _, report) = await _dbx.TrySaveReportAsync();
      _lgr.Log(LogLevel.Trace, $"GetCreateIpmPrmId: {ipmAppId}  {dbrole}  ({report})");
    }

    return ipmPrmRow.PermissionId;
  }
  async Task<long> Assign(int userId, int permId)
  {
    var ipmPAsRow = await _dbx.PermissionAssignments.FirstOrDefaultAsync(r => r.UserId == userId && r.PermissionId == permId);
    if (ipmPAsRow is null)
    {
      ipmPAsRow = (await _dbx.PermissionAssignments.AddAsync(new DB.Inventory.Models.PermissionAssignment { UserId = userId, PermissionId = permId, Status = "G" })).Entity;
      var (_, _, report) = await _dbx.TrySaveReportAsync();
      _lgr.Log(LogLevel.Trace, $"Assign: {userId}  {permId}  ({report})");
    }

    return ipmPAsRow.TblId;
  }
  async Task<long> Revoke(int userId, int permId)
  {
    var ipmPAsRow = await _dbx.PermissionAssignments.FirstOrDefaultAsync(r => r.UserId == userId && r.PermissionId == permId);
    if (ipmPAsRow is null)
      return 0;

    _ = _dbx.PermissionAssignments.Remove(ipmPAsRow);

    return ipmPAsRow.TblId;
  }
}
