namespace WinFormsControlLib;

public class ScreenHelper
{
  public static Screen[] GetAllScreens => Screen.AllScreens;
  public static Screen LargestScreen => Screen.AllScreens.First(r => r.WorkingArea.Width == Screen.AllScreens.Max(r => r.WorkingArea.Width));
  public static Screen PrimaryScreen => Screen.PrimaryScreen ?? throw new ArgumentNullException("▄▀▄▀▄ No Primary Screen ▀▄▀▄▀");
  public static Screen SecondaryScreen => (Screen.AllScreens.Length == 1 ? Screen.PrimaryScreen : Screen.AllScreens.First(r => r != Screen.PrimaryScreen)) ?? throw new ArgumentNullException("▄▀▄▀▄ No Primary nor Secondary Screen ▀▄▀▄▀");

  public static Rectangle GetSumOfAllBounds => Rectangle.Union(PrimaryScreen.Bounds, SecondaryScreen.Bounds);
  public static Rectangle GetFirstNonPrimaryScreenBounds() => Screen.AllScreens.FirstOrDefault(s => !s.Primary)?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
}