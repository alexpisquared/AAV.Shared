namespace StandardLib.Base;

public class UserSettingsShared : UserSettingsBase
{
  public string? PrefSqlServer { get; set; }
  public string? NewAcntFilter_ { get; set; }
  public string? OldAcntFilter_ { get; set; }
  public string? RifLifXlsFile_ { get; set; }
  public bool AutoGenMode_ { get; set; }
  public bool IsAudibleIpm_ { get; set; } = false;
  public bool IsAnimeOnIpm_ { get; set; } = false;
  public string? LogFolderFile { get; set; }
  public string? LogErrDirFile { get; set; }
  public string? ExportFolder_ { get; set; }
  public int YearOfInt { get; set; } = DateTime.Today.Year - 1;
}
