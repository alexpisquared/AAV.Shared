using System.Drawing; // keep here.

namespace WpfUserControlLib.Services;

public class GuiCapture
{
  readonly ILogger lgr;

  public GuiCapture(ILogger lgr) => this.lgr = lgr;

  public void StoreActiveWindowScreenshotToFile(string shortNote, bool isNew = false)
  {
    try
    {
      var file = LongName(shortNote);

      _ = FSHelper.ExistsOrCreated(Path.GetDirectoryName(file)!);

      using var bmp = CaptureActiveWindow();

      bmp.Save(file, System.Drawing.Imaging.ImageFormat.Jpeg);
    }
    catch (Exception ex) { lgr.LogError(ex, $"■175"); }
  }

  public Bitmap CaptureActiveWindow(bool isNew = false) => isNew ? CaptureWindowOffScreen(WinAPI.GetForegroundWindow()) : CaptureWindow(WinAPI.GetForegroundWindow());
  public Bitmap CaptureWholeDesktop(bool isNew = false) => isNew ? CaptureWindowOffScreen(WinAPI.GetDesktopWindow()) : CaptureWindow(WinAPI.GetDesktopWindow());

  public Bitmap CaptureWindow(IntPtr handle)
  {
    try
    {
      var (bitmap, bounds) = PrepareSizedBitmap(handle);

      using var graphics = Graphics.FromImage(bitmap);
      graphics.CopyFromScreen(new System.Drawing.Point(bounds.Left, bounds.Top), System.Drawing.Point.Empty, bounds.Size);

      return bitmap;
    }
    catch (Exception ex) { lgr.LogError(ex, $"■188"); throw; }
  }
  public Bitmap CaptureWindowOffScreen(IntPtr handle)
  {
    var (bitmap, _) = PrepareSizedBitmap(handle);

    using var graphics = Graphics.FromImage(bitmap);
    var hdc = graphics.GetHdc();
    try { _ = WinAPI.PrintWindow(handle, hdc, 1); } // captures a window bitmap even if the window is covered by other windows or if it is off-screen. 
    finally { graphics.ReleaseHdc(hdc); }

    return bitmap;
  }

  static (Bitmap bitmap, Rectangle bounds) PrepareSizedBitmap(IntPtr handle)
  {
    var rect = new WinAPI.Rect();
    _ = WinAPI.GetWindowRect(handle, ref rect);
    var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
    var bitmap = new Bitmap(bounds.Width, bounds.Height);
    return (bitmap, bounds);
  }
  static string LongName(string shortNote)
  {
    const int maxLen = 52;
    var shorterNote = shortNote.Length > maxLen ? $"{shortNote[..maxLen]}...{shortNote[maxLen..]}" : shortNote;

    var file = $"{OneDrive.Folder(@"Public\Logs.Viz\")}{Assembly.GetEntryAssembly()?.GetName().Name ?? "NUL"}-{Environment.MachineName[..3]}-{Environment.UserName[..3]}-{DateTime.Now:HHmmss.ffff}-{string.Concat(shorterNote.Split(Path.GetInvalidFileNameChars()))}.jpg";
    return file;
  }
}
public partial class WinAPI
{
  [DllImport("user32.dll", SetLastError = true)] public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint nFlags);
  //[DllImport("user32.dll", CharSet = CharSet.Auto)] public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint nFlags);
  //[LibraryImport("user32.dll")] public static partial bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint nFlags);

  [LibraryImport("user32.dll")] public static partial IntPtr GetForegroundWindow();
  [LibraryImport("user32.dll")] public static partial IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
  [LibraryImport("user32.dll")] public static partial IntPtr GetDesktopWindow();
  [StructLayout(LayoutKind.Sequential)] public struct Rect { public int Left; public int Top; public int Right; public int Bottom; }
}