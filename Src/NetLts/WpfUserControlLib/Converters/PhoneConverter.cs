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
      9 => Brushes.Pink,        // International
      _ => Brushes.Gray         // Not found
    } : Brushes.Fuchsia;        // null or empty
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => "";
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public PhoneBrushConverter() { }
}

public class CountryBrushConverter : MarkupExtension, IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is string str && !string.IsNullOrEmpty(str) && GenderApiConst.Retries.Contains(str) ?
    Brushes.Gray :
    (value switch
    {
      "India" or "Sri Lanka" or "Bangladesh" or "Nepal" or "Bhutan" or "Maldives" or "Pakistan"
        => Brushes.OrangeRed,
      "South Korea" or "North Korea" or "Mongolia" or "Vietnam" or "Laos" or "Cambodia" or "Thailand" or "Myanmar" or "Malaysia" or "Brunei" or "Philippines" or "Indonesia" or "East Timor" or "Papua New Guinea" or "Fiji" or "Solomon Islands" or "Vanuatu" or "New Caledonia" or "French Polynesia" or "Samoa" or "Tonga" or "Tuvalu" or "Kiribati" or "Marshall Islands" or "Palau" or "Micronesia" or "Nauru"
        => Brushes.DarkCyan,
      "China" or "Japan" or "Singapore" or "Australia" or "New Zealand"
        => Brushes.Cyan,
      "Italy" or "Spain" or "Portugal" or "Greece" or "Turkey" or "Cyprus" or "Malta" or "Croatia" or "Bosnia and Herzegovina" or "Montenegro" or "Albania" or "Serbia" or "Kosovo" or "North Macedonia" or "Slovenia" or "Austria" or "Switzerland" or "Liechtenstein" or "Romania" or "Bulgaria" or "Moldova" or "Hungary" or "Slovakia" or "Czech Republic" or "Poland" or "Lithuania" or "Latvia" or "Estonia" or "Finland" or "Sweden" or "Norway" or "Denmark" or "Iceland" or "Faroe Islands" or "Greenland" or "Belarus" or "Ukraine" or "Russia" or "Georgia" or "Armenia" or "Azerbaijan" or "Kazakhstan" or "Uzbekistan" or "Turkmenistan" or "Tajikistan" or "Kyrgyzstan" or "Afghanistan" or "Iran" or "Iraq" or "Syria" or "Lebanon" or "Jordan"
        => Brushes.GreenYellow,
      "Canada" or "Germany" or "United States" or "United Kingdom" or "France" or "Netherlands" or "Belgium" or "Luxembourg" or "Ireland" or "Monaco" or "Andorra" or "San Marino" or "Vatican City" or "Gibraltar" or "Isle"
        => Brushes.Lime,
      _ => Brushes.DarkGray
    });

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => "";
  public override object ProvideValue(IServiceProvider serviceProvider) => this;
  public CountryBrushConverter() { }
}
