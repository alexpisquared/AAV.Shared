﻿namespace AmbienceLib;

public class BprSilentMock : IBpr
{
  bool suppressTicks, suppressAlarm;

  public bool SuppressTicks { get => suppressTicks; set => suppressTicks = value; }
  public bool SuppressAlarm { get => suppressAlarm; set => suppressAlarm = value; }

  public void Beep(int hz, double sec) { }
  public async Task BeepAsync(int hz, double sec) => await Task.Yield();
  public async Task BeepHzMks(int[][] HzMks, ushort volume = 21845) => await Task.Yield();
  public void Click() { }
  public async Task ClickAsync() => await Task.Yield();
  public void Enter() { }
  public void Warn() { }
  public async Task WarnAsync() => await Task.Yield();
  public void Error() { }
  public async Task ErrorAsync() => await Task.Yield();
  public void Exit() { }
  public int[] FFD(int hz, int durationMks = 100111) => Array.Empty<int>();
  public void Finish() { }
  public async Task FinishAsync() => await Task.Yield();
  public void No() { }
  public async Task NoAsync() => await Task.Yield();
  public void Start() { }
  public async Task StartAsync() => await Task.Yield();
  public void Tick() { }
  public async Task TickAsync() => await Task.Yield();
  public async Task WaveAsync(int baseHz = 100, int plusHz = 300, int step = 4) => await Task.Yield();
  public void Yes() { }
  public async Task YesAsync() => await Task.Yield();
  public void AppFinish() {}
  public async Task AppFinishAsync() => await Task.Yield();
  public void AppStart() {}
  public async Task AppStartAsync() => await Task.Yield();
  public void Beep(int hz, double sec, ushort vol) {}
  public async Task BeepAsync(int hz, double sec, ushort vol) => await Task.Yield();
  public async Task BeepHzMks(int[][] HzMks) => await Task.Yield();
  public async Task EnterAsync() => await Task.Yield();
  public async Task ExitAsync() => await Task.Yield();
  public async Task GradientAsync(int fromHz = 100, int tillHz = 300, int stepHz = 4) => await Task.Yield();
  public async Task WaveAsync3k8k4() => await Task.Yield();
  public async Task WaveAsync4k7k2() => await Task.Yield();
  public async Task WaveAsync7k5k1() => await Task.Yield();
}