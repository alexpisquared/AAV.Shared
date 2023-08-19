using System.Drawing; // keep here.

namespace WpfUserControlLib.Services;

public partial class GuiCapture
{
  readonly ILogger lgr;

  public GuiCapture(ILogger lgr) => this.lgr = lgr;

  public Bitmap StoreActiveWindowScreenshotToFile(string shortNote)
  {
    var bmp = CaptureActiveWindow();

    try
    {
      const int maxLen = 52;
      var shortNote2 = shortNote.Length > maxLen ? shortNote[..maxLen] : shortNote;

      const string frm = $"{{0}}{{1}}-{{2}}-{{3:MMdd·HHmm}}-{{4}}.jpg";
      var pfn = string.Format(frm, OneDrive.Folder("""Public\Logs.Viz\"""), Assembly.GetEntryAssembly()?.GetName().Name ?? "Unkn", Environment.UserName[..3], DateTime.Now, string.Concat(shortNote2.Split(Path.GetInvalidFileNameChars())));

      _ = FSHelper.ExistsOrCreated(Path.GetDirectoryName(pfn)!);

      bmp.Save(pfn, System.Drawing.Imaging.ImageFormat.Jpeg);
    }
    catch (Exception ex) { lgr.LogError(ex, $"■175"); }

    return bmp;
  }
  public Bitmap CaptureActiveWindow() => CaptureWindow(GetForegroundWindow());
  public Bitmap CaptureWholeDesktop() => CaptureWindow(GetDesktopWindow());
  public Bitmap CaptureWindow(IntPtr handle)
  {
    try
    {
      var rect = new Rect();
      _ = GetWindowRect(handle, ref rect);
      var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
      var result = new Bitmap(bounds.Width, bounds.Height);

      using var graphics = Graphics.FromImage(result);
      graphics.CopyFromScreen(new System.Drawing.Point(bounds.Left, bounds.Top), System.Drawing.Point.Empty, bounds.Size);

      return result;
    }
    catch (Exception ex) { lgr.LogError(ex, $"■188"); throw; }
  }

  [LibraryImport("user32.dll")] private static partial IntPtr GetForegroundWindow();
  [LibraryImport("user32.dll")] private static partial IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
  [LibraryImport("user32.dll")] public static partial IntPtr GetDesktopWindow();
  [StructLayout(LayoutKind.Sequential)] struct Rect { public int Left; public int Top; public int Right; public int Bottom; }
}