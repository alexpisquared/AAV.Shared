var _bpr = new Bpr();

//await new SpeechSynthTest().TestVoice();
//await new SpeechSynthTest().TestMeasureTimedCoeficientForSpeakFreeAsync();

await WaveAsync();
return;


//_bpr.ClickAsync().Wait();
await _bpr.AppStartAsync();
await _bpr.StartAsync();
await _bpr.FinishAsync();
await _bpr.AppFinishAsync();

await _bpr.StartAsync();
await _bpr.FinishAsync();

await _bpr.StartAsync();
await _bpr.FinishAsync();

await _bpr.AppStartAsync();
await _bpr.AppFinishAsync();
//await _bpr.WaveAsync3k8k4();
//await _bpr.WaveAsync7k5k1();


if (!Debugger.IsAttached)
{
  Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Only when  Debugger.IsAttached !!!\nOnly when  Debugger.IsAttached !!!\nOnly when  Debugger.IsAttached !!!\nOnly when  Debugger.IsAttached !!!\nOnly when  Debugger.IsAttached !!!\nOnly when  Debugger.IsAttached !!!\n");
}
else
{
  Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("CLick here ... then Escape to exit");
  await NuHUh();
  await UhHuh();
  await BeepTest();
  await OneMinUp(_bpr); // works fine ...but not while debugging.
}

async Task WaveAsync()
{
  do
  {
    await _bpr.Wave2Async([      500, 3000, 1500, 5000], [10, 5, 2]);
    //await _bpr.Wave2Async([      1000, 2020, 1500, 2500, 1500, 3500, 2500, 4500, 1033, 50], [8, 1, 8, 3, 8, 5, 12, 7, 5]);
    Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("CLick here ... then Escape to exit"); Console.ResetColor();
  } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
}

async Task UhHuh()
{
  do
  {
    await _bpr.UhHuhAsync();
  } while (Console.ReadKey().Key != ConsoleKey.Escape);
}

async Task NuHUh()
{
  do
  {
    await _bpr.NuHuhAsync();
  } while (Console.ReadKey().Key != ConsoleKey.Escape);
}

Console.ResetColor();

static async Task OneMinUp(Bpr b)
{
  Console.WriteLine("52, 9_000, 19, 30_000, ushort.MaxValue / 20 "); await b.GradientAsync(52, 9_000, 19, 30_000, ushort.MaxValue / 20).ConfigureAwait(false);
  Console.WriteLine("52, 9_000, 19, 30_000, ushort.MaxValue / 1  "); await b.GradientAsync(52, 9_000, 19, 30_000).ConfigureAwait(false);
}

static Task Play(Bpr b) { try { Console.ForegroundColor = ConsoleColor.Green; Console.Write($"  started Audio  "); return b.GradientAsync(52, 9_000, 85, 30_000); } finally { Console.ResetColor(); } }

static Task Wait() { try { Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write($"  started Delay  "); return Task.Delay(TimeSpan.FromSeconds(5)); } finally { Console.ResetColor(); } }

async Task BeepTest()
{
  Console.ForegroundColor = ConsoleColor.DarkCyan;
  //await _vm.Ssynth.SpeakAsync("1 Mississippi, 2 Mississippi, 3 Mississippi, 4 Mississippi, 5 Mississippi, 6 Mississippi, 7 Mississippi, 8 Mississippi");
  //await _bpr.WarnAsync();
  //await _bpr.ErrorAsync();

  for (var i = 0; i < 999 && !Console.KeyAvailable; i++)
  {
    //await _bpr.AppStartAsync();
    //await _bpr.StartAsync();
    //await _bpr.FinishAsync();
    //await _bpr.AppFinishAsync();

    //_bpr.GradientAsync(610, 201, 1); // works fine ...but not while debugging.
    //await _bpr.GradientAsync(610, 201, 1).ConfigureAwait(false); // works fine ...but not while debugging.

    //await OneMinUp(_bpr).ConfigureAwait(false);

    Console.Write($"...{i,-3}Press any key to break the for() loop.");

    var started = Stopwatch.GetTimestamp();
    Console.Write($"  000  ");
    var taskDelay = Wait(); // must go first, or else it will be scheduled AFTER! completion of the scream.
    Console.Write($"  111  ");
    var taskScream = Play(_bpr); // it is blocking!?!?!?!?

    Console.Write($"  222  ");
    await Task.WhenAll(taskDelay, taskScream);
    Console.Write($"  333  ");

    //await Task.Delay(3);
    //await _bpr.DevDbg();

    Console.Write($"   {Stopwatch.GetElapsedTime(started).TotalSeconds,5:N2} sec     \n");
  }

  var __ = Console.ReadKey(true);
  Console.Write($"   {__} was pressed \n");
}
async Task BulkWhistle() => await _bpr.Wave2Async([200, 3000, 1000, 8000], [15, 15, 15]);
async Task Bulk_500_1000_110ms() => await _bpr.Wave2Async([500, 1000], [20]);
async Task Bulk_250_500_110ms() => await _bpr.Wave2Async([250, 500], [20]);
async Task Bulk100_600() => await _bpr.Wave2Async([100, 600], [20]); // 0.19 sec
async Task Tuk500_100() => await _bpr.Wave2Async([500, 100], [20]); // 0.19 sec
async Task Tuk505_50() => await _bpr.Wave2Async([505, 50], [5]); // 0.52 sec
