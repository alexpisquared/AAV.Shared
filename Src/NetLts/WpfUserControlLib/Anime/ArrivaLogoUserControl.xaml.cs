namespace WpfUserControlLib.Anime;

public partial class ArrivaLogoUserControl
{
  public ArrivaLogoUserControl()
  {
    InitializeComponent();
    DataContext = this;
  }
  public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(ArrivaLogoUserControl), new PropertyMetadata(false, propertyChangedCallback)); public bool IsBusy { get => (bool)GetValue(IsBusyProperty); set => SetValue(IsBusyProperty, value); }
  static void propertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => Debug.WriteLine($"@@@@@@@@ propertyChangedCallback  {e.NewValue}");
}
