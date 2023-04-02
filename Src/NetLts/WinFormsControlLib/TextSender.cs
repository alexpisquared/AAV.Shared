using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsLib;
public partial class TextSender
{
  const int _disamb = 200;
  [LibraryImport("User32.Dll")] private static partial long SetCursorPos(int x, int y);
  [LibraryImport("user32.dll", EntryPoint = "FindWindow", StringMarshalling = StringMarshalling.Utf16)] private static partial IntPtr FindWindow(string? lp1, string lp2);
  [LibraryImport("user32.dll")]  [return: MarshalAs(UnmanagedType.Bool)] private static partial bool SetForegroundWindow(IntPtr hWnd);
  [LibraryImport("user32.dll")]  [return: MarshalAs(UnmanagedType.Bool)] private  static partial bool GetWindowRect(IntPtr hwnd, ref Rect1 rectangle);

  public struct Rect1
  {
    public int Left { get; set; }
    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }
  }
  public (int Right, int Bottom) GetRB(IntPtr winh)
  {
    var mspaintRect = new Rect1();
    if (GetWindowRect(winh, ref mspaintRect))
      return (mspaintRect.Right, mspaintRect.Bottom);

    throw new InvalidOperationException("Failed to get the WinH coordinates.");
  }
  public void SendPOC() => SendMany("Standing by for a go-ahead to start the change...            {ENTER}", 3);
  // "Notepad", "*Untitled - Notepad", "Standing by for a go-ahead to start the change...{TAB}***{ENTER}");
  // "Notepad", "*Untitled - Notepad", "Standing by for a go-ahead to start the change...{TAB}***{ENTER}");
  public void SendMany(string msg, int tms)
  {
    for (var i = 0; i < tms; i++)
    {
      var rv = SendOnce(msg);
      if (!string.IsNullOrEmpty(rv))
      {
        _ = MessageBox.Show(rv, "failed");
        return;
      }

      Thread.Sleep(1000);
    }
  }
  public string SendOnce(string msg,
#if DEBUG
    string ttl = "Pigida, Alex (You) | Microsoft Teams",
#else
    string ttl = "Income Payment Dev | Microsoft Teams",
#endif
    string app = "Microsoft Teams")
  {
    try
    {
      var handle = FindWindow(null, ttl); // var handle = FindWindow("Notepad", "*Untitled - Notepad");
      return handle.Equals(IntPtr.Zero) ? $"{app}\n\n{ttl}\n\n{msg}" : SendMsg(handle, msg);
    }
    catch (Exception ex) { return ex.Message; }
  }
  public string SendMsg(IntPtr handle, string msg)
  {
    WriteLine($">> SendMsg() to    handle:{handle,9}    msg:'{msg}'.");

    try
    {
      if (SetForegroundWindow(handle))
      {
        SendKeys.SendWait(msg); // SendKeys.SendWait(msg.Replace("\n", "  ").Replace("\r", "  "));
      }
      else
      {
        return $"SetForegroundWindow {handle} failed";
      }

      return ""; // success
    }
    catch (Exception ex) { return ex.Message; }
  }

  ///https://stackoverflow.com/questions/2531828/how-to-enumerate-all-windows-belonging-to-a-particular-process-using-net
  ///
  delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

  const uint WM_GETTEXT = 0x000D;
  [LibraryImport("user32.dll")]  [return: MarshalAs(UnmanagedType.Bool)]  private static partial bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam); 
  [DllImport("user32.dll", CharSet = CharSet.Auto)] static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

  List<IntPtr> EnumerateProcessWindowHandles(int processId)
  {
    var handles = new List<IntPtr>();
    foreach (ProcessThread thread in Process.GetProcessById(processId).Threads) _ = EnumThreadWindows(thread.Id, (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);
    return handles;
  }
  public async Task<IntPtr?> GetFirstMatch(string proc, string winttl, bool byEndsWith = false)
  {
    var procWinList = EnumerateProcessWindowHandles(Process.GetProcessesByName(proc).First().Id);

    if (DateTime.Now == DateTime.Today)
    {
      WriteLine($"./> All Windows for process '{proc}':");
      for (var i = 0; i < procWinList.Count; i++)
      {
        var handle = procWinList[i];
        var title = new StringBuilder(1000);
        _ = SendMessage(handle, WM_GETTEXT, title.Capacity, title);

        WriteLine($"./> {1 + i,3} / {procWinList.Count}     Title: '{title}'.");
      }
    }

    foreach (var handle in procWinList)
    {
      var title = new StringBuilder(1000);
      _ = SendMessage(handle, WM_GETTEXT, title.Capacity, title);

      if (byEndsWith)
      {
        if (title.ToString().EndsWith(winttl))
          return handle;
      }
      else
      {
        if (title.ToString() == winttl)
          return handle;
      }
    }

    await Task.Yield();
    return null;
  }
  public async Task<(IntPtr? reader, IntPtr? writer, string whyFailed)> GetTwoWinndows(string proc, string winttl, Action tick)
  {
    var handles = new List<IntPtr>();
    foreach (var handle in EnumerateProcessWindowHandles(Process.GetProcessesByName(proc).First().Id))
    {
      var title = new StringBuilder(1000);
      _ = SendMessage(handle, WM_GETTEXT, title.Capacity, title);
      if (title.ToString() == winttl)
      {
        handles.Add(handle);
      }

      WriteLine($"./> Title: '{title}'.");
    }

    if (handles.Count != 2) return (null, null, $"Error:   handles.Count {handles.Count} != 2");
    var txt0 = await GetTargetTextFromWindow(handles[0], tick);
    var txt1 = await GetTargetTextFromWindow(handles[1], tick);

    for (var i = 0; txt0.Length == txt1.Length && i < 10; i++)
    {
      await Task.Delay(100);
      WriteLine($"..{i,4})  Seems the same: len:{txt1.Length}.");
      tick();
      txt1 = await GetTargetTextFromWindow(handles[1], tick);
    }

    if (txt0.Length == txt1.Length)
    {
      var spacer = " >scr< ";
      _ = SendMsg(handles[1], spacer);
      return (null, null, $"Error:   Same len of {txt0.Length}  ==> Typing '{spacer}' to hndl[1] ...");
    }

    return txt0.Length > txt1.Length ?
      ((IntPtr? reader, IntPtr? writer, string whyFailed))(handles[0], handles[1], "") :
      ((IntPtr? reader, IntPtr? writer, string whyFailed))(handles[1], handles[0], "");
  }
  public async void FindByProc(Action tick, string proc = "msteams", string winttl = "Oleksa Pigid | Microsoft Teams")
  {
    var i = 0;
    foreach (var handle in EnumerateProcessWindowHandles(Process.GetProcessesByName(proc).First().Id))
    {
      var title = new StringBuilder(1000);
      _ = SendMessage(handle, WM_GETTEXT, title.Capacity, title);
      if (title.ToString() == winttl)
      {
        var txt = await GetTargetTextFromWindow(handle, tick);
        WriteLine($">{++i,2})    {proc}    {title}    {SafeLengthTrim(txt)}");
      }
    }
  }

  public static string SafeLengthTrim(string txt, int maxLen = 52) => txt.Length > maxLen ? (txt[..maxLen].Replace("\n", "  ").Replace("\r", "  ") + "...").Trim() : txt;

  public async Task<string> GetTargetTextFromWindow(IntPtr handle, Action tick)
  {
    _ = SetForegroundWindow(handle);      /**/  await Task.Delay(_disamb); tick();
    SendKeys.SendWait("^a");              /**/  await Task.Delay(_disamb); //tick();
    SendKeys.SendWait("^c");              /**/  await Task.Delay(_disamb); //tick();
    return Clipboard.GetText();
  }
  public async Task<string> TabAndGetText(IntPtr handle)
  {
    _ = SetForegroundWindow(handle);      /**/  await Task.Delay(_disamb);
    SendKeys.SendWait("{TAB}");
    SendKeys.SendWait("^a");
    SendKeys.SendWait("^c");
    return Clipboard.GetText();
  }

  public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam); //utility function that will find a child window that matches a lambda (Predicate). Be easy to change to return a list. Multiple criteria are handled in the predicate.
  [LibraryImport("user32.Dll")][return: MarshalAs(UnmanagedType.Bool)] private static partial bool EnumChildWindows(IntPtr parentHandle, Win32Callback callback, IntPtr lParam);

  /// <summary>
  /// Find a child window that matches a set of conditions specified as a Predicate that receives hWnd.  Returns IntPtr.Zero
  /// if the target window not found.  Typical search criteria would be some combination of window attributes such as
  /// ClassName, Title, etc., all of which can be obtained using API functions you will find on pinvoke.net
  /// </summary>
  /// <remarks>
  ///     <para>Example: Find a window with specific title (use Regex.IsMatch for more sophisticated search)</para>
  ///     <code lang="C#"><![CDATA[var foundHandle = Win32.FindWindow(IntPtr.Zero, ptr => Win32.GetWindowText(ptr) == "Dashboard");]]></code>
  /// </remarks>
  /// <param name="parentHandle">Handle to window at the start of the chain.  Passing IntPtr.Zero gives you the top level
  /// window for the current process.  To get windows for other processes, do something similar for the FindWindow
  /// API.</param>
  /// <param name="target">Predicate that takes an hWnd as an IntPtr parameter, and returns True if the window matches.  The
  /// first match is returned, and no further windows are scanned.</param>
  /// <returns> hWnd of the first found window, or IntPtr.Zero on failure </returns>
  public static IntPtr FindWindow(IntPtr parentHandle, Predicate<IntPtr> target)
  {
    var result = IntPtr.Zero;
    if (parentHandle == IntPtr.Zero)
      parentHandle = Process.GetCurrentProcess().MainWindowHandle;
    _ = EnumChildWindows(parentHandle, (hwnd, param) =>
    {
      if (target(hwnd))
      {
        result = hwnd;
        return false;
      }

      return true;
    }, IntPtr.Zero);
    return result;
  }
  //Example
  //var foundHandle = Win32.FindWindow(IntPtr.Zero, ptr => Win32.GetWindowText(ptr) == "Dashboard");
}