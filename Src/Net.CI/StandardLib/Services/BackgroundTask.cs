namespace StandardLib.Services;
public class BackgroundTask //tu: Nick Chapsas - Scheduling repeating tasks with .NET 6’s NEW Timer - https://www.youtube.com/watch?v=J4JL4zR_l-0
{
  readonly CancellationTokenSource _cts = new();
  readonly PeriodicTimer _timer;
  readonly Action _act;
  Task? _timerTask;

  public BackgroundTask(TimeSpan interval, Action act) => (_timer, _act) = (new PeriodicTimer(interval), act);
  public void Start() => _timerTask = DoWorkAsync(_act);

  public async Task StopAsync()
  {
    if (_timerTask is null) return;

    _cts.Cancel();
    if (_timerTask is not null) await _timerTask.ConfigureAwait(false); // wait for the last invocation of the task to complete.
    _cts.Dispose();
    WriteLine("   ..Task (timer) was cancelled.");
  }

  async Task DoWorkAsync(Action act)
  {
    try
    {
      while (await _timer.WaitForNextTickAsync(_cts.Token))
      {
        act();
      }
    }
    catch (OperationCanceledException) { }
  }
}
public class BackgroundTaskDisposable : IDisposable
{
  readonly CancellationTokenSource _cts = new();
  readonly PeriodicTimer _timer;
  readonly Task _timerTask;

  public BackgroundTaskDisposable(TimeSpan interval, Action act)
  {
    _timer = new PeriodicTimer(interval);
    _timerTask = DoWorkAsync(act);              // does start execution!!!
  }
  //public void Start() => _timerTask.Start();    // ctor does start execution!!!

  public async Task StopAsync()
  {
    if (_timerTask is null) return;
    _cts.Cancel();
    if (_timerTask is not null) await _timerTask.ConfigureAwait(false); // wait for the last invocation of the task to complete.
    _cts.Dispose();
    WriteLine("   ..Task (timer) was cancelled.");
  }

  async Task DoWorkAsync(Action act)
  {
    try
    {
      while (await _timer.WaitForNextTickAsync(_cts.Token))
      {
        act();
      }
    }
    catch (OperationCanceledException) { }
  }

  void IDisposable.Dispose()
  {
    if (!_cts.IsCancellationRequested)
    {
      StopAsync().GetAwaiter().GetResult();
    }
    _cts.Dispose();
  }
}
public class BackgroundTaskTest
{
  public static async Task TestAllAsync()
  {
    await T3();
    await T0();
    await T();    
  }
  private static async Task T0()
  {
    var bt = new BackgroundTask0rg(TimeSpan.FromSeconds(.1));
    bt.Start(OnTimer);
    await Task.Delay(300);
    await bt.StopAsync();
  }
  private static async Task T()
  {
    var bt = new BackgroundTask(TimeSpan.FromSeconds(.1), OnTimer);
    bt.Start();
    await Task.Delay(300);
    await bt.StopAsync();
  }
  private static async Task T3()
  {
    var bt = new BackgroundTaskDisposable(TimeSpan.FromSeconds(.1), OnTimer);
    await Task.Delay(300);
    await bt.StopAsync();
  }
  static void OnTimer() => Write("   ***   ");
}
public class BackgroundTask0rg
{
  readonly CancellationTokenSource _cts = new();
  readonly PeriodicTimer _timer;
  Task? _timerTask;

  public BackgroundTask0rg(TimeSpan interval) => _timer = new PeriodicTimer(interval);
  public void Start(Action act) => _timerTask = DoWorkAsync(act);
  public async Task StopAsync()
  {
    if (_timerTask is null) return;
    _cts.Cancel();
    if (_timerTask is not null) await _timerTask.ConfigureAwait(false); // wait for the last invocation of the task to complete.
    _cts.Dispose();
    WriteLine("   ..Task (timer) was cancelled.");
  }

  async Task DoWorkAsync(Action act)
  {
    try
    {
      while (await _timer.WaitForNextTickAsync(_cts.Token))
      {
        act();
      }
    }
    catch (OperationCanceledException) { }
  }
}
