namespace LibraryImportLib;

public static class NativeMethods
{
  public static void BeepIf(int freq, int dur)
  {
    // if (VersionHelper.IsDbgOrRBD)
    _ = WinAPI.Beep(freq, dur);
    Console.Beep(freq, dur);
  }
  public static bool SetWindowPlacement_(nint hWnd, [In] ref WinAPI.WindowPlacement lpwndpl) => WinAPI.SetWindowPlacement(hWnd, ref lpwndpl);
  public static bool GetWindowPlacement_(nint hWnd, /**/ out WinAPI.WindowPlacement lpwndpl) => WinAPI.GetWindowPlacement(hWnd, out lpwndpl);
}
