namespace StandardLib.Services;
public class BackgroundTask //tu: Nick Chapsas - Scheduling repeating tasks with .NET 6’s NEW Timer - https://www.youtube.com/watch?v=J4JL4zR_l-0
{
  readonly CancellationTokenSource _cts = new();
  readonly PeriodicTimer _timer;
  readonly Action _act;
  Task? _timerTask;

  //public BackgroundTask(TimeSpan interval) => _timer = new PeriodicTimer(interval);
  public BackgroundTask(TimeSpan interval, Action act) => (_timer, _act) = (new PeriodicTimer(interval), act);

  public void Start(Action act) => _timerTask = DoWorkAsync(act);
  public void Start() => _timerTask = DoWorkAsync(_act);
  public async Task StopAsync()
  {
    _cts.Cancel();
    if (_timerTask is not null) await _timerTask; // wait for the last invocation of the task to complete.
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