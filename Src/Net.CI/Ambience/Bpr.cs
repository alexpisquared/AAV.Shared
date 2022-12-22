namespace AmbienceLib;

public partial class Bpr : IBpr
{
  public bool SuppressTicks { get; set; }
  public bool SuppressAlarm { get; set; }

  public void Beep(int hz, double sec) => Task.Run(async () => await BeepAsync(hz, sec).ConfigureAwait(false)); public async Task BeepAsync(int hz, double sec) => await BeepHzMks(new[] { FFD(hz, (int)(sec * 1000_000)) }).ConfigureAwait(false);
  public void Beep(int hz, double sec, ushort vol) => Task.Run(async () => await BeepAsync(hz, sec, vol).ConfigureAwait(false)); public async Task BeepAsync(int hz, double sec, ushort vol) => await BeepHzMks(new[] { FFD(hz, (int)(sec * 1000_000)) }, vol).ConfigureAwait(false);

  public void Tick() => Task.Run(TickAsync); public async Task TickAsync() { if (!SuppressTicks) await BeepHzMks(new[] { FFD(400, _dMin) }).ConfigureAwait(false); }
  public void Click() => Task.Run(ClickAsync); public async Task ClickAsync() { if (!SuppressTicks) await BeepHzMks(new[] { FFD(800, _dMin) }).ConfigureAwait(false); }
  public void Warn() => Task.Run(WarnAsync); public async Task WarnAsync() { if (!SuppressAlarm) await BeepHzMks(new[] { _fdw1, _fdw2 }).ConfigureAwait(false); }
  public void Error() => Task.Run(ErrorAsync); public async Task ErrorAsync() { if (!SuppressAlarm) await BeepHzMks(new[] { _fd21, _fd21, _fd21, _fd23 }).ConfigureAwait(false); }

  public void Start(int stepHz = 4) => Task.Run(async () => await StartAsync(stepHz).ConfigureAwait(false));   /**/ public async Task StartAsync(int stepHz = 4) => await GradientAsync(300, 1100, stepHz).ConfigureAwait(false); // 4 - 300 ms
  public void Finish(int stepHz = 4) => Task.Run(async () => await FinishAsync(stepHz).ConfigureAwait(false)); /**/ public async Task FinishAsync(int stepHz = 4) => await GradientAsync(1100, 300, stepHz).ConfigureAwait(false); // 300 ms
  public void AppStart() => Task.Run(AppStartAsync);   /**/ public async Task AppStartAsync() => await GradientAsync(200, 600, 2).ConfigureAwait(false);  // 600 ms

  public void AppFinish() => Task.Run(AppFinishAsync); /**/ public async Task AppFinishAsync() => await GradientAsync(799, 100, 1).ConfigureAwait(false); // 2.0 s (failed: 800 ) An unhandled exception of type 'System.AccessViolationException' occurred in System.Windows.Extensions.dll     Attempted to read or write protected memory.This is often an indication that other memory is corrupt.

  //was  void No() => Task.Run(async () => await NoAsync().ConfigureAwait(false)); public async Task NoAsync() => await BeepHzMks(new[] { FFD(6000, _dMin), FFD(5000, _dMin) }).ConfigureAwait(false); <== auto converted by the VS!!! to this:
  public void Yes() => Task.Run(YesAsync); public async Task YesAsync() => await BeepHzMks(new[] { FFD(4000, _dMin * 2), FFD(6000, _dMin) }).ConfigureAwait(false); // Oh, Yes.
  public void No() => Task.Run(NoAsync); public async Task NoAsync() => await BeepHzMks(new[] { FFD(5000, _dMin), FFD(50, _dMin / 2), FFD(4000, _dMin * 2) }).ConfigureAwait(false); // Hell, No.

  public void Enter() => Task.Run(EnterAsync); public async Task EnterAsync() => await BeepHzMks(new[] { FFD(1000, _dMin) }).ConfigureAwait(false);
  public void Exit() => Task.Run(ExitAsync); public async Task ExitAsync() => await BeepHzMks(new[] { FFD(1000, _dMin) }).ConfigureAwait(false);

  public async Task WaveAsync4k7k2() => await WaveAsync(4000, 7000, 2);
  public async Task WaveAsync3k8k4() => await WaveAsync(5000, 7000, 4); // quick  ~200ms
  public async Task WaveAsync7k5k1() => await WaveAsync(7000, 5000, 1); // longer ~400ms
  public async Task WaveAsync(int fromHz = 100, int tillHz = 300, int stepHz = 4) // 1sec
  {
    List<int[]> vs = new();

    if (fromHz < tillHz) //   /\
    {
      for (var hz = fromHz; hz < tillHz; hz += stepHz) { vs.Add(FixDuration(hz, 1)); } //   /

      for (var hz = tillHz; hz > fromHz; hz -= stepHz) { vs.Add(FixDuration(hz, 1)); } //   \
    }
    else                 //   \/
    {
      for (var hz = fromHz; hz > tillHz; hz -= stepHz) { vs.Add(FixDuration(hz, 1)); } //   \

      for (var hz = tillHz; hz < fromHz; hz += stepHz) { vs.Add(FixDuration(hz, 1)); } //   /
    }

    var started = Stopwatch.GetTimestamp();
    await BeepHzMks(vs.ToArray()).ConfigureAwait(false);
    WriteLine($"TrWL:> ::>>   from:{fromHz} - till:{tillHz} = {Math.Abs(fromHz - tillHz)} / step:{stepHz} ==> {vs.Count} steps. === {Stopwatch.GetElapsedTime(started).TotalMilliseconds} ms");
  }
  public async Task GradientAsync(int fromHz = 100, int tillHz = 300, int stepHz = 4) // 1sec
  {
    List<int[]> vs = new();

    if (fromHz < tillHz) //   /\
    {
      for (var hz = fromHz; hz < tillHz; hz += stepHz) { vs.Add(FixDuration(hz, 1)); } //   /
    }
    else                 //   \/
    {
      for (var hz = fromHz; hz > tillHz; hz -= stepHz) { vs.Add(FixDuration(hz, 1)); } //   \
    }

    var started = Stopwatch.GetTimestamp();
    await BeepHzMks(vs.ToArray()).ConfigureAwait(false);
    WriteLine($"TrWL:> ::>>   from:{fromHz} - till:{tillHz} = {Math.Abs(fromHz - tillHz)} / step:{stepHz} ==> {vs.Count} steps. === {Stopwatch.GetElapsedTime(started).TotalMilliseconds} ms");
  }

  public static async Task DevDbg() //  public App()  {    AmbienceLib.Bpr.DevDbg(); // ...
  {
    if (Debugger.IsAttached)
    {
      var bpr = new Bpr();
      while (true)
      {
        await bpr.StartAsync(2).ConfigureAwait(false); await Task.Delay(800);
        await bpr.FinishAsync(2).ConfigureAwait(false); await Task.Delay(800);

        await bpr.StartAsync(4).ConfigureAwait(false); await Task.Delay(600);
        await bpr.FinishAsync(4).ConfigureAwait(false); await Task.Delay(600);

        await bpr.StartAsync(8).ConfigureAwait(false); await Task.Delay(400);
        await bpr.FinishAsync(8).ConfigureAwait(false); await Task.Delay(400);

        await bpr.StartAsync(12).ConfigureAwait(false); await Task.Delay(200);
        await bpr.FinishAsync(12).ConfigureAwait(false); await Task.Delay(200);
        //await bpr.StartAsync(1).ConfigureAwait(false); 

        //////await bpr.WaveAsync(100, 300, 4);
        //////await bpr.WaveAsync(100, 300, 4);
        //////await bpr.WaveAsync(100, 300, 4);

        //////await bpr.WaveAsync(150, 51, 3);
        //////await bpr.WaveAsync(300, 100, 10);
        ////await bpr.WaveAsync(141, 100, 7);
        ////await bpr.WaveAsync(60, 101, 7); // noiseless pair

        ////bpr.Tick(); await Task.Delay(333);

        ////await bpr.TickAsync(); await Task.Delay(333);

        ////bpr.Click(); await Task.Delay(333);

        //var f = 5000;
        //var d = .075;
        //await bpr.BeepAsync(f, d);
        //await Task.Delay(300);

        //await bpr.TickAsync();
        //await Task.Delay(300);

        //await bpr.ClickAsync();
        //await Task.Delay(300);

        //WriteLine($"**************** {f}   {d}");
      }
    }
  }

  public static int[] FixDuration(int hz, int mks) // making sure whole wavelengths are sent to play.
  {
    var preciseTimesWavePlayed = mks * .000001 * hz;
    var roundedTimesWavePlayed = Math.Round(preciseTimesWavePlayed);

    if (roundedTimesWavePlayed <= 0) // make sure at least one wave is played.
      roundedTimesWavePlayed = 1;

    return new int[] { hz, (int)(1000_000 * roundedTimesWavePlayed / hz) };
  }

  public int[] FFD(int hz, int mks = _dMin) => FixDuration(hz, mks);
}