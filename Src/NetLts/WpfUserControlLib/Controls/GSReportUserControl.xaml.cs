namespace WpfUserControlLib.Controls;
public partial class GSReportUserControl : UserControl
{
  public GSReportUserControl() => InitializeComponent();
  void OnHideGSReport(object s, RoutedEventArgs e) { GSReport += ""; Opener0.IsChecked = false; }

  public static readonly DependencyProperty GSReportProperty = DependencyProperty.Register("GSReport", typeof(string), typeof(GSReportUserControl), new PropertyMetadata("+efg+", PropertyCnanged)); public string GSReport { get => (string)GetValue(GSReportProperty); set => SetValue(GSReportProperty, value); }
  static void PropertyCnanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as GSReportUserControl)?.ScrollViewer1.ScrollToEnd();
}
