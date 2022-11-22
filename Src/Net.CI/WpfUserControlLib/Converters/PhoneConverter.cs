using System.Text.RegularExpressions;

namespace WpfUserControlLib.Converters;

public class PhoneConverter : MarkupExtension, IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (value is string str && !string.IsNullOrEmpty(str)) ? Regex.Replace(str, @"(\d{3})(\d{3})(\d{4})", "$1-$2-$3") : value;
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => "";
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public PhoneConverter() { }
}
