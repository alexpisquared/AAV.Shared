using System.Drawing; // keep here.

namespace WpfUserControlLib.Services;

public partial class GuiCapture
{
  public static Bitmap StoreActiveWindowScreenshotToFile(string shortNote)
  {
    var bmp = CaptureActiveWindow();

#if !VisualCapture
    const string lcl = """C:\Temp\Logs.Viz\""";
    const string frm = $"{{0}}{{1}}-{{2}}-{{3:MM.dd-HH.mm.ss}}-{{4}}.jpg";
    var pfn = string.Format(frm, OneDrive.Folder("""Public\Logs.Viz"""), Assembly.GetEntryAssembly()?.GetName().Name ?? "Unkn", Environment.UserName[..3], DateTime.Now, string.Concat(shortNote.Split(Path.GetInvalidFileNameChars())));

    FSHelper.ExistsOrCreated(Path.GetDirectoryName(pfn) ?? lcl);

    const int maxLen = 26;
    if (shortNote.Length > maxLen)
      shortNote = shortNote[..maxLen];

    bmp.Save(pfn, System.Drawing.Imaging.ImageFormat.Jpeg);
#endif

    return bmp;
  }
  public static Bitmap CaptureActiveWindow() => CaptureWindow(GetForegroundWindow());
  public static Bitmap CaptureWholeDesktop() => CaptureWindow(GetDesktopWindow());
  public static Bitmap CaptureWindow(IntPtr handle)
  {
    var rect = new Rect();
    GetWindowRect(handle, ref rect);
    var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
    var result = new Bitmap(bounds.Width, bounds.Height);

    using var graphics = Graphics.FromImage(result);
    graphics.CopyFromScreen(new System.Drawing.Point(bounds.Left, bounds.Top), System.Drawing.Point.Empty, bounds.Size);

    return result;
  }

  [LibraryImport("user32.dll")] private static partial IntPtr GetForegroundWindow();
  [LibraryImport("user32.dll")] private static partial IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
  [LibraryImport("user32.dll")] public static partial IntPtr GetDesktopWindow();
  [StructLayout(LayoutKind.Sequential)] struct Rect { public int Left; public int Top; public int Right; public int Bottom; }
}
