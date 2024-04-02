namespace WpfUserControlLib.Controls;
public partial class GSReportUserControl : UserControl
{
  public GSReportUserControl() => InitializeComponent();
  void OnHideGSReport(object s, RoutedEventArgs e) => Opener0.IsChecked = false;
  void OnClearReport(object s, RoutedEventArgs e) => GSReport = "*"; //todo: breaks the binding: gets stuck with the last value.

  public static readonly DependencyProperty GSReportProperty = DependencyProperty.Register("GSReport", typeof(string), typeof(GSReportUserControl), new PropertyMetadata("+efg+", PropertyCnanged)); public string GSReport { get => (string)GetValue(GSReportProperty); set => SetValue(GSReportProperty, value); }
  static void PropertyCnanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as GSReportUserControl)?.ScrollViewer1.ScrollToEnd();
}