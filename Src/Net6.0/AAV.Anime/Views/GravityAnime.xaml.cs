using AAV.WPF.Ext;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace AAV.Anime.Views
{
  public partial class GravityAnime : UserControl
  {
    readonly Storyboard _sbGravity, _sbFadeOut;
    readonly Duration _fadeOuDuration;
    public GravityAnime()
    {
      InitializeComponent();
      _sbGravity = (Storyboard)FindResource("sbGravity");
      _sbFadeOut = (Storyboard)FindResource("sbFadeOut");
      _fadeOuDuration = (Duration)FindResource("fadeOuDuration");
      Visibility = Visibility.Collapsed;
      pnlRoot.Opacity = 0;
    }

    public static readonly DependencyProperty PromptProperty = DependencyProperty.Register("Prompt", typeof(string), typeof(GravityAnime)); public string Prompt { get => (string)GetValue(PromptProperty); set => WpfUtils.AutoInvokeOnUiThread(SetValue, PromptProperty, value); }
    public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(GravityAnime), new PropertyMetadata(false, new PropertyChangedCallback(startStopCallback))); public bool IsBusy { get => (bool)GetValue(IsBusyProperty); set => WpfUtils.AutoInvokeOnUiThread(SetValue, IsBusyProperty, value); }

    static void startStopCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as GravityAnime)?.toggle((bool)e.NewValue);
    void toggle(bool isBusy)
    {
      try
      {
        if (isBusy) start(); else stop();
      }
      catch (OperationCanceledException ex)
      {
        Debug.WriteLine($"--Ignore OperationCanceledException {ex.Message}");
      }
      catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
      {
        Debug.WriteLine($"--Ignore AggregateException>>OperationCanceledException {ex.InnerException.Message}");
      }
    }

    void start()
    {
      Visibility = Visibility.Visible;
      _sbGravity.Begin();
    }
    async void stop()
    {
      _sbFadeOut.Begin();
      await Task.Delay(_fadeOuDuration.TimeSpan); // stop animation and remove artifacts from the visual tree upon completion of the fade out.
      _sbGravity.Stop();
      await Task.Delay(3000);
      Visibility = Visibility.Collapsed; // eliminate CPU engagement
    }
  }
}
