namespace StandardContractsLib;
public interface IBpr
{
  bool SuppressAlarm { get; set; }
  bool SuppressTicks { get; set; }

  int[] FixDuration(int hz, int mks);
  void AppFinish(ushort vol = 5957);
  Task AppFinishAsync(ushort vol = 5957);
  void AppStart(ushort vol = 5957);
  Task AppStartAsync(ushort vol = 5957);
  Task AreYouSureAsync();
  Task AreYouSure2kAsync();
  Task AreYouSure1kAsync();
  void Beep(int hz, double sec);
  void Beep(int hz, double sec, ushort vol);
  void Beep1of2();
  void Beep2of2();
  Task BeepAsync(int hz, double sec);
  Task BeepAsync(int hz, double sec, ushort vol);
  Task BeepHzMks(int[][] HzMks, bool isAsync = true);
  Task BeepHzMks(int[][] HzMks, ushort volume, bool isAsync = true);
  Task Bulk100_600();
  Task BulkWhistle();
  Task Bulk_250_500_110ms();
  Task Bulk_500_1000_110ms();
  Task ChockingUhmAsync();
  void Click();
  Task ClickAsync();
  void Enter();
  Task EnterAsync();
  void Error();
  Task ErrorAsync();
  void Exit();
  Task ExitAsync();
  int[] FFD(int hz, int mks = 80000);
  void Finish(int stepHz = 4, ushort vol = 1985);
  Task FinishAsync(int stepHz = 4, ushort vol = 1985);
  Task GradientAsync(int fromHz = 100, int tillHz = 300, int stepHz = 4, int mks = 1, ushort vol = ushort.MaxValue);
  void No();
  Task NoAsync();
  void NuHuh();
  Task NuHuhAsync();
  Task OhWowAsync();
  void Start(int stepHz = 4, ushort vol = 1985);
  Task StartAsync(int stepHz = 4, ushort vol = 1985);
  void Tick();
  Task TickAsync();
  Task Tuk500_100();
  Task Tuk505_50();
  Task TwitAsync();
  void UhHuh();
  Task UhHuhAsync();
  Task WakeAudio();
  void Warn();
  Task WarnAsync();
  Task<string> Wave2Async(int[] freqs, int[] stepsHz);
  Task WaveAsync(int fromHz = 100, int tillHz = 300, int stepHz = 4);
  Task WaveAsync3k8k4();
  Task WaveAsync4k7k2();
  Task WaveAsync7k5k1();
  Task Wh_WhatAsync();
  Task WovAsync();
  void Yes();
  Task YesAsync();
  Task YouSureAsync();
}