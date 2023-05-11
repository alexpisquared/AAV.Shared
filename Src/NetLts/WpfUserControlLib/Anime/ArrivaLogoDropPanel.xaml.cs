namespace WpfUserControlLib.Anime;
public partial class ArrivaLogoDropPanel
{
  public ArrivaLogoDropPanel() => InitializeComponent();

  public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(ArrivaLogoDropPanel), new PropertyMetadata(false, new PropertyChangedCallback(StartStopCallback))); public bool IsBusy { get => (bool)GetValue(IsBusyProperty); set => WpfUtils.AutoInvokeOnUiThread(SetValue, IsBusyProperty, value); }

  static void StartStopCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as ArrivaLogoDropPanel)?.TogglePlay((bool)e.NewValue);
  void TogglePlay(bool isBusy)
  {
    try
    {
      theGravity.IsBusy = isBusy;
    }
    //catch (OperationCanceledException ex) { Logger.LogWarning(ex.Message); }
        catch (OperationCanceledException ex)
    {
      WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] --Ignore OperationCanceledException {ex.Message}");
    }
    catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
    {
      WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] --Ignore AggregateException>>OperationCanceledException {ex.InnerException.Message}");
    }
  }
}