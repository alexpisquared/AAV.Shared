namespace WpfUserControlLib.Views;
public partial class ExceptionPopup// : WindowBase
{
  //IBpr bpr = new Bpr(); ?? add ref to Ambience ?? is it worth it?

  public ExceptionPopup() => InitializeComponent();
  public ExceptionPopup(Exception ex, string[] ary, Window owner) : this()
  {
    IgnoreWindowPlacement = true;
    Owner = owner;
    WindowStartupLocation = owner != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen;
    InitializeComponent();
    KeepOpenReason = "";

    tbkNow.Text = $"{DateTime.Now:yyyy-MM-dd HH:mm}";
    ExType.Text = ex?.GetType().Name;

    if (ary.Length > 0) Msg0.Text = ary[0];
    if (ary.Length > 1) Msg1.Text = ary[1];
    if (ary.Length > 2) Msg2.Text = ary[2];
    if (ary.Length > 3) Msg3.Text = ary[3];
    if (ary.Length > 4) Msg4.Text = ary[4];
  }
  public ExceptionPopup(Exception ex, string msg, string cmn, string cfp, int cln, Window owner) : this()
  {
    IgnoreWindowPlacement = true;
    Owner = owner;
    WindowStartupLocation = owner != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen;
    InitializeComponent();
    KeepOpenReason = "";

    tbkNow.Text = $"{DateTime.Now:yyyy-MM-dd HH:mm}";
    ExType.Text = ex?.GetType().Name;
    Msg2.Text = $"{cfp} ({cln}): ";
    Msg3.Text = $"{cmn}()";
    Msg0.Text = msg;
    Msg1.Text = ex.InnerMessages();
  }

  async void OnLoaded(object s, RoutedEventArgs e)
  {
    Hand.Play();
    await Task.Delay(((Duration)FindResource("animDuration")).TimeSpan + ((Duration)FindResource("preAnimnWait")).TimeSpan);
    Beep.Play();
    await Task.Delay(950); // 1250 a bit late (Aug 28)
    Close(); // close popup and continue app execution
  }
  protected override void OnClosed(EventArgs e) { Loaded -= OnLoaded; base.OnClosed(e); }

  void OnAppShutdown(object s, RoutedEventArgs e) => Application.Current.Shutdown(55);
  void OnCopyAndContinue(object s, RoutedEventArgs e)
  {
    Clipboard.SetText($"{Msg2.Text}\r\n{Msg3.Text}\r\n{Msg0.Text}\r\n{Msg1.Text}");
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