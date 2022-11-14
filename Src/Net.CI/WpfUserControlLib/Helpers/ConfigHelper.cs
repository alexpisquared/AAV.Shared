using StandardContractsLib;

namespace WpfUserControlLib.Helpers;

public class ConfigHelper //todo:  appsettings as user prefs: https://makolyte.com/csharp-how-to-update-appsettings-json-programmatically/
{
  public static IConfigurationRoot AutoInitConfigFromFile(bool enforceCreation = false)
  {
    const int maxTries = 2;

    var appsettingsFiles = new[] {
        @$"Z:\Dev\_Redis_MTDEV\CI.IPM\Config\{_appSettingsFileNameOnly}",
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @$"AppSettings\{AppDomain.CurrentDomain.FriendlyName}\{_appSettingsFileNameOnly}"),
        @$"AppSettings\{_appSettingsFileNameOnly}",
        @$"{_appSettingsFileNameOnly}",
      };

    foreach (var appsettingsFile in appsettingsFiles)
    {
      int i;
      for (i = 0; i < maxTries && TryCreateDefaultFile(appsettingsFile, enforceCreation); i++)
      {
        try
        {
          return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(appsettingsFile)
            .AddUserSecrets<WhatIsThatForType>()
            .Build();
        }
        catch (InvalidOperationException ex)
        {
          ex.Pop();
        }
        catch (FileNotFoundException ex)
        {
          if (!TryCreateDefaultFile(appsettingsFile, enforceCreation))
            _ = ex.Log("Retrying 3 times ...");
        }
        catch (FormatException ex)
        {
          _ = new Process { StartInfo = new ProcessStartInfo("Notepad.exe", $"\"{appsettingsFile}\"") { RedirectStandardError = true, UseShellExecute = false } }.Start();
          ex.Pop($"Try to edit the errors out from \n\t {appsettingsFile}");
        }
        catch (Exception ex)
        {
          if (!TryCreateDefaultFile(appsettingsFile, enforceCreation))
            ex.Pop("██  ██  ██  Take a look!");
        }
      }

      new Exception().Pop($"Unable to create default  {appsettingsFile}  file  {i}/{maxTries} times.");
    }

    return AutoInitConfigHardcoded(enforceCreation);
  }
  public static IConfigurationRoot AutoInitConfigHardcoded(bool enforceCreation = false)
  {
    var svr = DevOps.IsDevMachineH ? @".\SqlExpress" : "mtDEVsqldb,1625";
    var cfg = new ConfigurationBuilder()
      .AddInMemoryCollection()            
      .AddUserSecrets<WhatIsThatForType>()
      .Build();

    cfg[CfgName.WhereAmAy] = "HardCODE in .\\WpfUserControlLib\\Helpers\\ConfigHelper.cs";
    cfg[CfgName.ServerLst] = StandardLib.Base.Consts.SqlServerCSV;
    cfg[CfgName.DtBsNmLst] = StandardLib.Base.Consts.DtBsNameCSV;
    cfg[CfgName.LogFolder] = VersionHelper.IsDbg ? @"C:\Temp\Logs\..log" : @"Z:\Dev\_Redis_MTDEV\BMS\IncomePaymentManagement\Logs\..log";
    cfg[CfgName.SqlVerIpm] = VersionHelper.IsDbg ? "Server={0};Database={1};Trusted_Connection=True;Encrypt=False;Connection Timeout=15;" : "Server={0};Database={1};Trusted_Connection=True;Encrypt=False;Connection Timeout=52;"; // On dbg 5 sec was fine .. let's see if 1 sec is going to cause YY's issues.
    cfg[CfgName.SqlVerIp_] = "Server={0};Database={1};persist security info=True;user id={2};password={3};MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=57";

#if !true
      var appConfig = new AppConfig();
      config.Bind(appConfig);
      string json = JsonConvert.SerializeObject(appConfig);
      File.WriteAllText("exported-" + _appSettingsFileNameOnly, json);
#endif

    return cfg;
  }

  const string
    _appSettingsFileNameOnly = "AppSettings.json",
    _cs11_demo = "demo",
    _cs11_demo1 = $$"""
    {
      "name": "{{_cs11_demo}}"
    }
    """,
    __cs11_demo2 = $$"""
    {
      "name": "{0}"
    }
    """,
    _defaultAppSetValues = @"{{
      ""WhoAmI"":               ""{1}"",
      ""WhereAmI"":             ""{0}"",
      ""LogFolder"":            ""Z:\\Dev\\_Redis_MTDEV\\BMS\\IncomePaymentManagement\\Logs\\..log"",
      ""ServerLst"":            ""mtDEVsqldb,1625 mtUATsqldb mtPRDsqldb .\\sqlexpress"",
      ""DtBsNmLst"":            ""QStatsDbg QStatsRls"",
      ""SqlConStrFormat"":      ""Server={{0}};Database={{1}};Trusted_Connection=True;Encrypt=False;Connection Timeout=41;"",
      ""SqlConStrForma_"":      ""Server={{0}};Database={{1}};persist security info=True;user id={{2}};password={{3}};MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=47"",
      ""SubDetails"": {{
        ""KeyVaultURL"":        ""<moved to a safe place>"",
        ""PreCreatedAt"":       ""{2}""
      }}
}}";
  static bool TryCreateDefaultFile(string appsettingsPathFileExt, bool enforceCreation)
  {
    try
    {
      var dir = Path.GetDirectoryName(appsettingsPathFileExt);
      if (string.IsNullOrEmpty(dir))
      {
        dir = "appsettings";
        appsettingsPathFileExt = Path.Combine(dir, appsettingsPathFileExt);
      }

      if (Directory.Exists(dir) != true)
        Directory.CreateDirectory(dir);

      if (enforceCreation || !File.Exists(appsettingsPathFileExt))
      {
        File.WriteAllText(appsettingsPathFileExt, string.Format(_defaultAppSetValues, appsettingsPathFileExt.Replace(@"\", @"\\"), Assembly.GetEntryAssembly()?.GetName().Name, DateTimeOffset.Now.ToString()));
        WriteLine($"TrWL:> ■ WARNING: overwrote this {appsettingsPathFileExt}.");
      }

      return true;
    }
    catch (FormatException ex)
    {
      _ = new Process { StartInfo = new ProcessStartInfo("Notepad.exe", $"\"{appsettingsPathFileExt}\"") { RedirectStandardError = true, UseShellExecute = false } }.Start();
      ex.Pop($"Try to edit the errors out from \n\t {appsettingsPathFileExt}");
      return false;
    }
    catch (Exception ex) { ex.Pop(); return false; }
  }
  class WhatIsThatForType { public string MyProperty { get; set; } = "<Default Value of Nothing Special>"; }
}
