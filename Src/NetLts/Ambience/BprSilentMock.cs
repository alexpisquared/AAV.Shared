//namespace AmbienceLib;

//public class BprSilentMock : IBpr
//{{
//  public bool SuppressTicks { get; set; }
//  public bool SuppressAlarm { get; set; }

//  public void Beep(int hz, double sec) { }
//  public async Task BeepAsync(int hz, double sec, bool isAsync) => await Task.Yield();
//  public async Task BeepHzMks(int[][] HzMks, ushort volume = 21845, bool isAsync = true) => await Task.Yield();
//  public async Task BeepAsync(int hz, double sec) => await Task.Yield();
//  public async Task BeepHzMks(int[][] HzMks, bool isAsync) => await Task.Yield();
//  public void Click() { }
//  public async Task ClickAsync() => await Task.Yield();
//  public void Enter() { }
//  public void Warn() { }
//  public async Task WarnAsync() => await Task.Yield();
//  public void Error() { }
//  public async Task ErrorAsync() => await Task.Yield();
//  public void Exit() { }
//  public int[] FFD(int hz, int durationMks = 100111) => Array.Empty<int>();
//  public void Finish(int stepHz = 4, ushort vol = ushort.MaxValue / 33) { }
//  public async Task FinishAsync(int stepHz = 4, ushort vol = ushort.MaxValue / 33) => await Task.Yield();
//  public void No() { }
//  public async Task NoAsync() => await Task.Yield();
//  public void Start(int stepHz = 4, ushort vol = ushort.MaxValue / 33) { }
//  public async Task StartAsync(int stepHz = 4, ushort vol = ushort.MaxValue / 33) => await Task.Yield();
//  public void Tick() { }
//  public async Task TickAsync() => await Task.Yield();
//  public async Task WaveAsync(int baseHz = 100, int plusHz = 300, int step = 4) => await Task.Yield();
//  public void Yes() { }
//  public async Task YesAsync() => await Task.Yield();
//  public void AppFinish() { }
//  public async Task AppFinishAsync() => await Task.Yield();
//  public void AppStart() { }
//  public async Task AppStartAsync() => await Task.Yield();
//  public void Beep(int hz, double sec, ushort vol) { }
//  public async Task BeepAsync(int hz, double sec, ushort vol) => await Task.Yield();
//  public async Task BeepHzMks(int[][] HzMks) => await Task.Yield();
//  public async Task EnterAsync() => await Task.Yield();
//  public async Task ExitAsync() => await Task.Yield();
//  public async Task GradientAsync(int fromHz = 100, int tillHz = 300, int stepHz = 4, int mks = 1, ushort vol = ushort.MaxValue) => await Task.Yield();
//  public async Task WaveAsync3k8k4() => await Task.Yield();
//  public async Task WaveAsync4k7k2() => await Task.Yield();
//  public async Task WaveAsync7k5k1() => await Task.Yield();
//  public void AppStart(ushort vol) => WinAPI.Beep(3456, 1200);
//  public void AppFinish(ushort vol) => WinAPI.Beep(3456, 1200);
//  public Task AppStartAsync(ushort vol) => throw new NotImplementedException("@888");
//  public Task AppFinishAsync(ushort vol) => throw new NotImplementedException("@888");
//  public void Beep1of2() => throw new NotImplementedException();
//  public void Beep2of2() => throw new NotImplementedException();
//  public static Task DevDbg() => throw new NotImplementedException();
//  public static int[] FixDuration(int hz, int mks) => throw new NotImplementedException();
//  public Task AreYouSureAsync() => throw new NotImplementedException();
//  public Task Bulk100_600() => throw new NotImplementedException();
//  public Task BulkWhistle() => throw new NotImplementedException();
//  public Task Bulk_250_500_110ms() => throw new NotImplementedException();
//  public Task Bulk_500_1000_110ms() => throw new NotImplementedException();
//  public Task ChockingUhmAsync() => throw new NotImplementedException();
//  public void NuHuh() => throw new NotImplementedException();
//  public Task NuHuhAsync() => throw new NotImplementedException();
//  public Task OhWowAsync() => throw new NotImplementedException();
//  public Task Tuk500_100() => throw new NotImplementedException();
//  public Task Tuk505_50() => throw new NotImplementedException();
//  public Task TwitAsync() => throw new NotImplementedException();
//  public void UhHuh() => throw new NotImplementedException();
//  public Task UhHuhAsync() => throw new NotImplementedException();
//  public Task WakeAudio() => throw new NotImplementedException();
//  public Task<string> Wave2Async(int[] freqs, int[] stepsHz) => throw new NotImplementedException();
//  public Task Wh_WhatAsync() => throw new NotImplementedException();
//  public Task WovAsync() => throw new NotImplementedException();
//  public Task YouSureAsync() => throw new NotImplementedException();
//}}