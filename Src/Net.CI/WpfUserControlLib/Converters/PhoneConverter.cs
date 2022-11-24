namespace WpfUserControlLib.Converters;

public class PhoneFormatConverter : MarkupExtension, IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (value is string str && !string.IsNullOrEmpty(str)) ? Regex.Replace(str, @"(\d{3})(\d{3})(\d{4})", "$1-$2-$3") : value;
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => "";
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public PhoneFormatConverter() { }
}

public class PhoneToolTipConverter : MarkupExtension, IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (value is string str && !string.IsNullOrEmpty(str)) ? AreaCodeValidator.GetToolTip(str) : value;
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => "";
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public PhoneToolTipConverter() { }
}

public class PhoneBrushConverter : MarkupExtension, IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (value is string str && !string.IsNullOrEmpty(str)) ?
    AreaCodeValidator.FirstOrDefault(str)?.Prio switch
    {
      1 => Brushes.Lime,        // TO
      2 => Brushes.GreenYellow,
      3 => Brushes.YellowGreen, // GTA
      4 => Brushes.Yellow,      // Next hr zone
      5 => Brushes.Green,       // Next 2 hr zone
      6 => Brushes.DarkCyan,    // Next 3 hr zone
      7 => Brushes.DarkCyan,
      8 => Brushes.DarkCyan,
      9 => Brushes.Pink,        // Intrnational
      _ => Brushes.Gray         // Not found
    }

    : value;
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => "";
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public PhoneBrushConverter() { }
}
