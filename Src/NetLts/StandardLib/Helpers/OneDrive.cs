namespace StandardLib.Helpers;

public static class OneDrive // Core 3
{
  public static string Root
  {
    get
    {
      var rv = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "OneDrive");
      return (Environment.MachineName.Equals("ASUS2") && Environment.UserName.Contains("lex")) ? rv.Replace("C:", "D:") : rv; // :'cause Asus2 is on D:
    }
  }

  public static string Folder(string subFolder) => Path.Combine(Root, subFolder);
  public static string DataFolder(string subFolder, bool createIfNotExists = true)
  {
    var folder = Path.Combine(Root, """Public\AppData\""", subFolder);
    if (createIfNotExists)
      _ = FSHelper.ExistsOrCreated(folder);
    return folder;
  }

  public static string VpdbFolder => DataFolder("vpdb");
  public static string WebCacheFolder => """C:\temp\web.cache""";// DataFolder("web.cache");
}
