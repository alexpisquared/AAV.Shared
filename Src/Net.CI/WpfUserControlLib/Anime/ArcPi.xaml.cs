using CI.Visual.Lib.Helpers;

namespace CI.Visual.Lib.Anime;

public partial class ArcPi : UserControl
{
  public ArcPi() => InitializeComponent();
  public static readonly DependencyProperty RadiuProperty = DependencyProperty.Register("Radiu", typeof(double), typeof(ArcPi), new PropertyMetadata(50d)); public double Radiu { get => (double)GetValue(RadiuProperty); set => SetValue(RadiuProperty, value); }
  public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(ArcPi), new PropertyMetadata(0d, PropertyChangedCallback)); public double Angle { get => (double)GetValue(AngleProperty); set => SetValue(AngleProperty, value); }
  static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ArcPi)d).DrawArc((double)e.NewValue);
  void DrawArc(double anglePercent) => ArcHelper.DrawArc(arc_path, new Point(Radiu, Radiu), Radiu - (arc_path.StrokeThickness / 2), 0, anglePercent * Math.PI * .02);
}