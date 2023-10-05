using System.Diagnostics;
using AmbienceDevDbg;
using AmbienceLib;

if (Debugger.IsAttached) { await SpeechSynthTest.TestTTS(); }


//await BeepTest();
static async Task OneMinUp(Bpr b) => await b.GradientAsync(52, 9_000, 19, 30_000).ConfigureAwait(false);
static Task M1(Bpr b)
{
  Console.Write($"  started Audio  ");
  return b.GradientAsync(52, 9_000, 19, 30_000);
}
static Task M2()
{
  Console.Write($"  started Delay  ");
  return Task.Delay(TimeSpan.FromMinutes(.25));
}
static async Task BeepTest()
{
  var b = new AmbienceLib.Bpr();

  //await _vm.Ssynth.SpeakAsync("1 Mississippi, 2 Mississippi, 3 Mississippi, 4 Mississippi, 5 Mississippi, 6 Mississippi, 7 Mississippi, 8 Mississippi");
  //await b.WarnAsync();
  //await b.ErrorAsync();

  for (int i = 0; i < 999 && !Console.KeyAvailable; i++)
  {
    var started = Stopwatch.GetTimestamp();
    Console.Write($"{i,4}... ");
    Debug.Write($"{i,4}  ");

    //await b.AppStartAsync();
    //await b.StartAsync();
    //await b.FinishAsync();
    //await b.AppFinishAsync();

    //b.GradientAsync(610, 201, 1); // works fine ...but not while debugging.
    //await b.GradientAsync(610, 201, 1).ConfigureAwait(false); // works fine ...but not while debugging.


    //await OneMinUp(b).ConfigureAwait(false);

    Task taskDelay = M2(); // must go first, or else it will be scheduled AFTER! completion of the scream.
    Task taskScream = M1(b);
    await Task.WhenAll(taskDelay, taskScream);


    await Task.Delay(3);

    //await b.DevDbg();

    Console.Write($"...{i,-4}   {Stopwatch.GetElapsedTime(started).TotalSeconds,5:N2} sec\n");
  }

  Console.ReadKey(true);
}