namespace WpfUserControlLib.Views;
public partial class ExceptionPopup// : WindowBase
{
  readonly string? msg, cmn, cfp;
  readonly Exception? ex;
  readonly int cln = 0;
  //IBpr bpr = new Bpr(); ?? add ref to Ambience ?? is it worth it?

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
    KeepOpenReason = "";
  }

  async void OnLoaded(object s, RoutedEventArgs e)
  {
    tbkNow.Text = $"{DateTime.Now:yyyy-MM-dd HH:mm}";
    ExType.Text = ex?.GetType().Name;
    callerFL.Text = $"{cfp} ({cln}): ";
    methodNm.Text = $"{cmn}()";
    optnlMsg.Text = msg;
    innrMsgs.Text = ex.InnerMessages();

    Hand.Play();
    await Task.Delay(((Duration)FindResource("animDuration")).TimeSpan + ((Duration)FindResource("preAnimnWait")).TimeSpan);
    Beep.Play();
    await Task.Delay(1250);
    Close(); // close popup and continue app execution
  }
  protected override void OnClosed(EventArgs e) { Loaded -= OnLoaded; base.OnClosed(e); }

  void OnAppShutdown(object s, RoutedEventArgs e) => Application.Current.Shutdown(55);
  void OnCopyAndContinue(object s, RoutedEventArgs e)
  {
    Clipboard.SetText($"{callerFL.Text}\r\n{methodNm.Text}\r\n{optnlMsg.Text}\r\n{innrMsgs.Text}");
    Close(); // close popup and continue app execution
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