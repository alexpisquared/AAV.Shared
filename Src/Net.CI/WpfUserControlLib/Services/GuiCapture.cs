using System.Drawing; // keep here.

namespace CI.Visual.Lib.Services;

public class GuiCapture
{
  public static Bitmap StoreActiveWindowToFile(string shortNote)
  {
    var bmp = CaptureActiveWindow();

#if VisualCapture
    FSHelper.ExistsOrCreated(Path.GetDirectoryName(_pfn) ?? _dir);

    const int maxLen = 20;
    if(shortNote.Length > maxLen)
      shortNote = shortNote[..maxLen];

    var pfn = string.Format(_pfn, Assembly.GetEntryAssembly()?.GetName().Name ?? "Unkn", Environment.UserName[..3], DateTime.Now, string.Concat(shortNote.Split(Path.GetInvalidFileNameChars())));
    bmp.Save(pfn, ImageFormat.Jpeg);
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

    using (var graphics = Graphics.FromImage(result))
    {
      graphics.CopyFromScreen(new System.Drawing.Point(bounds.Left, bounds.Top), System.Drawing.Point.Empty, bounds.Size);
    }

    return result;
  }

  [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
  [DllImport("user32.dll")] static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
  [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)] public static extern IntPtr GetDesktopWindow();
  [StructLayout(LayoutKind.Sequential)] struct Rect { public int Left; public int Top; public int Right; public int Bottom; }

  const string _dir = @"Z:\Dev\AlexPi\Misc\Logs.Viz\";
  const string _pfn = $"{_dir}{{0}}-{{1}}-{{2:MM.dd-HH.mm.ss}}-{{3}}.jpg";
}
