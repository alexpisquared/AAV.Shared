namespace WpfUserControlLib.Views;

public partial class ExceptionPopup : WindowBase
{
  const string _dotnet4exe = """C:\g\Util\Src\OpenInVsOnTheCulpritLine\bin\Release\OpenInVsOnTheCulpritLine.exe""";
  readonly string? msg, cmn, cfp;
  readonly Exception? ex;
  readonly int cln = 0;

  public ExceptionPopup() => InitializeComponent();
  public ExceptionPopup(Exception ex, string msg, string cmn, string cfp, int cln, Window owner) : this()
  {
    this.ex = ex;
    this.msg = msg;
    this.cmn = cmn;
    this.cfp = cfp;
    this.cln = cln;
    IgnoreWindowPlacement = true;
    Owner = owner;
    WindowStartupLocation = owner != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen;
    InitializeComponent();
  }

  async void OnLoaded(object s, RoutedEventArgs e)
  {
    tbkNow.Text = $"{DateTime.Now:yyyy-MM-dd HH:mm}";
    ExType.Text = ex?.GetType().Name;
    callerFL.Text = $"{cfp} ({cln}): ";
    methodNm.Text = $"{cmn}()";
    optnlMsg.Text = msg;
    innrMsgs.Text = ex.InnerMessages();
    if (VersionHelper.IsDbgAndRBD && cfp is not null)
      OpenVsOnTheCulpritLine(cfp, cln);

    Hand.Play();
    await Task.Delay(((Duration)FindResource("showDuration")).TimeSpan + ((Duration)FindResource("showDuratio2")).TimeSpan);
    Beep.Play();
    await Task.Delay(1250);
    Close(); // close popup and continue app execution
  }
  protected override void OnClosed(EventArgs e)  {    Loaded -= OnLoaded;    base.OnClosed(e);  }

  void OnAppShutdown(object s, RoutedEventArgs e) => Application.Current.Shutdown(55);
  void OnCopyAndContinue(object s, RoutedEventArgs e)
  {
    Clipboard.SetText($"{callerFL.Text}\r\n{methodNm.Text}\r\n{optnlMsg.Text}\r\n{innrMsgs.Text}");
    Close(); // close popup and continue app execution
  }

  static void OpenVsOnTheCulpritLine(string filename, int fileline)
  {
#if DotNet4
    EnvDTE80.DTE2 dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.17.0");
    dte2.MainWindow.Activate();
    EnvDTE.Window w = dte2.ItemOperations.OpenFile(filename, EnvDTE.Constants.vsViewKindTextView);
    ((EnvDTE.TextSelection)dte2.ActiveDocument.Selection).GotoLine(fileline, true);

    /* also see:
    https://docs.microsoft.com/en-us/visualstudio/extensibility/launch-visual-studio-dte?view=vs-2022
    https://github.com/diimdeep/VisualStudioFileOpenTool
    */
#else
    if (File.Exists(_dotnet4exe))
      Process.Start(_dotnet4exe, $"{filename} {fileline}");
    else
      _ = MessageBox.Show(_dotnet4exe, "Missing VS opener EXE");
#endif
  }

  void OnCopySection(object s, RoutedEventArgs e)
  {
    if (FindName(((Button)s).Tag.ToString()) is not TextBlock textblock)
      Hand.Play();
    else
    {
      Beep.Play();
      Clipboard.SetText(textblock.Text);
    }
  }
}