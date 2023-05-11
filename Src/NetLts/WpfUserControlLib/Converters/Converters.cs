namespace WpfUserControlLib.Converters;

public class ReportToVisibilityConverter : MarkupExtension, IValueConverter
{
  public bool InvertValue { get; set; }
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    value is null ? Visibility.Hidden :
    value is string str && (str.Contains("Error:") || str.Contains("Warning:")) ? (InvertValue ? Visibility.Visible : Visibility.Collapsed) : (!InvertValue ? Visibility.Visible : Visibility.Collapsed);
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => false;
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public ReportToVisibilityConverter() { }
}
public class StringToColor : MarkupExtension, IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    var _err = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f48"));
    var _wrn = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f91"));
    var _inf = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4b1"));
    try
    {
      return value is string str && !string.IsNullOrEmpty(str) ?
        (str is not null && str.Contains("Error:")) ? _err :
        (str is not null && str.Contains("Warning:")) ? _wrn :
        (str is not null && str.StartsWith("#")) ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(str)) :
        _inf :
        parameter;
    }
    catch
    {
      return parameter;
    }
  }
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => "";
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public StringToColor() { }
}
public class BoolToColorConverter : MarkupExtension, IValueConverter
{
  public Color ColorTrue { get; set; } = Colors.LightGreen;
  public Color ColorFalse { get; set; } = Colors.Red;
  public bool InvertValue { get; set; }
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => new SolidColorBrush(((bool?)value) == null ? Colors.Transparent : ((bool?)value) == true ? ColorTrue : ColorFalse);
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => false;
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public BoolToColorConverter() { }
}
public class BoolToCharConverter : MarkupExtension, IValueConverter // FontFamily="Wingdings" FontSize="26"
{
  public bool InvertValue { get; set; }
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((bool?)value) == null ? " " : ((bool?)value) == true ? "ü" : "û";
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => false;
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public BoolToCharConverter() { }
}

public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
{
  public bool InvertValue { get; set; }
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((bool?)value) == null ? Visibility.Hidden :
    InvertValue ?
    ((bool?)value) != true ? Visibility.Visible : Visibility.Collapsed :
    ((bool?)value) == true ? Visibility.Visible : Visibility.Collapsed;
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => false;
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public BoolToVisibilityConverter() { }
}
public class NullToVisibilityConverter : MarkupExtension, IValueConverter
{
  public bool InvertValue { get; set; }
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    var rv = InvertValue ? (value is null) == true : (value is null) != true;
    //System.Diagnostics.WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] +++   val:{value}  param:'{parameter}'  invert:{InvertValue}   =>   {rv}   !!! ");
    return rv ? Visibility.Visible : Visibility.Collapsed;
  }
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => false;
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public NullToVisibilityConverter() { }
}
public class NullToBoolConverter : MarkupExtension, IValueConverter
{
  public bool InvertValue { get; set; }
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    var rv = InvertValue ? (value is null) == true : (value is null) != true;
    //System.Diagnostics.WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] +++   val:{value}  param:'{parameter}'  invert:{InvertValue}   =>   {rv}   !!! ");
    return rv;
  }
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => false;
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public NullToBoolConverter() { }
}
public class ToBoolConverter : MarkupExtension, IValueConverter
{
  public bool InvertValue { get; set; }
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    var rv = Type.GetTypeCode(value.GetType()) switch
    {
      TypeCode.Int32 => InvertValue ? (int)value == 0 : (int)value != 0,
      TypeCode.Int64 => InvertValue ? (long)value == 0 : (long)value != 0,
      TypeCode.Int16 => InvertValue ? (short)value == 0 : (short)value != 0,
      TypeCode.Boolean => InvertValue ? !(bool)value : (bool)value,
      _ => InvertValue ? value is null : value is not null
    };
    //System.Diagnostics.WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] +++   val:{value}  param:'{parameter}'  invert:{InvertValue}   =>   {rv}   !!! ");
    return rv;
  }
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => false;
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public ToBoolConverter() { }
}
public class BoolToOpacityConverter : MarkupExtension, IValueConverter
{
  public bool InvertValue { get; set; }
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((bool?)value) == null ? Visibility.Hidden :
    InvertValue ?
    ((bool?)value) != true ? 1 : .25 :
    ((bool?)value) == true ? 1 : .25;
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => false;
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public BoolToOpacityConverter() { }
}
public class IntToVisibilityConverter : MarkupExtension, IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((int?)value) == null ? Visibility.Collapsed : ((int?)value) != 0 ? Visibility.Visible : Visibility.Collapsed;
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => false;
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public IntToVisibilityConverter() { }
}
public class SmartDateConverter : MarkupExtension, IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is not DateTime dtVal)
      return "";

    var dt = DateTime.Now - dtVal;

    return
      dt.TotalSeconds < 10 ? $"Now!!!" :
      dt.TotalMinutes < 60 ? $"{dtVal:HH:mm:ss}" :
      dt.TotalHours < 10.0 ? $"{dtVal:HH:mm}" :
      dt.TotalDays < 3.000 ? $"{dtVal:ddd HH:mm}" :
      dt.TotalDays < 183.0 ? $"{dtVal:MMM-d}" :
                             $"{dtVal:yyyy-MMM}";
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => false;
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public SmartDateConverter() { }
}

public class UniMVConverter : MarkupExtension, IMultiValueConverter
{
  public bool InvertValue { get; set; }
  public string TextTrue { get; set; } = "Yes";
  public string TextFalse { get; set; } = "No";
  public Brush BrushTrue { get; set; } = Brushes.Green;
  public Brush BrushFalse { get; set; } = Brushes.DarkRed;
  public string Match { get; set; } = default!;

  public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
  {
    if (values.Length == 2) // assume equality checker: equal=>true.
    {
      if (string.IsNullOrEmpty(values[0]?.ToString()) && string.IsNullOrEmpty(values[1]?.ToString())) return !InvertValue; // true if both are empty/null

      var value = values[0];

      // other then 18 TypeCode types below (must go first since case TypeCode.Object: takes over everything
      if (targetType == typeof(Visibility))
      {
        return Type.GetTypeCode(values[0]?.GetType()) switch
        {
          TypeCode.String => string.IsNullOrEmpty(Match) ?
          InvertValue ? (string.IsNullOrEmpty((string)value) ? Visibility.Visible : Visibility.Collapsed) : (!string.IsNullOrEmpty((string)value) ? Visibility.Visible : Visibility.Collapsed) :
          InvertValue ? ((string)value != Match ? Visibility.Visible : Visibility.Collapsed) : ((string)value == Match ? Visibility.Visible : Visibility.Collapsed),
          TypeCode.Int32 => InvertValue ? ((int)value == 0 ? Visibility.Visible : Visibility.Collapsed) : ((int)value != 0 ? Visibility.Visible : Visibility.Collapsed),
          TypeCode.Int64 => InvertValue ? ((long)value == 0 ? Visibility.Visible : Visibility.Collapsed) : ((long)value != 0 ? Visibility.Visible : Visibility.Collapsed),
          _ => InvertValue ? Visibility.Collapsed : Visibility.Visible
        };
      }
      else if (targetType == typeof(Brush))
      {
        return Type.GetTypeCode(value.GetType()) switch
        {
          TypeCode.Int32 => InvertValue ? ((int)value == 0 ? BrushTrue : BrushFalse) : ((int)value != 0 ? BrushTrue : BrushFalse),
          TypeCode.Int64 => InvertValue ? ((long)value == 0 ? BrushTrue : BrushFalse) : ((long)value != 0 ? BrushTrue : BrushFalse),
          TypeCode.Boolean => InvertValue ? ((bool)value ? BrushFalse : BrushTrue) : ((bool)value ? BrushTrue : BrushFalse),
          _ => Brushes.YellowGreen
        };
      }

      switch (Type.GetTypeCode(targetType))
      {
        case TypeCode.Boolean:
          return Type.GetTypeCode(value.GetType()) switch
          {
            TypeCode.Int32 => InvertValue ? (int)value == 0 : (int)value != 0,
            TypeCode.Int64 => InvertValue ? (long)value == 0 : (long)value != 0,
            TypeCode.Int16 => InvertValue ? (short)value == 0 : (short)value != 0,
            TypeCode.Boolean => InvertValue ? !(bool)value : (bool)value,
            TypeCode.String => string.IsNullOrEmpty(Match) ?
            (InvertValue ? string.IsNullOrEmpty((string)value) : !string.IsNullOrEmpty((string)value)) :
                            (InvertValue ? ((string)value) != Match : (string)value == Match),
            //TypeCode.Empty => throw new NotImplementedException(),
            //TypeCode.DBNull => throw new NotImplementedException(),
            //TypeCode.Char => throw new NotImplementedException(),
            //TypeCode.SByte => throw new NotImplementedException(),
            //TypeCode.Byte => throw new NotImplementedException(),
            //TypeCode.UInt16 => throw new NotImplementedException(),
            //TypeCode.UInt32 => throw new NotImplementedException(),
            //TypeCode.UInt64 => throw new NotImplementedException(),
            //TypeCode.Single => throw new NotImplementedException(),
            //TypeCode.Double => throw new NotImplementedException(),
            //TypeCode.Decimal => throw new NotImplementedException(),
            //TypeCode.DateTime => throw new NotImplementedException(),
            _ => InvertValue ? value is null : value is not null
          };
        case TypeCode.String:
          return Type.GetTypeCode(value.GetType()) switch
          {
            TypeCode.Int32 => InvertValue ? ((int)value == 0 ? TextTrue : TextFalse) : ((int)value != 0 ? TextTrue : TextFalse),
            TypeCode.Int64 => InvertValue ? ((long)value == 0 ? TextTrue : TextFalse) : ((long)value != 0 ? TextTrue : TextFalse),
            TypeCode.Int16 => InvertValue ? ((short)value == 0 ? TextTrue : TextFalse) : ((short)value != 0 ? TextTrue : TextFalse),
            TypeCode.Boolean => InvertValue ? !(bool)value : (bool)value,
            _ => InvertValue ? value is null : value is not null
          };
        case TypeCode.Object:                                             // so far only ToolTip => returning string.
          return Type.GetTypeCode(value.GetType()) switch
          {
            TypeCode.Int32 => InvertValue ? ((int)value == 0 ? TextTrue : TextFalse) : ((int)value != 0 ? TextTrue : TextFalse),
            TypeCode.Int64 => InvertValue ? ((long)value == 0 ? TextTrue : TextFalse) : ((long)value != 0 ? TextTrue : TextFalse),
            TypeCode.Int16 => InvertValue ? ((short)value == 0 ? TextTrue : TextFalse) : ((short)value != 0 ? TextTrue : TextFalse),
            TypeCode.Boolean => InvertValue ? !(bool)value : (bool)value,
            _ => InvertValue ? value is null : value is not null
          };
        case TypeCode.Empty:
        case TypeCode.DBNull:
        case TypeCode.Char:
        case TypeCode.SByte:
        case TypeCode.Byte:
        case TypeCode.Int16:
        case TypeCode.UInt16:
        case TypeCode.Int32:
        case TypeCode.UInt32:
        case TypeCode.Int64:
        case TypeCode.UInt64:
        case TypeCode.Single:
        case TypeCode.Double:
        case TypeCode.Decimal:
        case TypeCode.DateTime:
        default: Write($"TrcW:> --/> This targetType '{targetType.Name}' is not supported yet."); break;
      }
    }

    return false;
  }
  public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException("██  ██  ██");
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public UniMVConverter() { }
}
