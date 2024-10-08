﻿namespace WinFormsControlLib;

public partial class WindowTopMoster // https://chat.openai.com/chat : better to use the OpenAI's tried method, than this unknown piece (2022-12)
{
  [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)] private static partial IntPtr FindWindow(string? lpClassName, string lpWindowName);
  [LibraryImport("user32.dll")]  [return: MarshalAs(UnmanagedType.Bool)] private  static partial bool SetForegroundWindow(IntPtr hWnd);
  [LibraryImport("user32.dll")]  [return: MarshalAs(UnmanagedType.Bool)] private  static partial bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

  const int WM_LBUTTONDOWN = 0x0201;
  const int WM_LBUTTONUP = 0x0202;

  public void PostMouseDownUpToWindow(string windowTitle)
  {
    var windowHandle = FindWindow(null, windowTitle);
    if (windowHandle == IntPtr.Zero)
    {
      Console.WriteLine("Window not found: {0}", windowTitle);
      return;
    }

    _ = SetForegroundWindow(windowHandle);

    _ = PostMessage(windowHandle, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
    Thread.Sleep(10); // Wait a short time to ensure the messages are processed
    _ = PostMessage(windowHandle, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
  }

  public void SendTabKeyToWindow(string processName)
  {
    var process = Process.GetProcessesByName(processName)[0];

    _ = SetForegroundWindow(process.MainWindowHandle);

    var x = 100;
    var y = 100;
    _ = new MouseEventArgs(MouseButtons.Left, 1, x, y, 0);
    _ = new MouseEventArgs(MouseButtons.Left, 1, x, y, 0);
    //var window = (IWin32Window)process.MainWindowHandle;
    SendKeys.SendWait("{TAB}");
    //window.RaiseMouseDown(mouseDown);
    //window.RaiseMouseUp(mouseUp);
  }
}