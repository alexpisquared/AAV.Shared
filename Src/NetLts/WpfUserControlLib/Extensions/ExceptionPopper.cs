namespace WpfUserControlLib.Extensions;

public static class ExnPopr
{
  public static void Pop(this Exception ex, [CallerLineNumber] int cln = 0, [CallerMemberName] string cmn = "", [CallerFilePath] string cfp = "") => ex.Pop(Application.Current.MainWindow, "", null, cmn, cfp, cln);
  public static void Pop(this Exception ex, string note, [CallerMemberName] string cmn = "", [CallerFilePath] string cfp = "", [CallerLineNumber] int cln = 0) => ex.Pop(Application.Current.MainWindow, note, null, cmn, cfp, cln);
  public static void Pop(this Exception ex, ILogger lgr, [CallerMemberName] string cmn = "", [CallerFilePath] string cfp = "", [CallerLineNumber] int cln = 0) => ex.Pop(Application.Current.MainWindow, "", lgr, cmn, cfp, cln);
  public static void Pop(this Exception ex, string note, ILogger? lgr, [CallerMemberName] string cmn = "", [CallerFilePath] string cfp = "", [CallerLineNumber] int cln = 0) => ex.Pop(Application.Current.MainWindow, note, lgr, cmn, cfp, cln);
  public static void Pop(this Exception ex, UserControl u, string note = "", ILogger? lgr = null, [CallerMemberName] string cmn = "", [CallerFilePath] string cfp = "", [CallerLineNumber] int cln = 0) => ex.Pop(u.FindParentWindow(), note, lgr, cmn, cfp, cln);

  public static void Pop(this Exception ex, Window owner, string note, ILogger? lgr, [CallerMemberName] string cmn = "", [CallerFilePath] string cfp = "", [CallerLineNumber] int cln = 0)
  {
    var msgForPopup = ex.Log(note, cmn, cfp, cln);

    lgr?.LogError(ex, msgForPopup.Replace("\n", " ").Replace("\n", " ").Replace("\r", " ") + "    StoreActiveWindowScreenshotToFile?!?!?!");
    try
    {
      if (Debugger.IsAttached)
        return;

      _ = GuiCapture.StoreActiveWindowScreenshotToFile(msgForPopup);
      _ = WpfUtils.AutoInvokeOnUiThread(new ExceptionPopup(ex, note, cmn, cfp, cln, owner).ShowDialog);
    }
    catch (InvalidOperationException ex2) // for "Cannot set Owner property to a Window that has not been shown previously."
    {
      lgr?.LogError(ex2, $"(org ex: {msgForPopup})");

      if (--_circuitBreaker > 0)
        Pop(ex, Application.Current.MainWindow, note, lgr);
    }
    catch (Exception ex2)
    {
      lgr?.LogError(ex2, $"(org ex: {msgForPopup})");

      if (Debugger.IsAttached)
        Debugger.Break();
      else
        _ = MessageBox.Show(owner, $"{ex2.InnerMessages()} \n\nwhile reporting this:\n\n{msgForPopup}", $"Exception During Exception in {cmn}", MessageBoxButton.OK, MessageBoxImage.Error);
    }
  }

  static int _circuitBreaker = 8;
}
