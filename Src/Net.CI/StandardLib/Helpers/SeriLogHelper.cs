using Serilog;

namespace StandardLib.Helpers;

public class SeriLogHelper
{
  public static ILoggerFactory InitLoggerFactory(string folder, string levels = "+Verbose -Info +Warning +Error +ErNT -11mb -Infi") => LoggerFactory.Create(builder =>
  {
    WriteLine($"TrWL:/> {folder}\nTrWL:/> {folder.Replace("..", ".ERR..")}");

    var loggerConfiguration =
      Debugger.IsAttached ?
        new LoggerConfiguration().WriteTo.Debug().MinimumLevel.Verbose() :
        new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext(); // .Enrich.WithMachineName().Enrich.WithThreadId()                                       

    if (levels.Contains("+ErNT")) loggerConfiguration.WriteTo.File(path: @$"{folder.Replace("..", ".ErNT..")}", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error); // ErNT == Error No Template.
    if (levels.Contains("+Erro")) loggerConfiguration.WriteTo.File(path: @$"{folder.Replace("..", ".Er▄▀..")}", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error, outputTemplate: _exnOnlyTemplate);
    if (levels.Contains("+Warn")) loggerConfiguration.WriteTo.File(path: @$"{folder.Replace("..", ".Warn..")}", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning, outputTemplate: _optimalTemplate);
    if (levels.Contains("+Info")) loggerConfiguration.WriteTo.File(path: @$"{folder.Replace("..", ".Info..")}", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, outputTemplate: _optimalTemplate);
    if (levels.Contains("+Verb")) loggerConfiguration.WriteTo.File(path: @$"{folder.Replace("..", ".Verb..")}", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose, outputTemplate: _optimalTemplate);
    if (levels.Contains("+Infi")) loggerConfiguration.WriteTo.File(path: @$"{folder.Replace("..", ".Infi..")}", rollingInterval: RollingInterval.Infinite);
    if (levels.Contains("+11mb")) loggerConfiguration.WriteTo.File(path: @$"{folder.Replace("..", ".11mb..").Replace(".log", ".json")}", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose, rollOnFileSizeLimit: true, fileSizeLimitBytes: 11000000, formatter: new Serilog.Formatting.Json.JsonFormatter()); // useful only with log aggregators.

    _ = builder.AddSerilog(loggerConfiguration.CreateLogger());
  });
  public static ILoggerFactory InitLoggerFactory() => LoggerFactory.Create(builder => // :mostly for unit testing.
  {
    var loggerConfiguration = new LoggerConfiguration().WriteTo.Debug().MinimumLevel.Information();

    _ = builder.AddSerilog(loggerConfiguration.CreateLogger());
  });

  const string
    _optimalTemplate = "{Timestamp:HH:mm:ss.fff} [{Level:w3}]   {Message}   {Exception}{NewLine}", // slightly better than no template: time format + 1 line save.  
    _exnOnlyTemplate = "{Timestamp:HH:mm:ss.fff}   {Exception}{NewLine}"; //tu: _exnOnlyTemplate outputTemplate is better than sans-outputTemplate message: it contains actual line of throwing (not the catch line!!!)
}
/*  static Logger ConfigSerilogger()
  {
    #region Serilog -- https://stackoverflow.com/questions/59362461/logging-in-net-core-wpf-application
    //main: https://github.com/serilog/serilog/blob/dev/README.md
    //iatc: https://www.youtube.com/watch?v=_iryZxv8Rxw
    //todo: cool idea to sink into UI: https://stackoverflow.com/questions/35567814/is-it-possible-to-display-serilog-log-in-the-programs-gui
    //also https://dzone.com/articles/serilog-tutorial-for-net-logging-16-best-practices
    //also https://github.com/sstorie/SerilogDemo.Wpf/blob/develop/SerilogDemo.Wpf/App.xaml.cs

    var serilogILogger = new LoggerConfiguration()
      //-- .ReadFrom.Configuration(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build())
      .MinimumLevel.Verbose()
      .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
      .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
      .Enrich.FromLogContext() // .Enrich.WithMachineName().Enrich.WithThreadId()
      .WriteTo.Debug(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
      .WriteTo.File(path: @"C:\temp\logs\log-.txt", outputTemplate: _template, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, rollingInterval: RollingInterval.Minute)
      .WriteTo.File(path: @"C:\temp\logs\MaxLen-11k.json", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, rollOnFileSizeLimit: true, fileSizeLimitBytes: 11000, formatter: new Serilog.Formatting.Json.JsonFormatter())
      //.WriteTo.ColoredConsole(outputTemplate: outputTemplate)
      .CreateLogger();
    Log.Logger = serilogILogger; // for Log.Fatal("...");
    #endregion
    return serilogILogger;
  }
* ...or this:
{
"Serilog": {
  "Using": [],
  "MinimumLevel": "Verbose",
  "Override": {
    "Microsoft": "Warning",
    "System": "Warning"
  },
  "Enrich": [ "FromLogContext" ], //, "WithMachineName", "WithThreadId"
  "WriteTo": [
    {
      "Name": "Debug",
      "Args": {
        "restrictedToMinimumLevel": "Error" // show errors+fatals in the output window.
      }
    },
    {
      "Name": "File",
      "Args": {
        "path": "C:\\temp\\logs\\log-.txt",
        "rollingInterval": "Minute",
        "outputTemplate": "{Timestamp:HH:mm:ss.fff} {Level:w3} {Message:j}\t{Properties}\t{NewLine:1}{Exception:1}"
      }
    },
    {
      "Name": "File",
      "Args": {
        "path": "C:\\temp\\logs\\MaxLen-11k.json",
        "formatter": "Serilog.Formatting.Json.JsonFormatter, Serilog",
        "rollOnFileSizeLimit": true,
        "fileSizeLimitBytes": 11000
      }
    }
  ]
}
}

*/