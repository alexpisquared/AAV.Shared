﻿namespace StandardContracts.Lib;

public interface IBpr
{
  bool SuppressAlarm { get; set; }
  bool SuppressTicks { get; set; }

  void AppFinish();
  Task AppFinishAsync();
  void AppStart();
  Task AppStartAsync();
  void Beep(int hz, double sec);
  void Beep(int hz, double sec, ushort vol);
  Task BeepAsync(int hz, double sec);
  Task BeepAsync(int hz, double sec, ushort vol);
  Task BeepHzMks(int[][] HzMks);
  Task BeepHzMks(int[][] HzMks, ushort volume);
  void Click();
  Task ClickAsync();
  void Enter();
  Task EnterAsync();
  void Error();
  Task ErrorAsync();
  void Exit();
  Task ExitAsync();
  int[] FFD(int hz, int mks = 75000);
  void Finish();
  Task FinishAsync();
  Task GradientAsync(int fromHz = 100, int tillHz = 300, int stepHz = 4);
  void No();
  Task NoAsync();
  void Start();
  Task StartAsync();
  void Tick();
  Task TickAsync();
  void Warn();
  Task WarnAsync();
  Task WaveAsync(int fromHz = 100, int tillHz = 300, int stepHz = 4);
  Task WaveAsync3k8k4();
  Task WaveAsync4k7k2();
  Task WaveAsync7k5k1();
  void Yes();
  Task YesAsync();
}
