using CliWrap;

namespace StandardLib.Helpers;
public class AltProcessRunner
{
  public static async Task<(bool success, string rv, string er, TimeSpan runTime)> RunAsync(string exe, string[]? args = null, int timeoutmin = 3) // https://www.youtube.com/watch?v=Pt-0KM5SxmI&t=418s
  {
    StringBuilder sbOut = new(), sbErr = new();

    using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(timeoutmin));

    try
    {
      var commandResult = await Cli.Wrap(exe) //tu: process.start alternative
        .WithArguments(args ?? [])
        //.WithValidation(CommandResultValidation.None) gives error .. could by since no path.
        .WithStandardOutputPipe(PipeTarget.ToStringBuilder(sbOut))
        .WithStandardErrorPipe(PipeTarget.ToStringBuilder(sbErr))
        .ExecuteAsync(cts.Token);

      return (true, sbOut.ToString().Trim('\n').Trim('\r'), sbErr.ToString(), commandResult.RunTime);
    }
    catch (OperationCanceledException ex)
    {
      return (false, "", ex.Message, TimeSpan.MinValue);
    }
    catch (Exception ex)
    {
      return (false, "", ex.Message, TimeSpan.MinValue);
    }
  }
  public static async Task<(bool success, string rv, string er, TimeSpan runTime)> RunAsync(string exe, CancellationTokenSource cts, string[]? args = null, int timeoutmin = 3) // https://www.youtube.com/watch?v=Pt-0KM5SxmI&t=418s
  {
    StringBuilder sbOut = new(), sbErr = new();

    try
    {
      var commandResult = await Cli.Wrap(exe) //tu: process.start alternative
        .WithArguments(args ?? [])
        //.WithValidation(CommandResultValidation.None) gives error .. could by since no path.
        .WithStandardOutputPipe(PipeTarget.ToStringBuilder(sbOut))
        .WithStandardErrorPipe(PipeTarget.ToStringBuilder(sbErr))
        .ExecuteAsync(cts.Token);

      return (true, sbOut.ToString().Trim('\n').Trim('\r'), sbErr.ToString(), commandResult.RunTime);
    }
    catch (OperationCanceledException ex)
    {
      return (false, "", ex.Message, TimeSpan.MinValue);
    }
    catch (Exception ex)
    {
      return (false, "", ex.Message, TimeSpan.MinValue);
    }
  }
}
