namespace WpfUserControlLib.Anime;
public partial class GravityAnimeDropPanel : UserControl
{
  public GravityAnimeDropPanel() => InitializeComponent();

  public static readonly DependencyProperty PromptProperty = DependencyProperty.Register("Prompt", typeof(string), typeof(GravityAnimeDropPanel)); public string Prompt { get => (string)GetValue(PromptProperty); set => WpfUtils.AutoInvokeOnUiThread(SetValue, PromptProperty, value); }
  public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(GravityAnimeDropPanel), new PropertyMetadata(false, new PropertyChangedCallback(StartStopCallback))); public bool IsBusy { get => (bool)GetValue(IsBusyProperty); set => WpfUtils.AutoInvokeOnUiThread(SetValue, IsBusyProperty, value); }

  static void StartStopCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as GravityAnimeDropPanel)?.TogglePlay((bool)e.NewValue);
  void TogglePlay(bool isBusy)
  {
    try
    {
      theGravity.IsBusy = isBusy;
      theGravity.Prompt = Prompt;
    }
    catch (OperationCanceledException ex)
    {
      WriteLine($"[xx:xx:xx Trc] --Ignore OperationCanceledException {ex.Message}");
    }
    catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
    {
      WriteLine($"[xx:xx:xx Trc] --Ignore AggregateException>>OperationCanceledException {ex.InnerException.Message}");
    }
  }
}