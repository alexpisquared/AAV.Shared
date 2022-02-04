using System;

namespace AsLink
{
  public class EnvCanRadarUrlHelper //2021: this is the single central intel used/shared by all radar consumers, including: ToRunOr, AlexPi.Scr, etc. True on 2022-02-04.
  {
    public static string GetRadarUrl(DateTime d) => $"https://dd.meteo.gc.ca/radar/PRECIPET/GIF/WKR/{d:yyyyMMddHHmm}_WKR_COMP_PRECIPET_RAIN.gif";  
    public static string GetRadarUrl_NotRadarFallback(DateTime d) => $"https://weather.gc.ca/data/satellite/goes_ecan_1070_m_{d.Year}@{d.Month:0#}@{d.Day:0#}_{d.Hour:0#}h{d.Minute:0#}m.jpg";  // <===========================================
    public static string GetRadarUrl_OLD(DateTime d, string rsRainOrSnow /*= "RAIN"*/, string station /*= "WKR"*/, bool isFallbackCAPPI, bool isFallbackCOMP = false)
    {
      var cmp = isFallbackCOMP ? "_COMP" : "";
      var cap = isFallbackCAPPI ? "CAPPI" : "PRECIP";
      var drk = IsDark ? "" : "detailed/";

      return $"https://weather.gc.ca/data/radar/{drk}temp_image//{station}/{station}{cmp}_{cap}_{rsRainOrSnow}_{d.Year}_{d.Month:0#}_{d.Day:0#}_{d.Hour:0#}_{d.Minute:0#}.GIF";
    }

    public static DateTime RoundBy10min(DateTime dt) => dt.AddTicks(-dt.Ticks % (10 * TimeSpan.TicksPerMinute));
    public static bool IsDark => DateTime.Now.Hour < 8 || 20 <= DateTime.Now.Hour;
  }
}
/*
2021-11-21
 https://eccc-msc.github.io/open-data/msc-data/obs_radar/readme_radar_en/
 https://eccc-msc.github.io/open-data/msc-data/obs_radar/readme_radarimage-datamart_en/
Apparently, the old style GIFs are still available, but at the new location/format:
 https://dd.meteo.gc.ca/radar/PRECIPET/GIF/WKR/     every 10 min
 https://dd.meteo.gc.ca/radar/PRECIPET/GIF/CASKR/   every 6/10 min
 https://dd.meteo.gc.ca/radar/DPQPE/GIF/CASKR/      every 6 min

2021-04-05
Latest cloud cover image: ir, vis, ir+vis:
https://weather.gc.ca/data/satellite/goes_ecan_visible_100.jpg
https://weather.gc.ca/data/satellite/goes_ecan_1070_100.jpg
https://weather.gc.ca/data/satellite/goes_ecan_vvi_100.jpg

timestamped: ir, vis, ir+vis:
https://weather.gc.ca/data/satellite/goes_ecan_1070_m_2021@04@04_02h10m.jpg                                                             // <===========================================
https://weather.gc.ca/data/satellite/goes_ecan_visible_m_2021@04@03_10h30m.jpg
https://weather.gc.ca/data/satellite/goes_ecan_vvi_m_2021@04@03_10h30m.jpg

Old RADAR stuff:
https://weather.gc.ca/data/radar/detailed/temp_image//WKR/WKR_COMP_PRECIP_SNOW_2021_03_12_23_50.GIF
https://weather.gc.ca/data/radar/temp_image//WKR/WKR_PRECIP_SNOW_2021_03_12_23_50.GIF
https://weather.gc.ca/data/radar/         temp_image//WKR/WKR_     PRECIP_SNOW_2021_03_12_21_00.GIF

https://weather.gc.ca/data/radar/detailed/temp_image//WKR/WKR_COMP_PRECIP_SNOW_2021_03_12_23_50.GIF
https://weather.gc.ca/data/radar/detailed/temp_image//WKR/WKR_comp_PRECIP_SNOW_2021_03_12_23_50.GIF

*/