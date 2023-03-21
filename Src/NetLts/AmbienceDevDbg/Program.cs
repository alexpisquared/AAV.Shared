// See https://aka.ms/new-console-template for more information

var b = new AmbienceLib.Bpr();
do
{
  await b.WarnAsync();
  await b.ErrorAsync();

  await b.AppStartAsync();
  await b.StartAsync();
  await b.FinishAsync();
  await b.AppFinishAsync();

  //await b.DevDbg();

  Console.WriteLine("Press any Escape!...");
} while (Console.ReadKey().Key != ConsoleKey.Escape);
