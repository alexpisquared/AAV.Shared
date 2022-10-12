using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CI.Standard.Lib.Views
{
  public partial class ThemeToggleUsrCtrl : UserControl
  {
    public delegate void ApplyThemeDelegate(string v);
    public ThemeToggleUsrCtrl() => InitializeComponent();
    public ApplyThemeDelegate? ThemeApplier { get; set; }
    public static readonly DependencyProperty CurThemeProperty = DependencyProperty.Register("CurTheme", typeof(string), typeof(ThemeToggleUsrCtrl)); public string CurTheme { get => (string)GetValue(CurThemeProperty); set => SetValue(CurThemeProperty, value); }
    public void SetCurThemeToMenu(string thm) => tgl1.IsChecked = thm == "Lite.Gray";

    void onChangeTheme(object s, RoutedEventArgs e) => ThemeApplier?.Invoke(((Button)s)?.Tag?.ToString() ?? "No Theme");
    void onToggleTheme(object s, RoutedEventArgs e) => ThemeApplier?.Invoke(CurTheme = ((ToggleButton)s).IsChecked == false ? "Lite.Bootstrap" : "Lite.Gray");
  }
}
