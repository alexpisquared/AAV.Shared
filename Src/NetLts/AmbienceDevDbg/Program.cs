using System.Diagnostics;
using AmbienceDevDbg;
using AmbienceLib;

var _bpr = new Bpr();

if (!Debugger.IsAttached)
{
  Console.WriteLine("Only when  Debugger.IsAttached !!!\nOnly when  Debugger.IsAttached !!!\nOnly when  Debugger.IsAttached !!!\nOnly when  Debugger.IsAttached !!!\nOnly when  Debugger.IsAttached !!!\nOnly when  Debugger.IsAttached !!!\n");
}
else
{
  await BeepTest();
  await OneMinUp(_bpr); // works fine ...but not while debugging.
  await SpeechSynthTest.TestTTS();
}

static async Task OneMinUp(Bpr b)
{
  Console.WriteLine("52, 9_000, 19, 30_000, ushort.MaxValue / 20 ");
  await b.GradientAsync(52, 9_000, 19, 30_000, ushort.MaxValue / 20).ConfigureAwait(false);

  Console.WriteLine("52, 9_000, 19, 30_000, ushort.MaxValue / 1 ");
  await b.GradientAsync(52, 9_000, 19, 30_000).ConfigureAwait(false);
}

static Task M1(Bpr b) { Console.Write($"  started Audio  "); return b.GradientAsync(52, 9_000, 19, 30_000); }

static Task M2() { Console.Write($"  started Delay  "); return Task.Delay(TimeSpan.FromMinutes(.25)); }

async Task BeepTest()
{
  //await _vm.Ssynth.SpeakAsync("1 Mississippi, 2 Mississippi, 3 Mississippi, 4 Mississippi, 5 Mississippi, 6 Mississippi, 7 Mississippi, 8 Mississippi");
  //await _bpr.WarnAsync();
  //await _bpr.ErrorAsync();

  for (var i = 0; i < 999 && !Console.KeyAvailable; i++)
  {
    var started = Stopwatch.GetTimestamp();
    Console.Write($"{i,4}... ");

    //await _bpr.AppStartAsync();
    //await _bpr.StartAsync();
    //await _bpr.FinishAsync();
    //await _bpr.AppFinishAsync();

    //_bpr.GradientAsync(610, 201, 1); // works fine ...but not while debugging.
    //await _bpr.GradientAsync(610, 201, 1).ConfigureAwait(false); // works fine ...but not while debugging.

    //await OneMinUp(_bpr).ConfigureAwait(false);

    var taskDelay = M2(); // must go first, or else it will be scheduled AFTER! completion of the scream.
    var taskScream = M1(_bpr);
    await Task.WhenAll(taskDelay, taskScream);

    await Task.Delay(3);

    //await _bpr.DevDbg();

    Console.Write($"...{i,-4}   {Stopwatch.GetElapsedTime(started).TotalSeconds,5:N2} sec     Press any key to break for loop.\n");
  }

  _ = Console.ReadKey(true);
}