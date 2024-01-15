namespace WpfUserControlLib.Controls;
public partial class GSReportUserControl : UserControl
{
  public GSReportUserControl() => InitializeComponent();
  public static readonly DependencyProperty GSReportProperty = DependencyProperty.Register("GSReport", typeof(string), typeof(GSReportUserControl), new PropertyMetadata("efg")); public string GSReport { get => (string)GetValue(GSReportProperty); set => SetValue(GSReportProperty, value); }
  void OnHideGSReport(object s, RoutedEventArgs e) => Opener0.IsChecked = false;
}
