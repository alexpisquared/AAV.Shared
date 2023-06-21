namespace WinFormsControlLib;

public class WinFormHelper
{
  public static Screen[] GetAllScreens => Screen.AllScreens;
  public static Screen LargestScreen => Screen.AllScreens.First(r => r.WorkingArea.Width == Screen.AllScreens.Max(r => r.WorkingArea.Width));
  public static Screen PrimaryScreen => Screen.PrimaryScreen ?? throw new ArgumentNullException("▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀");
  public static Screen SecondaryScreen => Screen.AllScreens.First(r => r != Screen.PrimaryScreen);

  public static Rectangle GetFirstNonPrimaryScreenBounds()
  {
    var secondaryScreen = Screen.AllScreens.FirstOrDefault(s => !s.Primary);
    return (secondaryScreen is null) ? new Rectangle(0, 0, 1920, 1080) : secondaryScreen.Bounds;
  }
  public static Rectangle GetSumOfAllBounds => Rectangle.Union(PrimaryScreen.Bounds, SecondaryScreen.Bounds);
}