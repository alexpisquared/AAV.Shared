using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;
using System.Globalization;
using static System.Diagnostics.Trace;
namespace SerilogHelperLib;

public class SeriLogHelperMini
{
  static readonly string _logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Sigma", AppDomain.CurrentDomain.FriendlyName, $"{AppDomain.CurrentDomain.FriendlyName}{(Constants.IsDbg ? ".dbg" : "")}.log");
  public static Microsoft.Extensions.Logging.ILogger CreateLogger<T>() => InitLoggerFactory__().CreateLogger<T>();

  static ILoggerFactory InitLoggerFactory__() => LoggerFactory.Create(builder =>
  {
    WriteLine($"SeriLogHelper.InitLoggerFactory__: logFile: {_logFile}");

    LoggerConfiguration loggerConfiguration = Debugger.IsAttached ?
      new LoggerConfiguration().WriteTo.Debug().MinimumLevel.Verbose() :
      new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext(); // .Enrich.WithMachineName().Enrich.WithThreadId()                                       

    _ = loggerConfiguration.WriteTo.File(path: _logFile, rollingInterval: RollingInterval.Infinite, outputTemplate: "{Timestamp:MM-dd HH:mm:ss.fff} {Level:w3} {Message}  {Exception}{NewLine}", formatProvider: new CultureInfo("en-US"), restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose);

    _ = builder.AddSerilog(loggerConfiguration.CreateLogger());
  });
}
