using System;
using System.Diagnostics;

namespace AsLink
{
  public class EnvCanRadarUrlHelper
  {
    public static DateTime RoundBy10min(DateTime dt) => dt.AddTicks(-dt.Ticks % (10 * TimeSpan.TicksPerMinute));
    public static string GetRadarUrl(DateTime d, string rsRainOrSnow /*= "RAIN"*/, string station /*= "WKR"*/, bool isFallbackCAPPI, bool? isDark /*= null*/, bool isFallbackCOMP = false)
    {
      isDark = isDark ?? IsDark; // isDark ??= IsDark; in UWP gives Error CS8370  Feature 'coalescing assignment' is not available in C# 7.3. Please use language version 8.0 or greater.	

      ///todo: on error try this _COMP_
      var cmp = isFallbackCOMP ? "_COMP" : "";
      var cap = isFallbackCAPPI ? "CAPPI" : "PRECIP";
      var drk = isDark == true ? "" : "detailed/";

      var url = $"https://weather.gc.ca/data/radar/{drk}temp_image//{station}/{station}{cmp}_{cap}_{rsRainOrSnow}_{d.Year}_{d.Month:0#}_{d.Day:0#}_{d.Hour:0#}_{d.Minute:0#}.GIF";

      return url;
    }

    public static bool IsDark { get => DateTime.Now.Hour < 8 || 20 <= DateTime.Now.Hour; }
  }
}
/*
2021-01-15

Latest cloud cover image: ir, vis, ir+vis:
https://weather.gc.ca/data/satellite/goes_ecan_1070_100.jpg
https://weather.gc.ca/data/satellite/goes_ecan_visible_100.jpg
https://weather.gc.ca/data/satellite/goes_ecan_vvi_100.jpg

https://weather.gc.ca/data/radar/detailed/temp_image//WKR/WKR_COMP_PRECIP_SNOW_2021_03_12_23_50.GIF
https://weather.gc.ca/data/radar/temp_image//WKR/WKR_PRECIP_SNOW_2021_03_12_23_50.GIF
https://weather.gc.ca/data/radar/         temp_image//WKR/WKR_     PRECIP_SNOW_2021_03_12_21_00.GIF

https://weather.gc.ca/data/radar/detailed/temp_image//WKR/WKR_COMP_PRECIP_SNOW_2021_03_12_23_50.GIF
https://weather.gc.ca/data/radar/detailed/temp_image//WKR/WKR_comp_PRECIP_SNOW_2021_03_12_23_50.GIF
*/