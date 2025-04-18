namespace AmbienceLib;

public partial class Bpr : IBpr
{
  const int _startFinishQuieter = 33, _appStartFinishQuieter = 11;
  public bool SuppressTicks { get; set; }
  public bool SuppressAlarm { get; set; }

  public void Beep(int hz, double sec) => Task.Run(async () => await BeepAsync(hz, sec).ConfigureAwait(false)); public async Task BeepAsync(int hz, double sec) => await BeepHzMks(new[] { FFD(hz, (int)(sec * 1000_000)) }).ConfigureAwait(false);
  public void Beep(int hz, double sec, ushort vol) => Task.Run(async () => await BeepAsync(hz, sec, vol).ConfigureAwait(false)); public async Task BeepAsync(int hz, double sec, ushort vol) => await BeepHzMks(new[] { FFD(hz, (int)(sec * 1000_000)) }, volume: vol).ConfigureAwait(false);

  public void Tick() => Task.Run(TickAsync); public async Task TickAsync() { if (!SuppressTicks) await BeepHzMks([FFD(300, _dMin)]).ConfigureAwait(false); }
  public void Click() => Task.Run(ClickAsync); public async Task ClickAsync() { if (!SuppressTicks) await BeepHzMks([FFD(500, _dMin)]).ConfigureAwait(false); }
  public void Warn() => Task.Run(WarnAsync); public async Task WarnAsync() { if (!SuppressAlarm) await BeepHzMks(_Beethoven6thWarn/*new[] { _fdw1, _fdw2 }*/).ConfigureAwait(false); }
  public void Error() => Task.Run(ErrorAsync); public async Task ErrorAsync() { if (!SuppressAlarm) await BeepHzMks(_Beethoven6thErr).ConfigureAwait(false); }

  public void Start(int stepHz = 4, ushort vol = ushort.MaxValue / _startFinishQuieter) => Task.Run(async () => await StartAsync(stepHz, vol).ConfigureAwait(false));   /**/ public async Task StartAsync(int stepHz = 4, ushort vol = ushort.MaxValue / _startFinishQuieter) => await GradientAsync(500, 1100, stepHz, vol: vol).ConfigureAwait(false); // 4 - 300 ms
  public void Finish(int stepHz = 4, ushort vol = ushort.MaxValue / _startFinishQuieter) => Task.Run(async () => await FinishAsync(stepHz, vol).ConfigureAwait(false)); /**/ public async Task FinishAsync(int stepHz = 4, ushort vol = ushort.MaxValue / _startFinishQuieter) => await GradientAsync(1100, 500, stepHz, vol: vol).ConfigureAwait(false); // 300 ms
  public void AppStart(ushort vol = ushort.MaxValue / _appStartFinishQuieter) => Task.Run(() => AppStartAsync(vol));   /**/ public async Task AppStartAsync(ushort vol = ushort.MaxValue / _appStartFinishQuieter) => await GradientAsync(90, 508, stepHz: 2, vol: vol).ConfigureAwait(false);  // 0.8 s
  public void AppFinish(ushort vol = ushort.MaxValue / _appStartFinishQuieter) => Task.Run(() => AppFinishAsync(vol)); /**/ public async Task AppFinishAsync(ushort vol = ushort.MaxValue / _appStartFinishQuieter) => await GradientAsync(610, 51, stepHz: 1, vol: vol).ConfigureAwait(false); // 1.2 s for step=2           pass on RAZER, others give: An unhandled exception of type 'System.AccessViolationException' occurred in System.Windows.Extensions.dll     Attempted to read or write protected memory.This is often an indication that other memory is corrupt.)

  //was  void No() => Task.Run(async () => await NoAsync().ConfigureAwait(false)); public async Task NoAsync() => await BeepHzMks(new[] { FFD(6000, d1), FFD(5000, d1) }).ConfigureAwait(false); <== auto converted by the VS!!! to this:
  public void No() => Task.Run(NoAsync); public async Task NoAsync() => await BeepHzMks(new[] { FFD(500, _dMin), FFD(50, _dMin / 2), FFD(400, _dMin * 2) }).ConfigureAwait(false); // Hell, No.
  public void Yes() => Task.Run(YesAsync); public async Task YesAsync() => await BeepHzMks(new[] { FFD(400, _dMin * 2), FFD(600, _dMin) }).ConfigureAwait(false); // Oh, Yes.

  public void UhHuh() => Task.Run(UhHuhAsync); public async Task UhHuhAsync()
  {
    await FinishAsync(16).ConfigureAwait(false);
    await StartAsync(16).ConfigureAwait(false);
  }
  public void NuHuh() => Task.Run(NuHuhAsync); public async Task NuHuhAsync()
  {
    await StartAsync(16).ConfigureAwait(false);
    await FinishAsync(16).ConfigureAwait(false);
  }

  public void Enter() => Task.Run(EnterAsync); public async Task EnterAsync() => await BeepHzMks(new[] { FFD(1000, _dMin) }).ConfigureAwait(false);
  public void Exit() => Task.Run(ExitAsync); public async Task ExitAsync() => await BeepHzMks(new[] { FFD(1000, _dMin) }).ConfigureAwait(false);

  public async Task WaveAsync4k7k2() => await WaveAsync(4000, 7000, 2);
  public async Task WaveAsync3k8k4() => await WaveAsync(5000, 7000, 4); // quick  ~200ms
  public async Task WaveAsync7k5k1() => await WaveAsync(7000, 5000, 1); // longer ~400ms
  public async Task<string> Wave2Async(int[] freqs, int[] stepsHz)
  {
    List<int[]> vs = [];
    string report = "";

    for (var i = 0; i < freqs.Length - 1; i++)
    {
      var fromHz = freqs[i + 0];
      var tillHz = freqs[i + 1];
      var stepHz = stepsHz[i % stepsHz.Length];

      if (fromHz < tillHz)
        for (var hz = fromHz; hz < tillHz; hz += stepHz) { vs.Add(FixDuration(hz, 1)); } //   /
      else
        for (var hz = fromHz; hz > tillHz; hz -= stepHz) { vs.Add(FixDuration(hz, 1)); } //   \

      report += $"{DateTime.Now:HH:mm:ss.fff}       Hz: {fromHz,6} ->{tillHz,6} = {Math.Abs(fromHz - tillHz),6} /{stepHz,3} Hz step   ==>{vs.Count,5} steps. \n";
    }

    var started = Stopwatch.GetTimestamp();
    await BeepHzMks(vs.ToArray(), isAsync: false).ConfigureAwait(false);

    report += $"                                                        {Stopwatch.GetElapsedTime(started).TotalSeconds:N2} sec";
    Console.ForegroundColor = ConsoleColor.DarkCyan; Console.WriteLine(report); Console.ResetColor();
    return report;
  }
  public async Task WaveAsync(int fromHz = 100, int tillHz = 300, int stepHz = 4) // 1sec
  {
    List<int[]> vs = [];

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
    await BeepHzMks(vs.ToArray(), isAsync: false).ConfigureAwait(false);
    //tmi: Trace_(fromHz, tillHz, stepHz, vs, started);
  }

  public async Task GradientAsync(int fromHz = 100, int tillHz = 300, int stepHz = 4, int mks = 1, ushort vol = ushort.MaxValue) // 1sec
  {
    List<int[]> vs = [];

    if (fromHz < tillHz) //   /\
    {
      for (var hz = fromHz; hz < tillHz; hz += stepHz) { vs.Add(FixDuration(hz, mks)); } //   /
    }
    else                 //   \/
    {
      for (var hz = fromHz; hz > tillHz; hz -= stepHz) { vs.Add(FixDuration(hz, mks)); } //   \
    }

    var started = Stopwatch.GetTimestamp();
    await BeepHzMks(HzMks: vs.ToArray(), volume: vol, isAsync: false).ConfigureAwait(false);
    //tmi: Trace_(fromHz, tillHz, stepHz, vs, started);
  }
  public async Task WakeAudio() => await BeepAsync(1, .1);
  public int[] FixDuration(int hz, int mks) // making sure whole wavelengths are sent to play.
  {
    var preciseTimesWavePlayed = mks * .000_001 * hz;
    var roundedTimesWavePlayed = Math.Round(preciseTimesWavePlayed);

    if (roundedTimesWavePlayed <= 0) // make sure at least one wave is played.
      roundedTimesWavePlayed = 1;

    return [hz, (int)(1_000_000 * roundedTimesWavePlayed / hz)];
  }
  public int[] FFD(int hz, int mks = _dMin) => FixDuration(hz, mks);
  static void Trace_(int fromHz, int tillHz, int stepHz, List<int[]> vs, long started) => WriteLine($"{DateTime.Now:yy-MM-dd HH:mm:ss.f}            Bpr.Wave()   Hz: {fromHz,4} -> {tillHz,4} = {Math.Abs(fromHz - tillHz)} / step:{stepHz} Hz ==> {vs.Count} steps. ==> {Stopwatch.GetElapsedTime(started).TotalSeconds:N2} sec");

  // LickCreator results:
  public async Task BulkWhistle() => await Wave2Async([200, 3000, 1000, 8000], [15, 15, 15]);
  public async Task Bulk_500_1000_110ms() => await Wave2Async([500, 1000], [20]);
  public async Task Bulk_250_500_110ms() => await Wave2Async([250, 500], [20]);
  public async Task Bulk100_600() => await Wave2Async([100, 600], [20]); // 0.19 sec
  public async Task Tuk500_100() => await Wave2Async([500, 100], [20]); // 0.19 sec
  public async Task Tuk505_50() => await Wave2Async([505, 50], [5]); // 0.52 sec
  public async Task Wh_WhatAsync() => await Wave2Async([200, 1000, 100, 4000], [16]); // 0.54 sec  	 2025-04-17 09:38
  public async Task OhWowAsync() => await Wave2Async([1000, 100, 4000, 200], [16]); // 0.61 sec  	 2025-04-17 09:40
  public async Task WovAsync() => await Wave2Async([80, 800, 160], [16]); // 0.31 sec  	 2025-04-17 09:43
  public async Task TwitAsync() => await Wave2Async([6555, 1111, 3555], [16]); // 0.24 sec  	 2025-04-17 09:45
  public async Task ChockingUhmAsync() => await Wave2Async([655, 111, 355], [16]); // 0.26 sec  	 2025-04-17 09:49
  public async Task YouSureAsync() => await Wave2Async([5000, 800, 6000, 500], [16]); // 0.46 sec  	 2025-04-17 09:54
  public async Task AreYouSureAsync() => await Wave2Async([4000, 600, 5000, 500, 4000, 300], [16]); // 0.77 sec  	 2025-04-17 09:56
  public async Task AreYouSure2kAsync() => await Wave2Async([2000, 300, 2500, 250, 2000, 200], [16]); // 0.76 sec  	 2025-04-18 10:25
  public async Task AreYouSure1kAsync() => await Wave2Async([33, 1000, 150, 1100, 150, 1000, 100], [16]); // 0.92 sec  	 2025-04-18 10:28
  public void Beep1of2() => throw new NotImplementedException();
  public void Beep2of2() => throw new NotImplementedException();
}