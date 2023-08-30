
using System.Diagnostics;

var b = new AmbienceLib.Bpr();

//await b.WarnAsync();
//await b.ErrorAsync();

for (int i = 0; i < 999 && !Console.KeyAvailable; i++)
{
  Console.WriteLine($"{i,4}");
  Debug.WriteLine($"{i,4}");

  //await b.AppStartAsync();
  //await b.StartAsync();
  //await b.FinishAsync();
  //await b.AppFinishAsync();

  //b.GradientAsync(610, 201, 1); // works fine ...but not while debugging.
  //await b.GradientAsync(610, 201, 1).ConfigureAwait(false); // works fine ...but not while debugging.
  await b.GradientAsync(614, 203, 1).ConfigureAwait(false);

  await Task.Delay(3);

  //await b.DevDbg();
}

Console.ReadKey(true);
