namespace WinFormsControlLib;

public class ScreenHelper
{
  public static Screen[] GetAllScreens => Screen.AllScreens;
  public static Screen LargestScreen => Screen.AllScreens.First(r => r.WorkingArea.Width == Screen.AllScreens.Max(r => r.WorkingArea.Width));
  public static Screen PrimaryScreen => Screen.PrimaryScreen ?? throw new ArgumentNullException("▄▀▄▀▄ No Primary Screen ▀▄▀▄▀");
  public static Screen SecondaryScreen => (Screen.AllScreens.Length == 1 ? Screen.PrimaryScreen : Screen.AllScreens.First(r => r != Screen.PrimaryScreen)) ?? throw new ArgumentNullException("▄▀▄▀▄ No Primary nor Secondary Screen ▀▄▀▄▀");

   public static Rectangle GetSumOfAllBounds
  {
    get
    {
      var screens = GetAllScreens;
      if (screens.Length == 0) return Rectangle.Empty;
      var union = screens[0].Bounds;
      for (int i = 1; i < screens.Length; i++) union = Rectangle.Union(union, screens[i].Bounds);

      return union;
    }
  }

  public static Rectangle GetFirstNonPrimaryScreenBounds()
  {
    var f = Screen.AllScreens.FirstOrDefault(s => !s.Primary)?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
    var l = Screen.AllScreens.LastOrDefault(s => !s.Primary)?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
    return f.Left > l.Left ? f : l;
  }
}
