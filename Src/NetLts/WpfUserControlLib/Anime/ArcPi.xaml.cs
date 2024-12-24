namespace WpfUserControlLib.Anime;

public partial class ArcPi
{
  public ArcPi() => InitializeComponent();
  public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Brush), typeof(ArcPi), new PropertyMetadata(Brushes.Green)); public Brush Color { get => (Brush)GetValue(ColorProperty); set => SetValue(ColorProperty, value); }
  public static readonly DependencyProperty BckgrProperty = DependencyProperty.Register("Bckgr", typeof(Brush), typeof(ArcPi), new PropertyMetadata(Brushes.Fuchsia)); public Brush Bckgr { get => (Brush)GetValue(BckgrProperty); set => SetValue(BckgrProperty, value); }
  public static readonly DependencyProperty RadiuProperty = DependencyProperty.Register("Radiu", typeof(double), typeof(ArcPi), new PropertyMetadata(50d)); public double Radiu { get => (double)GetValue(RadiuProperty); set => SetValue(RadiuProperty, value); }
  public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(ArcPi), new PropertyMetadata(0d, PropertyChangedCallback)); public double Angle { get => (double)GetValue(AngleProperty); set => SetValue(AngleProperty, value); }
  static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ArcPi)d).DrawArc((double)e.NewValue);
  void DrawArc(double anglePercent) => DrawArc(arc_path, new Point(Radiu, Radiu), Radiu - (arc_path.StrokeThickness / 2), 0, anglePercent * Math.PI * .02);

  static void DrawArc(System.Windows.Shapes.Path path, Point center, double radius, double angleBgn_, double angleEnd_)
  {
    const double _2Pi = Math.PI * 2;
    var angleBgn = ((angleEnd_ < angleBgn_ ? angleEnd_ : angleBgn_ % _2Pi) + _2Pi) % _2Pi;
    var angleEnd = ((angleEnd_ < angleBgn_ ? angleBgn_ : angleEnd_ % _2Pi) + _2Pi) % _2Pi; //if (angleEnd < angleBgn)      (angleBgn, angleEnd) = (angleEnd, angleBgn); //tu: swap!!!

    var pathFigure = new PathFigure
    {
      StartPoint = new Point(center.X + (radius * Math.Cos(angleBgn)), center.Y + (radius * Math.Sin(angleBgn))) // :set start point of arc.
    };

    pathFigure.Segments.Add(new ArcSegment
    {
      Size = new Size(radius, radius),
      Point = new Point(center.X + (radius * Math.Cos(angleEnd)), center.Y + (radius * Math.Sin(angleEnd))), // :set end point of arc.
      IsLargeArc = angleEnd - angleBgn >= Math.PI,
      SweepDirection = SweepDirection.Clockwise
    });

    path.Data = new PathGeometry();
    ((PathGeometry)path.Data).Figures.Add(pathFigure);
  }
}