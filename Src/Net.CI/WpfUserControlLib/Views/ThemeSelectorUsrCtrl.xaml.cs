namespace WpfUserControlLib.Views;

public partial class ThemeSelectorUsrCtrl : UserControl
{
  public delegate void ApplyThemeDelegate(string v, string? cmn);
  public ThemeSelectorUsrCtrl() => InitializeComponent();
  public ApplyThemeDelegate? ThemeApplier { get; set; }
  public static readonly DependencyProperty CurThemeProperty = DependencyProperty.Register("CurTheme", typeof(string), typeof(ThemeSelectorUsrCtrl)); public string CurTheme { get => (string)GetValue(CurThemeProperty); set => SetValue(CurThemeProperty, value); }
  public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(ThemeSelectorUsrCtrl), new PropertyMetadata(" ð ")); public string Header { get => (string)GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }
  public static readonly DependencyProperty FontFmlProperty = DependencyProperty.Register("FontFml", typeof(FontFamily), typeof(ThemeSelectorUsrCtrl), new PropertyMetadata(new FontFamily("Wingdings 2"))); public FontFamily FontFml { get => (FontFamily)GetValue(FontFmlProperty); set => SetValue(FontFmlProperty, value); }
  public static readonly DependencyProperty FontSzeProperty = DependencyProperty.Register("FontSze", typeof(int), typeof(ThemeSelectorUsrCtrl), new PropertyMetadata(26)); public int FontSze { get => (int)GetValue(FontSzeProperty); set => SetValue(FontSzeProperty, value); }

  public void SetCurThemeToMenu(string theme)
  {
    foreach (MenuItem? item in ((ItemsControl)menu1.Items[0]).Items)
      if (item != null)
        item.IsChecked = theme?.Equals(item.Tag.ToString(), StringComparison.OrdinalIgnoreCase) ?? false;
  }

  void onChangeTheme(object s, RoutedEventArgs e) => ThemeApplier?.Invoke(((Button)s)?.Tag?.ToString() ?? "No Theme", "");
  void onSelectionChanged(object s, SelectionChangedEventArgs e)
  {
    if (e?.AddedItems?.Count > 0 && ThemeApplier != null)
      ThemeApplier(CurTheme = ((FrameworkElement)((object[])e.AddedItems)[0]).Tag?.ToString() ?? "No Theme?", "" );
  }
  void onMenuClick(object s, RoutedEventArgs e)
  {
    ThemeApplier?.Invoke(CurTheme = ((FrameworkElement)s).Tag?.ToString() ?? "No Theme", "");

    SetCurThemeToMenu(CurTheme);
  }
}
