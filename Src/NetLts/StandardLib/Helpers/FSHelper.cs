namespace StandardLib.Helpers;

public static class FSHelper
{
  public static string GetCreateSafeLogFolderAndFile(string[] fullPaths)
  {
    foreach (var fp in fullPaths)
      if (ExistsOrCreated(Path.GetDirectoryName(fp) ?? throw new ArgumentNullException("▄▀")))
        return fp;

    return "__FallbackName__.log";
  }
  public static string GetCreateSafeLogFolderAndFile(string fullPath)
  {
    if (ExistsOrCreated(Path.GetDirectoryName(fullPath) ?? throw new ArgumentNullException("▄▀")))
      return fullPath;

    return "__FallbackName__.log";
  }
  public static bool ExistsOrCreated(string directory) // true if created or exists; false if unable to create.
  {
    try
    {
      if (Directory.Exists(directory))
        return true;

      _ = Directory.CreateDirectory(directory);

      return Directory.Exists(directory);
    }
    catch (IOException ex) { _ = ex.Log($"Directory.CreateDirectory({directory})"); }
    catch (Exception ex) { _ = ex.Log($"Directory.CreateDirectory({directory})"); throw; }

    return false;
  }
}
