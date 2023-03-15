using CliWrap;
using System.Text;
using System.Threading;

namespace WpfUserControlLib.Services;
public class ClickOnceUpdater
{
  readonly string _deplTrgExe, _deplSrcDir, _deplTrgDir, _robocopy = "robocopy.exe";
  readonly ILogger _lgr;

  public ClickOnceUpdater(ILogger logger)
  {
    _lgr = logger;

    //if (Environment.GetCommandLineArgs().Length < 5)
    //{
    //  _deplSrcDir = DeplConstSpm.DeplSrcDir;
    //  _deplTrgDir = DeplConstSpm.DeplTrgDir;
    //  _deplTrgExe = DeplConstSpm.DeplTrgExe;
    //  _lgr.Log(LogLevel.Trace, $"{string.Join('\n', Environment.GetCommandLineArgs())} \t\t ==> Using hardcoded defaults for IPM");
    //}
    //else
    {
      _deplSrcDir = Environment.GetCommandLineArgs()[2];
      _deplTrgDir = Environment.GetCommandLineArgs()[3];
      _deplTrgExe = Environment.GetCommandLineArgs()[4];
      _lgr.Log(LogLevel.Trace, $"{string.Join('\n', Environment.GetCommandLineArgs().Skip(1))} \t\t <== SrcDir TrgDir TrgExe ==> Automatic args-based execution mode");
    }
  }    public async Task CopyAndLaunch(Action<string> ReportProgress)
  {
    try
    {
      ReportProgress("Copying...");
      await CopyZtoCDrive();
      ReportProgress("Copying done!"); await Task.Delay(2000);

      if (Environment.GetCommandLineArgs().Last() == "skipLaunchExit")
      {
        ReportProgress($"Launch+Exit skipped due to \nargs.Last()=='{Environment.GetCommandLineArgs().Last()}'");
        return;
      }

      ReportProgress("Re-Starting the Tool..."); await Task.Delay(2000);
await      LaunchFromCDrive();

      ReportProgress("Copying done! \n\n   Finalizing...");

      Close();
      Application.Current?.Shutdown();
    }
    catch (Exception ex) { _ = MessageBox.Show(ex.Message, "Warning / Error", MessageBoxButton.OK, MessageBoxImage.Error); }
  }

  async Task CopyZtoCDrive_()
  {
    try
    {
      _lgr.Log(LogLevel.Trace, "Copying...                                \n\n"); await Task.Delay(2000);

      using var processA = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = "robocopy.exe",
          //guments = $"{_deplSrcDir}  {_deplTrgDir} /XF *.config /MIR /NJH /NDL /NP /W:3",
          Arguments = $"{_deplSrcDir}  {_deplTrgDir}              /MIR /NJH /NDL /NP /W:3",
          WindowStyle = ProcessWindowStyle.Normal,
          RedirectStandardOutput = true,
          //RedirectStandardError = true,
          UseShellExecute = false
        }
      };
      _ = processA.Start();
      _lgr.Log(LogLevel.Trace, processA.StandardOutput.ReadToEnd());
      await processA.WaitForExitAsync(); //TMI: var output = processA.StandardOutput.ReadToEnd();      Logger.LogInformation(output);  :TMI

      using var processB = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = "robocopy.exe",
          Arguments = @$"{_deplSrcDir}  ""{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar""  *.lnk /XO",
          WindowStyle = ProcessWindowStyle.Normal,
          RedirectStandardOutput = true,
          //RedirectStandardError = true,
          UseShellExecute = false
        }
      };
      _ = processB.Start();
      var output = processB.StandardOutput.ReadToEnd();
      _lgr.Log(LogLevel.Trace, output);
      await processB.WaitForExitAsync();

      if (!processB.HasExited) //redundant here ..but useful for other scenarios
      {
        _ = processB.CloseMainWindow();
        processB.Close();
        processB.Kill();
      }
    }
    catch (Exception ex) { _ = MessageBox.Show(ex.Message, "Warning / Error", MessageBoxButton.OK, MessageBoxImage.Error); }
  }
  async Task CopyZtoCDrive()
  {
    try
    {
      _lgr.Log(LogLevel.Trace, "Copying... \n\n"); await Task.Delay(2000);

      var r1 = await Run(_robocopy, new[] { _deplSrcDir, _deplTrgDir, "/MIR", "/NJH", "/NDL", "/NP", "/W:3" });
      _lgr.Log(LogLevel.Trace, r1.rv + r1.er);


      var r2 = await Run(_robocopy, new[] { $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar", "*.lnk", "/XO"});
      _lgr.Log(LogLevel.Trace, r2.rv + r2.er);
    }
    catch (Exception ex) { _ = MessageBox.Show(ex.Message, "Warning / Error", MessageBoxButton.OK, MessageBoxImage.Error); }
  }
  async Task LaunchFromCDrive()
  {
    try
    {
      _lgr.Log(LogLevel.Trace, $"Launching {_deplTrgExe}... \n\n"); 

      var r1 = await Run(_deplTrgExe, new[] {  "none" });
      _lgr.Log(LogLevel.Trace, r1.rv + r1.er);
    }
    catch (Exception ex) { _ = MessageBox.Show(ex.Message, "Warning / Error", MessageBoxButton.OK, MessageBoxImage.Error); }
  }
  void LaunchFromCDrive_()
  {
    try
    {
      using var process = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = _deplTrgExe,
          Arguments = "None",
          WindowStyle = ProcessWindowStyle.Normal,
          RedirectStandardOutput = true,
          RedirectStandardError = true,
          UseShellExecute = false
        }
      };
      _ = process.Start();
      _ = process.WaitForExit(2500);
    }
    catch (Exception ex) { _ = MessageBox.Show(ex.Message, "Warning / Error", MessageBoxButton.OK, MessageBoxImage.Error); }
  }


  async Task<(bool success, string rv, string er, TimeSpan runTime)> Run(string exe, string[] ee, int timeoutmin = 3) // https://www.youtube.com/watch?v=Pt-0KM5SxmI&t=418s
  {
    StringBuilder sbOut = new(), sbErr = new();

    using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(timeoutmin));

    try
    {
      var commandResult = await Cli.Wrap(exe)
        .WithArguments(ee)
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
