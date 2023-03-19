// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

do
{
  await new AmbienceLib.Bpr().ErrorAsync();
  //await AmbienceLib.Bpr.DevDbg();
} while (Console.ReadKey().Key != ConsoleKey.Q);
