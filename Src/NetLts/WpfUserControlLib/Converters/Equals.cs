namespace WpfUserControlLib.Converters;
public class Equals : MarkupExtension, IValueConverter
{
  public Equals() { }

  public bool InvertValue { get; set; } = false;

  public string EqualsText { get; set; } = default!;
  public string NotEqualsText { get; set; } = default!;
  public Brush BrushTrue { get; set; } = default!;
  public Brush BrushFalse { get; set; } = default!;

  public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
  {
    bool result;

    if (value != null)
      result = InvertValue ? !value.Equals(parameter) : value.Equals(parameter);
    else
    {
      result = value == null && parameter == null;
      result = InvertValue ? !result : result;
    }

    if (targetType == typeof(Visibility))
      return result ? Visibility.Visible : Visibility.Collapsed;

    if (targetType == typeof(string))
      return result ? EqualsText : NotEqualsText;

    if (targetType == typeof(Brush))
      return result ? BrushTrue : BrushFalse;

    return result;
  }

  public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException("█ █ █ █ █ █ █ █ █ █");

  public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
