﻿using AAV.Sys.Helpers;

namespace Helpers;

[Obsolete("Use  AmbienceLib.Bpr  instead.")]
public static class Bpr
{
  static void beep(int freq, int dur) => NativeMethods.Beep(freq, dur);

  public static void WakeAudio() => NativeMethods.Beep(11111, 333);
  public static void OkFaF() => Task.Run(BeepOk);
  public static void OKbFaF() => Task.Run(BeepOkB);
  public static void ErrorFaF() => Task.Run(ErrorSynch);
  public static void Begin1FaF() => Task.Run(Begin1Sync);
  public static void Begin2FaF() => Task.Run(Begin2Sync);
  public static void ShortFaF() => Task.Run(BeepShort);

  public static void ErrorSynch()
  {
    beep(7000, 60);
    beep(7000, 60);
    beep(7000, 60);
    beep(6000, 120);
  }
  public static void Begin1Sync() => beep(200, 333);
  public static void Begin2Sync() { beep(200, 100); beep(221, 100); }

  public static void Error() => ErrorFaF();

  const int _minDurn140 = 140, _210ms = 210,  // minimal and medium durations on Razer1. (120 is already silent)
    higher = 250, highest = 260;              // org too loud: highest = 10111, higher = 9111;

  public static void BeepOk() => beep(2500, _210ms);
  public static void BeepOkB() => beep(4000, _210ms);
  public static void Click() => beep(higher, _210ms);
  public static void BeepShort() => beep(highest, _minDurn140);
  public static void BeepLong() => beep(100, 750);
  public static void BeepDone() => BeepEnd3();
  public static void BeepFD(int v1, int v2) => beep(v1, v2);
  public static void Beep1of2() { beep(higher, _210ms); beep(highest, _minDurn140); }
  public static void Beep2of2() { beep(highest, _minDurn140); beep(higher, _210ms); }

  public static void No() { beep(4211, _minDurn140); beep(4000, _minDurn140); }
  public static void BeepBigError() { for (double i = 1000; i < 5000; i *= 1.5) beep((int)i, (int)(higher / i)); }
  public static void BeepBgn2() { var v = 300; beep(v + 200, _minDurn140); beep(v + 300, _minDurn140 * 6); }
  public static void BeepEnd2() { var v = 300; beep(v + 200, _minDurn140); beep(v + 100, _minDurn140 * 6); }
  public static void BeepBgn3() { var v = 300; beep(v + 100, _minDurn140); beep(v + 200, _minDurn140); beep(v + 300, _minDurn140 * 6); }
  public static void BeepEnd3() { var v = 300; beep(v + 300, _minDurn140); beep(v + 200, _minDurn140); beep(v + 100, _minDurn140 * 6); }
  public static void BeepEnd6() { var v = 999; beep(v + 300, _minDurn140); beep(v + 200, _minDurn140); beep(v + 100, _minDurn140); beep(v, _minDurn140); beep(v + 300, _minDurn140); beep(v + 300, _minDurn140); }

  [Obsolete("await AlexPi.Scr.AltBpr.ChimerAlt.Chime()", false)]
  public static void Chime(int min)
  {
    switch (min)
    {
      case 1: chime1(); break;
      case 2: chime2(); break;
      case 3: chime3(); break;
      case 4: chime4(); break;
      case 5: chime5(); break;
      case 6: chime6(); break;
      case 7: chime7(); break;
      case 8: chime8(); break;
      case 9: chime9(); break;
      case 11: chime10(); chime1(); break;
      case 12: chime10(); chime2(); break;
      case 13: chime10(); chime3(); break;
      case 14: chime10(); chime4(); break;
      case 15: chime10(); chime5(); break;
      case 16: chime10(); chime6(); break;
      case 17: chime10(); chime7(); break;
      case 18: chime10(); chime8(); break;
      case 19: chime10(); chime9(); break;
      case 21: chime10(); chime10(); chime1(); break;
      case 22: chime10(); chime10(); chime2(); break;
      case 23: chime10(); chime10(); chime3(); break;
      case 24: chime10(); chime10(); chime4(); break;
      case 25: chime10(); chime10(); chime5(); break;
      case 26: chime10(); chime10(); chime6(); break;
      case 27: chime10(); chime10(); chime7(); break;
      case 28: chime10(); chime10(); chime8(); break;
      case 29: chime10(); chime10(); chime9(); break;
      case 10: case 20: case 30: case 40: case 50: case 60: case 70: case 80: case 90: case 100: case 110: for (var i = 0; i < min / 10; i++) chime10(); break;
      default: break;
    }
  }
  #region chimes 1 - 10

  const int _a = 4401, _b = 4801, _c = 5237, _1 = 30, _2 = 40, _3 = 50;

  static void chime1() { beep(_a, _3 + _3); pause(); }
  static void chime2() { beep(_a, _3); beep(_b, _3); pause(); }
  static void chime3() { beep(_a, _2); beep(_b, _2); beep(_c, _2); pause(); }
  static void chime4() { beep(_a, _1); beep(_b, _1); pause(); beep(_a, _1); beep(_b, _1); pause(); }
  static void chime5() { beep(_a, _1); beep(_b, _1); pause(); beep(_a, _1); beep(_b, _1); pause(); beep(_a, _3); pause(); }
  static void chime6() { chime3(); chime3(); }
  static void chime7() { chime3(); chime3(); chime1(); }
  static void chime8() { chime3(); chime3(); chime2(); }
  static void chime9() { chime3(); chime3(); chime3(); }
  static void chime10() => beep(_c, 800);
  static void pause() => Debug.Write(""); // beep(21000, 10);

  #endregion
}
