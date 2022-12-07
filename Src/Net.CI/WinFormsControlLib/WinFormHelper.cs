using System;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsLib
{
  public class WinFormHelper
  {
    public static Screen[] GetAllScreens() => Screen.AllScreens;
    public static Screen LargestScreen => Screen.AllScreens.First(r => r.WorkingArea.Width ==(Screen.AllScreens.Max(r => r.WorkingArea.Width)));
    public static Screen PrimaryScreen => Screen.PrimaryScreen ?? throw new ArgumentNullException("▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀");
    public static Screen SecondaryScreen => Screen.AllScreens.First(r => r != Screen.PrimaryScreen);
  }

  static class Program {[STAThread] static void Main___() { } } //?? what is this doing here?..
}
