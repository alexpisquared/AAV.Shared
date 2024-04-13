namespace WpfUserControlLib.Helpers;

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