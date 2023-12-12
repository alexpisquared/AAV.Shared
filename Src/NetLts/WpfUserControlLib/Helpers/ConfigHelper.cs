namespace WpfUserControlLib.Helpers;
public class ConfigHelper //todo: appsettings as user prefs: https://makolyte.com/csharp-how-to-update-appsettings-json-programmatically/
{
  public static IConfigurationRoot AutoInitConfigFromFile(bool enforceCreation = false)
  {
    const int maxTries = 2;
    int tryCntr;

    foreach (var appsettingsFile in new[] {
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @$"AppSettings\{AppDomain.CurrentDomain.FriendlyName}\{_appSettingsFileNameOnly}"),
        @$"C:\Temp\Publish\{Assembly.GetExecutingAssembly().GetName().Name}\Config\{_appSettingsFileNameOnly}",
        @$"AppSettings\{_appSettingsFileNameOnly}",
        @$"{_appSettingsFileNameOnly}",
      })
    {
      for (tryCntr = 0; tryCntr < maxTries && TryCreateDefaultFile(appsettingsFile, enforceCreation); tryCntr++)
      {
        try
        {
          return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(appsettingsFile)
            .AddUserSecrets<WhatIsThatForType>() //note: of this assenbly!!! // :overrides value from the appsettings file above, but only for debug?local?etc. runs, right?
            .Build();
        }
        catch (InvalidOperationException ex)
        {
          ex.Pop();
        }
        catch (FileNotFoundException ex)
        {
          if (!TryCreateDefaultFile(appsettingsFile, enforceCreation))
            _ = ex.Log($"Retrying {maxTries} times ...");
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

      new Exception().Pop($"Unable to create default  {appsettingsFile}  file  {tryCntr}/{maxTries} times.");
    }

    return AutoInitConfigHardcoded();
  }
  public static IConfigurationRoot AutoInitConfigHardcoded(string? sqlServerCSV = null, string? dtBsNameCSV = null)
  {
    var svr = DevOps.IsDevMachineH ? @".\SqlExpress" : "mtDEVsqldb,1625";
    var cfg = new ConfigurationBuilder()
      .AddInMemoryCollection()
      .AddUserSecrets<WhatIsThatForType>() //note: of this assenbly!!!
      .Build();

    cfg[CfgName.WhereAmAy] = "HardCODE in .\\WpfUserControlLib\\Helpers\\ConfigHelper.cs";
    cfg[CfgName.ServerLst] = sqlServerCSV ?? StandardLib.Base.Consts.SqlServerCSV;
    cfg[CfgName.DtBsNmLst] = dtBsNameCSV ?? StandardLib.Base.Consts.DtBsNameCSV;
    cfg[CfgName.LogFolder] = VersionHelper.IsDbg ? @"C:\Temp\Logs\..log" : @"\\oak\cm\felixdev\apps\data\Oleksa\Tooling\SmreV2\bin\Logs\..log";
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
        WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] ■ WARNING: overwrote this {appsettingsPathFileExt}.");
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

  const string
    _appSettingsFileNameOnly = "AppSettings.json", //tu: will be places to C:\ProgramData\AppSettings\[MinNavTpl]\
    _cs11_demo0 = "demo",
    _cs11_demo1 = $$"""
    {
      "name": "{{_cs11_demo0}}"
    }
    """,
    _cs11_demo2 = $$"""
    {
      "name": "{0}"
    }
    """,
    _defaultAppSetValues = @"{{
      ""WhoAmI"":               ""{1}"",
      ""WhereAmI"":             ""{0}"",
      ""LogFolder"":            ""C:\\Temp\\Logs\\WpfUsrCtrlLib..log"",
      ""ServerLst"":            "".\\sqlexpress .\\sqlexpress"",
      ""DtBsNmLst"":            ""QStatsDbg QStatsRls"",
      ""NoteToConsider0"":      ""it is better to wait forever 00 than to fail a broadcast:"",
      ""SqlConStrFormat"":      ""Server={{0}};Database={{1}};Trusted_Connection=True;Encrypt=False;Connection Timeout=00;"", 
      ""SqlConStrForma_"":      ""Server={{0}};Database={{1}};persist security info=True;user id={{2}};password={{3}};MultipleActiveResultSets=True;App=EntityFramework;Connection Timeout=47"",
      ""SubDetails"": {{
        ""KeyVaultURL"":        ""<moved to a safe place>"",
        ""PreCreatedAt"":       ""{2}""
      }}
}}"; // let's wait for 9000 sec (15 min) lest fail a scheduled distribution.
}
  
class WhatIsThatForType { public string MyProperty { get; set; } = "<Default Value of Nothing Special>"; }

public class ConfigRandomizer
{
  readonly Random _random = new(DateTime.Now.Millisecond);
  readonly IConfigurationRoot _config;

  public ConfigRandomizer(string jsonFile = "appsettings.json") => _config = new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      //.AddJsonFile(jsonFile)                                            // the last overwrites!!!
      .AddUserSecrets<WhatIsThatForType>() //note: of this assenbly!!!    // the last overwrites!!!
      .Build();

  public IConfigurationRoot Config => _config;
  public string GetValue(string name) => _config[name] ?? "";
  public string GetRandomFromUserSection(string section)
  {
    var sn = $"{section}_{Environment.UserName}";
    var sa = _config.GetSection(sn).Get<string[]>();
    if (sa == null)
    {
      WriteLine($"\"{sn}\": [\"abc\", \"efg\", \"hij\"],      // <== if (sc?.Value == null)");
      return $"Section  '{section}'  not found in app settings nor secrets";
    }

    return sa[_random.Next(sa.Length)];
  }
}