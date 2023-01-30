namespace WpfUserControlLib.Anime;
public partial class GravityAnime : UserControl
{
  readonly Storyboard _sbGravity, _sbFadeOut;
  readonly Duration _fadeOuDuration;
  public GravityAnime()
  {
    InitializeComponent();
    _sbGravity = (Storyboard)FindResource("sbGravity");
    _sbFadeOut = (Storyboard)FindResource("sbFadeOut");
    _fadeOuDuration = (Duration)FindResource("fadeOuDuration");
    Visibility = Visibility.Collapsed;
    pnlRoot.Opacity = 0;
  }

  public static readonly DependencyProperty PromptProperty = DependencyProperty.Register("Prompt", typeof(string), typeof(GravityAnime)); public string Prompt { get => (string)GetValue(PromptProperty); set => WpfUtils.AutoInvokeOnUiThread(SetValue, PromptProperty, value); }
  public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(GravityAnime), new PropertyMetadata(false, new PropertyChangedCallback(StartStopCallback))); public bool IsBusy { get => (bool)GetValue(IsBusyProperty); set => WpfUtils.AutoInvokeOnUiThread(SetValue, IsBusyProperty, value); }

  static void StartStopCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as GravityAnime)?.TogglePlay((bool)e.NewValue);
  void TogglePlay(bool isBusy)
  {
    try
    {
      if (isBusy) StartPlay(); else StopPlaying();
    }
    catch (OperationCanceledException ex)
    {
      Trace.WriteLine($"[xx:xx:xx Trc] --Ignore OperationCanceledException {ex.Message}");
    }
    catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
    {
      Trace.WriteLine($"[xx:xx:xx Trc] --Ignore AggregateException>>OperationCanceledException {ex.InnerException.Message}");
    }
  }

  void StartPlay()
  {
    Visibility = Visibility.Visible;
    _sbGravity.Begin();
  }
  async void StopPlaying()
  {
    _sbFadeOut.Begin();
    await Task.Delay(_fadeOuDuration.TimeSpan); // stop animation and remove artifacts from the visual tree upon completion of the fade out.
    _sbGravity.Stop();
  }
}