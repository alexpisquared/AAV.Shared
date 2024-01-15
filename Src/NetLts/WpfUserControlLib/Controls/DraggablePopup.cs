namespace WpfUserControlLib.Controls;

public class DraggablePopup : Popup
{
  bool _isDragging;
  Point _initialMousePosition;

  protected override void OnInitialized(EventArgs e)
  {
    var contents = Child as FrameworkElement;
    ArgumentNullException.ThrowIfNull(contents, "■ DraggablePopup either has no content if content that does not derive from FrameworkElement. Must be fixed for dragging to work. ■");

    contents.MouseLeftButtonDown += Child_MouseLeftButtonDown;
    contents.MouseLeftButtonUp += Child_MouseLeftButtonUp;
    contents.MouseMove += Child_MouseMove;
  }

  void Child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    _initialMousePosition = e.GetPosition(null);
    
    _ = (sender as FrameworkElement)?.CaptureMouse();
    _isDragging = true;
    e.Handled = true;
  }

  void Child_MouseMove(object sender, MouseEventArgs e)
  {
    if (!_isDragging) return;

    var currentPoint = e.GetPosition(null);
    HorizontalOffset += currentPoint.X - _initialMousePosition.X;
    VerticalOffset += currentPoint.Y - _initialMousePosition.Y;
  }

  void Child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
  {
    if (!_isDragging) return;

    (sender as FrameworkElement)?.ReleaseMouseCapture();
    _isDragging = false;
    e.Handled = true;
  }
}