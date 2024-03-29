﻿namespace WpfUserControlLib.Converters;
public class QStatsConverter : MarkupExtension, IValueConverter
{
  public bool IsInverted { get; set; } = false;
  readonly Brush
     _b0 = new SolidColorBrush(Color.FromRgb(0, 0, 128)),
     _b6 = new SolidColorBrush(Color.FromRgb(80, 80, 210)),
     _b7 = new SolidColorBrush(Color.FromRgb(0, 160, 160)),
     _b8 = new SolidColorBrush(Color.FromRgb(80, 160, 80)),
     _b9 = new SolidColorBrush(Color.FromRgb(255, 200, 80)),
     _bA = new SolidColorBrush(Color.FromRgb(255, 255, 255));

  public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is double valInt && targetType == typeof(Brush))
    {
      const int min = 700, max = 850, ddd = max - min;
      return valInt switch
      {
        < min => new SolidColorBrush(Color.FromRgb(0, 0, 128)),
        > max => new SolidColorBrush(Color.FromRgb(255, 255, 0)),
        _ => new SolidColorBrush(Color.FromRgb((byte)(255.0 * (valInt - min) / ddd), 0, 0)),
      };
    }
    else if (value is string valStr)
      switch (valStr)
      {
        case "offline":
          if (targetType == typeof(Brush)) return Brushes.White;
          else if (targetType == typeof(bool)) return false;
          else if (targetType == typeof(Visibility)) return Visibility.Collapsed;
          else return null;
        case "online":
          if (targetType == typeof(Brush)) return new BrushConverter().ConvertFromString("#00ff00");//new LinearGradientBrush(Colors.Green, Colors.LightGreen, 0);//
          else if (targetType == typeof(bool)) return true;
          else if (targetType == typeof(Visibility)) return Visibility.Visible;
          else return null;
        case "DND":
          if (targetType == typeof(Brush)) return new LinearGradientBrush(new GradientStopCollection() { new GradientStop(Colors.Red, 0.0), new GradientStop(Colors.Red, 0.44), new GradientStop(Colors.White, 0.45), new GradientStop(Colors.White, 0.55), new GradientStop(Colors.Red, 0.56), new GradientStop(Colors.Red, 1.0) }, 90);//
          else if (targetType == typeof(bool)) return true;
          else if (targetType == typeof(Visibility)) return Visibility.Visible;
          else return null;
        case "page-me":
          if (targetType == typeof(Brush)) return new BrushConverter().ConvertFromString("#ff8000");//new LinearGradientBrush(Colors.Green, Colors.LightGreen, 0);//
          else if (targetType == typeof(bool)) return true;
          else if (targetType == typeof(Visibility)) return Visibility.Visible;
          else return null;
        case "sign-in":
        case "sign-out":
        case "change-state":
        case "state":
        case "disconnecting":
        case "shutting-down":
        case "acquiring-network":
        case "connecting":
        case "synchronizing":
        case "no-network":
        case "no-service":
        case "quit":
        case "terminate": return Brushes.Black;
        default:
          if (targetType == typeof(Visibility)) return string.IsNullOrEmpty(valStr) && IsInverted ? Visibility.Visible : Visibility.Collapsed;
          else if (targetType == typeof(Brush)) return Brushes.Gray;
          else return Brushes.Gray;
      }
    else if (value is bool valBool)
      if (targetType == typeof(bool)) return valBool;
      else if (targetType == typeof(Brush))
        return valBool ? new LinearGradientBrush(Colors.Green, Colors.LightGreen, 0) : new LinearGradientBrush(Colors.DarkRed, Colors.Red, 0);//new BrushConverter().ConvertFromString("#00ff00");
      else if (targetType == typeof(Visibility))
        return valBool ? IsInverted ? Visibility.Collapsed : Visibility.Visible : IsInverted ? Visibility.Visible : Visibility.Collapsed;
      else if (targetType == typeof(FontWeight))
        return valBool ? IsInverted ? FontWeights.Normal : FontWeights.Bold : IsInverted ? FontWeights.Bold : FontWeights.Normal;
      else return null;
    else if (value is DateTime valDTm) //todo: dupe                                       // if (targetType == typeof(DateTime))
    {
      var va = valDTm;
      var dt = DateTime.Now - va;
      return
        dt.TotalSeconds < 10 ? $"Now!!!" :
        dt.TotalMinutes < 60 ? $"{va:HH:mm:ss}" :
        dt.TotalHours < 10.0 ? $"{va:HH:mm}" :
        dt.TotalDays < 3.000 ? $"{va:ddd HH:mm}" :
        dt.TotalDays < 183.0 ? $"{va:MMM-d}" :
        $"{va:yyyy-MMM}";
    }
    else if (value == null)
      return value;
    else
    {
      WriteLine($"**** Unprocesssed type: {value.GetType().Name} ");
      return value;
    }
  }
  public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public QStatsConverter() { }
}
