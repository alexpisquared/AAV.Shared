namespace StandardContractsLib;

public interface IBpr
{
  const int _startFinishQuieter = 33, _appStartFinishQuieter = 11;
  bool SuppressAlarm { get; set; }
  bool SuppressTicks { get; set; }

  void AppFinish(ushort vol = ushort.MaxValue / _appStartFinishQuieter);
  Task AppFinishAsync(ushort vol = ushort.MaxValue / _appStartFinishQuieter);
  void AppStart(ushort vol = ushort.MaxValue / _appStartFinishQuieter);
  Task AppStartAsync(ushort vol = ushort.MaxValue / _appStartFinishQuieter);
  void Beep(int hz, double sec);
  void Beep(int hz, double sec, ushort vol);
  Task BeepAsync(int hz, double sec);
  Task BeepAsync(int hz, double sec, ushort vol);
  Task BeepHzMks(int[][] HzMks, bool isAsync);
  Task BeepHzMks(int[][] HzMks, ushort volume, bool isAsync);
  void Click();
  Task ClickAsync();
  void Enter();
  Task EnterAsync();
  void Error();
  Task ErrorAsync();
  void Exit();
  Task ExitAsync();
  int[] FFD(int hz, int mks = 75000);
  void Finish(int stepHz = 4, ushort vol = ushort.MaxValue / 133);
  Task FinishAsync(int stepHz = 4, ushort vol = ushort.MaxValue / 133);
  Task GradientAsync(int fromHz = 100, int tillHz = 300, int stepHz = 4, int mks = 1, ushort vol = ushort.MaxValue / 115);
  void No();
  Task NoAsync();
  void Start(int stepHz = 4, ushort vol = ushort.MaxValue / 133);
  Task StartAsync(int stepHz = 4, ushort vol = ushort.MaxValue / 133);
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
