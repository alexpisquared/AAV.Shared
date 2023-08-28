using System.Drawing; // keep here.

namespace WpfUserControlLib.Services;

public class GuiCapture
{
  readonly ILogger lgr;

  public GuiCapture(ILogger lgr) => this.lgr = lgr;

  public void StoreActiveWindowScreenshotToFile(string shortNote)
  {
    try
    {
      const int maxLen = 52;
      var shorterNote = shortNote.Length > maxLen ? shortNote[..maxLen] : shortNote;

      var pfn = $"{OneDrive.Folder(@"Public\Logs.Viz\")}{Assembly.GetEntryAssembly()?.GetName().Name ?? "NUL"}-{Environment.MachineName[..3]}-{Environment.UserName[..3]}-{DateTime.Now:HHmmss.ffff}-{string.Concat(shorterNote.Split(Path.GetInvalidFileNameChars()))}.jpg";
        
      _ = FSHelper.ExistsOrCreated(Path.GetDirectoryName(pfn)!);

      using var bmp = CaptureActiveWindow();
      bmp.Save(pfn, System.Drawing.Imaging.ImageFormat.Jpeg);
    }
    catch (Exception ex) { lgr.LogError(ex, $"■175"); }
  }
  public Bitmap CaptureActiveWindow() => CaptureWindow(GuiCaptureAPI.GetForegroundWindow());
  public Bitmap CaptureWholeDesktop() => CaptureWindow(GuiCaptureAPI.GetDesktopWindow());
  public Bitmap CaptureWindow(IntPtr handle)
  {
    try
    {
      var rect = new GuiCaptureAPI.Rect();
      _ = GuiCaptureAPI.GetWindowRect(handle, ref rect);
      var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
      var result = new Bitmap(bounds.Width, bounds.Height);

      using var graphics = Graphics.FromImage(result);
      graphics.CopyFromScreen(new System.Drawing.Point(bounds.Left, bounds.Top), System.Drawing.Point.Empty, bounds.Size);

      return result;
    }
    catch (Exception ex) { lgr.LogError(ex, $"■188"); throw; }
  }
}
public partial class GuiCaptureAPI
{
  [LibraryImport("user32.dll")] public static partial IntPtr GetForegroundWindow();
  [LibraryImport("user32.dll")] public static partial IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
  [LibraryImport("user32.dll")] public static partial IntPtr GetDesktopWindow();
  [StructLayout(LayoutKind.Sequential)] public struct Rect { public int Left; public int Top; public int Right; public int Bottom; }
}