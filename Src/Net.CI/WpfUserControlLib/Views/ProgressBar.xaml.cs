using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CI.Standard.Lib.Views
{
  public partial class ProgressBar : UserControl
  {
    public ProgressBar() => InitializeComponent();

    static void reflect(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var pb = (d as ProgressBar);
      if (pb == null) return;
      pb.Progress = pb.ActualWidth * pb.PBValue / pb.Maximum;
    }

    public double Progress { get => (double)GetValue(ProgressProperty); set => SetValue(ProgressProperty, value); }
    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(ProgressBar), new PropertyMetadata(199.0));
    public double Minimum { get => (double)GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(ProgressBar), new PropertyMetadata(0.0));
    public double Maximum { get => (double)GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(ProgressBar), new PropertyMetadata(100.0));
    public double PBValue { get => (double)GetValue(PBValueProperty); set => SetValue(PBValueProperty, value); }
    public static readonly DependencyProperty PBValueProperty = DependencyProperty.Register("PBValue", typeof(double), typeof(ProgressBar), new PropertyMetadata(33.0, new PropertyChangedCallback(reflect)));

    public Brush ForeGround { get => (Brush)GetValue(ForeGroundProperty); set => SetValue(ForeGroundProperty, value); }
    public static readonly DependencyProperty ForeGroundProperty = DependencyProperty.Register("ForeGround", typeof(SolidColorBrush), typeof(ProgressBar), new PropertyMetadata(Brushes.Red));
    public Brush BackGround { get => (Brush)GetValue(BackGroundProperty); set => SetValue(BackGroundProperty, value); }
    public static readonly DependencyProperty BackGroundProperty = DependencyProperty.Register("BackGround", typeof(SolidColorBrush), typeof(ProgressBar), new PropertyMetadata(Brushes.Black));
  }
}
