namespace StandardContractsLib;

public interface ISecForcer
{
  bool CanEdit { get; }
  bool CanRead { get; }
  string PermisssionCSV { get; }

  bool HasAccessTo(PermissionFlag ownedPermissions, PermissionFlag requiredPermission);
}

public interface ISecurityForcer // older
{
  bool CanEdit { get; }
  bool CanRead { get; }
  string PermisssionCSV { get; }

  bool HasAccessTo(PermissionFlag ownedPermissions, PermissionFlag requiredPermission);
}

[Flags]
public enum PermissionFlag
{
  None = 0,
  Read = 1,
  Edit = 2,
  Admin = 1024,
}