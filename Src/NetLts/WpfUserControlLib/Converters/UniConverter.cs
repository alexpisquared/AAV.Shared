namespace WpfUserControlLib.Converters;
public class UniConverter : MarkupExtension, IValueConverter
{
  public bool InvertValue { get; set; }
  public string TextTrue { get; set; } = "Yes";
  public string TextFalse { get; set; } = "No";
  public Brush BrushTrue { get; set; } = Brushes.Green;
  public Brush BrushFalse { get; set; } = Brushes.DarkRed;
  public required string Match { get; set; } // = default!;

  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is null) // no type info in this case
    {
      if (targetType == typeof(Visibility)) return InvertValue ? Visibility.Visible : Visibility.Collapsed;
      if (targetType == typeof(string)) return InvertValue ? TextTrue : TextFalse;
      if (targetType == typeof(object)) return InvertValue ? TextTrue : TextFalse; // so far only ToolTip
      if (targetType == typeof(double)) return InvertValue ? 0.0 : 10.0; // Blur for IsBusy
      if (targetType == typeof(Brush)) return InvertValue ? BrushTrue : BrushFalse;
      if (targetType == typeof(bool)) return InvertValue;
      if (targetType == typeof(ImageSource)) return new DrawingImage(); // otherwise binding to null error.

      return InvertValue;
    }

    // other then 18 TypeCode types below (must go first since case TypeCode.Object: takes over everything
    if (targetType == typeof(Visibility))
    {
      return Type.GetTypeCode(value.GetType()) switch
      {
        TypeCode.String => string.IsNullOrEmpty(Match) ?
            InvertValue ? (string.IsNullOrEmpty((string)value) ? Visibility.Visible : Visibility.Collapsed) : (!string.IsNullOrEmpty((string)value) ? Visibility.Visible : Visibility.Collapsed) :
            InvertValue ? ((string)value != Match ? Visibility.Visible : Visibility.Collapsed) : ((string)value == Match ? Visibility.Visible : Visibility.Collapsed),
        TypeCode.Int32 => InvertValue ? ((int)value == 0 ? Visibility.Visible : Visibility.Collapsed) : ((int)value != 0 ? Visibility.Visible : Visibility.Collapsed),
        TypeCode.Int64 => InvertValue ? ((long)value == 0 ? Visibility.Visible : Visibility.Collapsed) : ((long)value != 0 ? Visibility.Visible : Visibility.Collapsed),
        TypeCode.Boolean => InvertValue ? ((bool)value ? Visibility.Collapsed : Visibility.Visible) : ((bool)value ? Visibility.Visible : Visibility.Collapsed),
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
    else if (targetType == typeof(ImageSource))
    {
      return value;
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
          TypeCode.Boolean => InvertValue ? ((bool)value == false ? TextTrue : TextFalse) : ((bool)value == false ? TextFalse : TextTrue),
          _ => InvertValue ? value is null : value is not null
        };
      case TypeCode.Object:                                             // so far only ToolTip => returning string.
        return Type.GetTypeCode(value.GetType()) switch
        {
          TypeCode.Int32 => InvertValue ? ((int)value == 0 ? TextTrue : TextFalse) : ((int)value != 0 ? TextTrue : TextFalse),
          TypeCode.Int64 => InvertValue ? ((long)value == 0 ? TextTrue : TextFalse) : ((long)value != 0 ? TextTrue : TextFalse),
          TypeCode.Int16 => InvertValue ? ((short)value == 0 ? TextTrue : TextFalse) : ((short)value != 0 ? TextTrue : TextFalse),
          TypeCode.Boolean => InvertValue ? ((bool)value == false ? TextTrue : TextFalse) : ((bool)value != false ? TextTrue : TextFalse), // ToolTip comes as object; take it as string ... see if it breaks anything (22-07-15).
          _ => InvertValue ? value is null : value is not null
        };
      case TypeCode.Double:
        return Type.GetTypeCode(value.GetType()) switch
        {
          TypeCode.Boolean => InvertValue ? ((bool)value ? 0.0 : 3.0) : ((bool)value ? 3.0 : 0.0), // Blur for IsBusy .. 10 was too harsh.
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
      case TypeCode.Decimal:
      case TypeCode.DateTime:
      default: Write($"TrcW:> --/> This targetType '{targetType.Name}' is not supported yet."); break;
    }

    return value;
  }
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => false;
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public UniConverter() { }
}
