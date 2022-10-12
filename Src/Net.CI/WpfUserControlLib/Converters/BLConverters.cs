namespace WpfUserControlLib.Converters;

public class DropListConverter : MarkupExtension, IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    try
    {
      if (!(value is string str && !string.IsNullOrEmpty(str)))
        return Brushes.Gray;

      return
        str.Equals("A", StringComparison.OrdinalIgnoreCase) ? Brushes.Green :
        str.Equals("U", StringComparison.OrdinalIgnoreCase) ? Brushes.Blue :
        str.Equals("D", StringComparison.OrdinalIgnoreCase) ? Brushes.Red :
        str.StartsWith("#") ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(str)) : // ColorRGB
        Brushes.Brown;
    }
    catch
    {
      return Brushes.Red;
    }
  }
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => "";
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public DropListConverter() { }
}
