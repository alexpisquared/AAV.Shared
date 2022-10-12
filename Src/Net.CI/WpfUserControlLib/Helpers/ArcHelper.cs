namespace CI.Visual.Lib.Helpers;
class ArcHelper
{
  const double _2Pi = Math.PI * 2;

  public static void DrawArc(System.Windows.Shapes.Path path, Point center, double radius, double angleBgn_, double angleEnd_)
  {
    var angleBgn = ((angleEnd_ < angleBgn_ ? angleEnd_ : angleBgn_ % _2Pi) + _2Pi) % _2Pi;
    var angleEnd = ((angleEnd_ < angleBgn_ ? angleBgn_ : angleEnd_ % _2Pi) + _2Pi) % _2Pi; //if (angleEnd < angleBgn)      (angleBgn, angleEnd) = (angleEnd, angleBgn); //tu: swap!!!

    var arcSegment = new ArcSegment
    {
      Size = new Size(radius, radius),
      Point = new Point(center.X + (radius * Math.Cos(angleEnd)), center.Y + (radius * Math.Sin(angleEnd))), // :set end point of arc.
      IsLargeArc = angleEnd - angleBgn >= Math.PI,
      SweepDirection = SweepDirection.Clockwise
    };

    var pathFigure = new PathFigure
    {
      StartPoint = new Point(center.X + (radius * Math.Cos(angleBgn)), center.Y + (radius * Math.Sin(angleBgn))) // :set start of arc.
    };
    pathFigure.Segments.Add(arcSegment);

    path.Data = new PathGeometry();
    ((PathGeometry)path.Data).Figures.Add(pathFigure);
  }
}