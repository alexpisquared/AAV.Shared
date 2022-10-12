using System.Threading.Tasks;

namespace StandardLib.Helpers
{
  //[System.Obsolete] - only for exceptional situations when proper beeper is not available.
  internal class BprKernel32
  {
    public static void Start() { Beep(_7kHz, _msA); Beep(_8kHz, _msB); }
    public static void Finish() { Beep(_8kHz, _msB); Beep(_7kHz, _msA); }
    public static void Tick() => NativeMethods.BeepIf(8000, 120);
    public static void Error() // CI-RDP-adjusted.
    {
      NativeMethods.BeepIf(_6kHz, 200);
      NativeMethods.BeepIf(_6kHz, 200);
      NativeMethods.BeepIf(_6kHz, 200);
      NativeMethods.BeepIf(_5kHz, 800);
    }

    public static void ErrorFaF() => Task.Run(() => Error());
    public static void StartFAF() => Task.Run(() => Start());
    public static void TickFAF() => Task.Run(() => Tick());
  
    static void Beep(int hz = 999, int ms = 999) => NativeMethods.BeepIf(hz, ms);
    static void BeepFAF(int hz = 999, int ms = 999) => Task.Run(() => NativeMethods.BeepIf(hz, ms));

    const int _5kHz = 5000, _6kHz = 6000, _7kHz = 7000, _8kHz = 8000, _msA = 50, _msB = 150;
  }
}
