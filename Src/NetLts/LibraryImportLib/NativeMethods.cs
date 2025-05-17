namespace LibraryImportLib;

public static class NativeMethods
{
  public static void BeepIf(int freq, int dur)
  {
    if (IsDbgOrRBD)
      _ = WinAPI.Beep(freq, dur); // use this when not referencing StdLib:     WinAPI.Beep(freq, dur);
  }
  public static bool SetWindowPlacement_(nint hWnd, [In] ref WinAPI.WindowPlacement lpwndpl) => WinAPI.SetWindowPlacement(hWnd, ref lpwndpl);
  public static bool GetWindowPlacement_(nint hWnd, /**/ out WinAPI.WindowPlacement lpwndpl) => WinAPI.GetWindowPlacement(hWnd, out lpwndpl);

  public static bool IsDbgOrRBD => IsDbg || IsRBD; // Ran by Dev.
  public static bool IsRBD => Environment.UserName.EndsWith("lexp") || Environment.UserName.StartsWith("olepi"); // ran by dev.
  public static bool IsDbg =>
#if DEBUG
      true;
#else
      false;
#endif
}
