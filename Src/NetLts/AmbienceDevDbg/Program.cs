
using System.Diagnostics;
using AmbienceLib;

var b = new AmbienceLib.Bpr();

//await b.WarnAsync();
//await b.ErrorAsync();

for (int i = 0; i < 999 && !Console.KeyAvailable; i++)
{
  Console.WriteLine($"{i,4}");
  Debug.Write($"{i,4}  ");

  //await b.AppStartAsync();
  //await b.StartAsync();
  //await b.FinishAsync();
  //await b.AppFinishAsync();

  //b.GradientAsync(610, 201, 1); // works fine ...but not while debugging.
  //await b.GradientAsync(610, 201, 1).ConfigureAwait(false); // works fine ...but not while debugging.
  
  
  //await OneMinUp(b).ConfigureAwait(false);

  var taskScream = b.GradientAsync(52, 9_000, 19, 120_000);
  var taskDelay = Task.Delay(TimeSpan.FromMinutes(1));

  await Task.WhenAll(taskScream, taskDelay);


  await Task.Delay(3);

  //await b.DevDbg();
}

Console.ReadKey(true);

static async Task OneMinUp(Bpr b) => await b.GradientAsync(52, 9_000, 19, 30_000).ConfigureAwait(false);