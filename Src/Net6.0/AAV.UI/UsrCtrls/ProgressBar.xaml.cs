using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AAV.Common.UI.UsrCtrls
{
	public partial class ProgressBar : UserControl
	{
		public ProgressBar() { InitializeComponent(); } //tu: !!! Binding ElementName=uc, Path= DataContext = this;

		static void reflect(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var pb = (d as ProgressBar);
			pb.Progress = pb.ActualWidth * pb.PBValue / pb.Maximum;
		}

		public double Progress { get { return (double)GetValue(ProgressProperty); } set { SetValue(ProgressProperty, value); } }
		public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(ProgressBar), new PropertyMetadata(199.0));
		public double Minimum { get { return (double)GetValue(MinimumProperty); } set { SetValue(MinimumProperty, value); } }
		public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(ProgressBar), new PropertyMetadata(0.0));
		public double Maximum { get { return (double)GetValue(MaximumProperty); } set { SetValue(MaximumProperty, value); } }
		public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(ProgressBar), new PropertyMetadata(100.0));
		public double PBValue { get { return (double)GetValue(PBValueProperty); } set { SetValue(PBValueProperty, value); } }
		public static readonly DependencyProperty PBValueProperty = DependencyProperty.Register("PBValue", typeof(double), typeof(ProgressBar), new PropertyMetadata(33.0, new PropertyChangedCallback(reflect)));

		public Brush ForeGround { get { return (Brush)GetValue(ForeGroundProperty); } set { SetValue(ForeGroundProperty, value); } }
		public static readonly DependencyProperty ForeGroundProperty = DependencyProperty.Register("ForeGround", typeof(SolidColorBrush), typeof(ProgressBar));
		public Brush BackGround { get { return (Brush)GetValue(BackGroundProperty); } set { SetValue(BackGroundProperty, value); } }
		public static readonly DependencyProperty BackGroundProperty = DependencyProperty.Register("BackGround", typeof(SolidColorBrush), typeof(ProgressBar));
	}
}
