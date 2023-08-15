namespace StandardLib.Helpers;

public static partial class NativeMethods
{
  [LibraryImport("user32.dll")] public static partial int SendMessage(int hWnd, uint wMsg, uint wParam, uint lParam);
  [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)] public static partial int SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, uint fWinIni);
  [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)] public static partial IntPtr SendMessageTimeout(IntPtr hWnd, int Msg, IntPtr wParam, string lParam, uint fuFlags, uint uTimeout, IntPtr lpdwResult);
  [DllImport("user32.dll", EntryPoint = "SendMessageTimeoutW", SetLastError = true, CharSet = CharSet.Unicode)] public static extern uint SendMessageTimeoutW(IntPtr hWnd, int Msg, int countOfChars, StringBuilder text, uint flags, uint uTImeoutj, uint result);

  internal static void BeepIf(int freq, int dur)
  {
    if (VersionHelper.IsDbgOrRBD)
      _ = Beep(freq, dur);
  }

#if DEBUG_ // just in case.
  static bool Beep(int freq, int dur) { WriteLine($"Silent Beep({freq}, {dur})"); return true; }
#else
  [LibraryImport("kernel32.dll")]  [return: MarshalAs(UnmanagedType.Bool)] private  static partial bool Beep(int freq, int dur);
#endif

  #region Win32 API declarations to set and get window placement
  public static bool SetWindowPlacement_(IntPtr hWnd, [In] ref WindowPlacement lpwndpl) => SetWindowPlacement(hWnd, ref lpwndpl);
  public static bool GetWindowPlacement_(IntPtr hWnd, out WindowPlacement lpwndpl) => GetWindowPlacement(hWnd, out lpwndpl);

  [LibraryImport("user32.dll")]  [return: MarshalAs(UnmanagedType.Bool)] private static partial bool SetWindowPlacement(IntPtr hWnd, /*[In]*/ ref WindowPlacement lpwndpl);
  [LibraryImport("user32.dll")]  [return: MarshalAs(UnmanagedType.Bool)] private  static partial bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement lpwndpl);

  [Serializable]
  [StructLayout(LayoutKind.Sequential)]
  public struct WindowPlacement
  {
    public int length;
    public int flags;
    public int showCmd;
    public Point minPosition;
    public Point maxPosition;
    public Rect normalPosition;

    public override bool Equals(object? obj) => obj is WindowPlacement placement && length == placement.length && flags == placement.flags && showCmd == placement.showCmd && EqualityComparer<Point>.Default.Equals(minPosition, placement.minPosition) && EqualityComparer<Point>.Default.Equals(maxPosition, placement.maxPosition) && EqualityComparer<Rect>.Default.Equals(normalPosition, placement.normalPosition);
    public static bool operator ==(WindowPlacement left, WindowPlacement right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(WindowPlacement left, WindowPlacement right)
    {
      return !(left == right);
    }

    public override int GetHashCode() => -987654321;
  }

  [Serializable]
  [StructLayout(LayoutKind.Sequential)]
  public struct Point
  {
    public int X, Y; public Point(int x, int y) { X = x; Y = y; }
  }

  [Serializable]
  [StructLayout(LayoutKind.Sequential)]
  public struct Rect
  {
    public int Left, Top, Right, Bottom; public Rect(int left, int top, int right, int bottom) { Left = left; Top = top; Right = right; Bottom = bottom; }
  } // RECT structure required by WINDOWPLACEMENT structure

  [Serializable]
  [StructLayout(LayoutKind.Sequential)]
  public struct WPContainer
  {
    public WindowPlacement WindowPlacement { get; set; }
    public double Zb { get; set; }
    public string Thm { get; set; }
  }
  #endregion
}
