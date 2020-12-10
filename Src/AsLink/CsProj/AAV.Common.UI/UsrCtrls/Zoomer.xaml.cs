using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AAV.Common.UI.UsrCtrls
{
	public partial class Zoomer : UserControl
	{
		public Zoomer()
		{
			InitializeComponent();

			//Loaded += (_slct, e) =>			{				try { ZoomSlider.ZmValue = Settings.Default.Zoom; }				catch { ZoomSlider.ZmValue = 1; }			};

			ZoomSlider.MouseDoubleClick += (s, e) => ZoomSlider.Value = 1;
			ZoomPrgBar.MouseDoubleClick += (s, e) => ZoomSlider.Value = 1;
			ZoomSlider.MouseWheel += (s, e) => ZoomSlider.Value += (e.Delta * .0002);
			ZoomPrgBar.MouseWheel += (s, e) => ZoomSlider.Value += (e.Delta * .0002);

			DataContext = this;
		}

		public static readonly DependencyProperty ZmValueProperty = DependencyProperty.Register("ZmValue", typeof(double), typeof(Zoomer), new PropertyMetadata(1.0, new PropertyChangedCallback(OnTrgValChanged))); public double ZmValue { get { return (double)GetValue(ZmValueProperty); } set { SetValue(ZmValueProperty, value); } }
		public static readonly DependencyProperty AnimZVProperty = DependencyProperty.Register("AnimZV", typeof(double), typeof(Zoomer), new PropertyMetadata(1d)); public double AnimZV { get { return (double)GetValue(AnimZVProperty); } set { SetValue(AnimZVProperty, value); } }
		static void OnTrgValChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { _da.To = (double)e.NewValue; (d as Zoomer).BeginAnimation(Zoomer.AnimZVProperty, _da); }
		static DoubleAnimation _da = new DoubleAnimation { Duration = new Duration(TimeSpan.FromSeconds(1.0)), EasingFunction = new ElasticEase { Springiness = 6 } };
	}
}
