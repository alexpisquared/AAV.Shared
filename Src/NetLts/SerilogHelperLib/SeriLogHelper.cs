using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
namespace SerilogHelperLib;

public class SeriLogHelper
{
  public static Microsoft.Extensions.Logging.ILogger CreateLogger<T>(LogLevel minLogLevel = LogLevel.Information, string? assemblyName = null) =>
    LoggerFactory.Create(builder =>
    {
      LoggerConfiguration loggerConfig = Debugger.IsAttached
          ? new LoggerConfiguration().WriteTo.Debug().MinimumLevel.Verbose()
          : new LoggerConfiguration()
              .MinimumLevel.Verbose()
              .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
              .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
              .Enrich.FromLogContext();

      _ = loggerConfig.WriteTo.File(
          path: GetLogFilePath(assemblyName ?? Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown"),
          rollingInterval: RollingInterval.Infinite,
          outputTemplate: "{Timestamp:MM-dd HH:mm:ss.fff} {Level:w3} {Message}  {Exception}{NewLine}",
          formatProvider: new CultureInfo("en-US"),
          restrictedToMinimumLevel: (Serilog.Events.LogEventLevel)minLogLevel);

      _ = builder.AddSerilog(loggerConfig.CreateLogger());
    }).CreateLogger<T>();

  static string GetLogFilePath(string assemblyName)
  {
    var basePath = Constants.IsDbg ? $@"\temp\bak\Logs" : $@"C:\Users\{Environment.UserName}\OneDrive\Public\Logs\";

    var fileName = Constants.IsDbg
        ? $"{assemblyName}-{DateTime.Now:MM.dd·HH.mm.ss.f}.log"
        : $"{assemblyName}.{DateTime.Now:MMdd.HHmm}.log";

    return Path.Combine(basePath, fileName);
  }
}
