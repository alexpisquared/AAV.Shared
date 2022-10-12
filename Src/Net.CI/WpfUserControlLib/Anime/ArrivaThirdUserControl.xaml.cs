namespace WpfUserControlLib.Anime;
public partial class ArrivaThirdUserControl : UserControl
{
  public ArrivaThirdUserControl()
  {
    InitializeComponent();
    DataContext = this;
  }
  public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(ArrivaThirdUserControl), new PropertyMetadata(false, propertyChangedCallback)); public bool IsBusy { get => (bool)GetValue(IsBusyProperty); set => SetValue(IsBusyProperty, value); }
  static void propertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => WriteLine($"@@@@@@@@ propertyChangedCallback   {e.NewValue}    {d.GetType().Name}");
}
