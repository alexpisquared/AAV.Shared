namespace StandardLib.Services;

public class BackgroundTaskBase
{
  protected readonly CancellationTokenSource _cts = new();
  protected PeriodicTimer? _timer;
  protected Task? _timerTask;

  public async Task StopAsync()
  {
    _cts.Cancel();
    if (_timerTask is not null) await _timerTask.ConfigureAwait(false); // wait for the last invocation of the task to complete.
    _cts.Dispose();
    WriteLine("   ..Task (timer) was cancelled.");
  }
  protected async Task DoWorkAsync(Action act)
  {
    ArgumentNullException.ThrowIfNull(nameof(_timer));
    try { while (await _timer.WaitForNextTickAsync(_cts.Token)) { act(); } } catch (OperationCanceledException) { }
  }
}
public class BackgroundTask0rg : BackgroundTaskBase
{
  public BackgroundTask0rg(TimeSpan interval) => _timer = new PeriodicTimer(interval);
  public void Start(Action act) => _timerTask = DoWorkAsync(act);
}
public class BackgroundTask : BackgroundTaskBase //tu: Nick Chapsas - Scheduling repeating tasks with .NET 6’s NEW Timer - https://www.youtube.com/watch?v=J4JL4zR_l-0
{
  readonly Action _act;
  public BackgroundTask(TimeSpan interval, Action act) => (_timer, _act) = (new PeriodicTimer(interval), act);
  public void Start() => _timerTask = DoWorkAsync(_act);
}
public class BackgroundTaskDisposable : BackgroundTaskBase, IDisposable
{
  public BackgroundTaskDisposable(TimeSpan interval, Action act)
  {
    _timer = new PeriodicTimer(interval);
    _timerTask = DoWorkAsync(act);              // does start execution!!!
  }
  //public void Start() => _timerTask.Start();    // ctor does start execution!!!

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
    await TRandomAsync();
    await T3Async();
    await T0Async();
    await TAsync();
  }
  static async Task T0Async()
  {
    var bt = new BackgroundTask0rg(TimeSpan.FromSeconds(.1));
    bt.Start(OnTimer);
    await Task.Delay(300);
    await bt.StopAsync();
  }
  static async Task TAsync()
  {
    var bt = new BackgroundTask(TimeSpan.FromSeconds(.1), OnTimer);
    bt.Start();
    await Task.Delay(300);
    await bt.StopAsync();
  }
  static async Task T3Async()
  {
    var bt = new BackgroundTaskDisposable(TimeSpan.FromSeconds(.1), OnTimer);
    await Task.Delay(300);
    await bt.StopAsync();
  }
  static async Task TRandomAsync()
  {
    _start = Stopwatch.GetTimestamp();
    var bt = new BackgroundTaskDisposable(TimeSpan.FromSeconds(.1), OnTimerRandoDelay);
    await Task.Delay(300);
    await bt.StopAsync();
  }
  static void OnTimer() => Write("* ");
  static async void OnTimerRandoDelay()
  {
    var ms = rnd.Next(100);
    await Task.Delay(ms);
    Stopwatch.GetElapsedTime(_start);
    Write($"{Stopwatch.GetElapsedTime(_start).TotalMilliseconds:N0} ");
  }
  static Random rnd = new Random();
  private static long _start;
}

/*
 * 
No, there is no such method as WaitAllAsync. The closest equivalent would be the Task.WhenAll() method, which is an asynchronous version of WaitAll. It takes an array of Task objects as a parameter and returns a Task that can be awaited until all of the tasks in the array have completed. Here is an example of how you can use it:

Task task1 = Task.Run(() => { ... }); 
Task task2 = Task.Run(() => { ... });

await Task.WhenAll(new Task[] { task1, task2 });
*/