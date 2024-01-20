using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AAV.WPF.AltBpr;

public class ChimerAlt
{
  static readonly IConfigurationRoot _config;
  static readonly int _freqUp, _freqDn, _wakeMks;
  static readonly int[] _freqWhstl;
  static readonly double _stepDurnSec, _freqMultplr;

  static ChimerAlt()
  {
    _config = new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      .AddJsonFile("appsettings.AAV.WPF.json")
      .AddUserSecrets<ChimerAlt>()
      .Build();

    _freqWhstl = _config.GetSection("FreqWhistle").Get<int[]>() ?? [2000, 7000, 3000, 9000];
    _freqUp = _config.GetSection("FreqUp").Get<int?>() ?? 30;
    _freqDn = _config.GetSection("FreqDn").Get<int?>() ?? 8000;
    _wakeMks = _config.GetSection("WakeMks").Get<int?>() ?? 150111;
    _stepDurnSec = _config.GetSection("StepDurnSec").Get<double?>() ?? .00725;
    _freqMultplr = _config.GetSection("FreqMultiplier").Get<double?>() ?? 1.017;
  }

  public static async Task BeepFD(int freq = 98, int durationMks = 250111, ushort volume = ushort.MaxValue) => await Bpr.BeepMks(new[] { new[] { freq, Bpr.FixDuration(freq, durationMks) } }, volume);
  public static async Task BeepFD(int freq = 98, double durationSec = .25, ushort volume = ushort.MaxValue) => await Bpr.BeepMks(new[] { new[] { freq, Bpr.FixDuration(freq, (int)(durationSec * 1000000)) } }, volume);

  public static async Task FreqWalkUp(ushort volume) => await PlayFreqList(new[] { _freqDn, _freqUp }, 30, volume);
  public static async Task FreqWalkDn(ushort volume) => await PlayFreqList(new[] { _freqUp, _freqDn }, 60, volume);
  public static async Task FreqWalkUpDn() { await FreqWalkUp(ushort.MaxValue); await Task.Delay(333); await FreqWalkDn(ushort.MaxValue); }

  public static async Task FreqRunUpHiPh() => await PlayFreqList(new[] { 4000, 9000 }, durationSec: .3);
  public static async Task FreqRunUpDn() { await FreqRunUp(); await Task.Delay(999); await FreqRunDn(); }
  public static async Task FreqRunUp() => await PlayFreqList(new[] { 48, 100 }, durationSec: 1.9);
  public static async Task FreqRunDn() => await PlayFreqList(new[] { 128, 48 }, durationSec: 1.9);

  public static async Task WakeAudio() => await BeepFD(48, _wakeMks, ushort.MaxValue / 100);

  //public static async Task FreqWalkUpDnUp(double freqA = 200, double freqB = 20, double freqC = 300, double durationSec = 1, double durnMultr = 1)
  //{
  //  var freqDurnList = new List<int[]>();

  //  addS_Old(freqA, freqB, freqDurnList, durationSec, durnMultr);
  //  addS_Old(freqB, freqA, freqDurnList, durationSec, durnMultr);
  //  addS_Old(freqA, freqC, freqDurnList, durationSec, durnMultr);

  //  await Bpr.BeepMks(freqDurnList.ToArray());
  //}

  public static async Task PlayWhistle(ushort volume = ushort.MaxValue) => await PlayFreqList(_freqWhstl, .175, volume);
  public static async Task PlayFreqList(int[] freqs, double durationSec = 1, ushort volume = ushort.MaxValue) => await Pfl(freqs, durationSec, volume);
  public static async Task pfL(int[] freqs, double durnSec, ushort volume = ushort.MaxValue) { var l = new List<int[]>(); for (var i = 0; i < freqs.Length - 1; i++) addStrng(freqs[i], freqs[i + 1], l, durnSec); await Bpr.BeepMks(l.ToArray(), volume); }
  public static async Task Pfl(int[] freqs, double durnSec, ushort volume = ushort.MaxValue) { var l = new List<int[]>(); for (var i = 0; i < freqs.Length - 1; i++) addSteps(freqs[i], freqs[i + 1], l, durnSec); await Bpr.BeepMks(l.ToArray(), volume); }
  public static void addStrng(double freqA, double freqB, List<int[]> freqDurnList, double durationSec = 1, double durnMultr = 1)
  {
    var stepDurnSec = _stepDurnSec * durationSec / .1;
    var stepsTtlCnt = (int)(durationSec / stepDurnSec);
    var stepDurnMks = stepDurnSec * 1e6;

    var frMultr = Math.Pow(freqB / freqA, 1.0 / stepsTtlCnt);
    var freq = freqA;

    for (var i = 0; i <= stepsTtlCnt; i++)
    {
      fixDurnAndAdd((int)stepDurnMks, (int)Math.Round(freq), freqDurnList);
      freq *= frMultr;
    }

    Console.WriteLine($"    frMultr: {frMultr:N3}   Steps: {stepsTtlCnt}   Step Duration: {stepDurnMks:N0} mks  =>  total time (s) requested / actual / took: {durationSec:N3} / {stepsTtlCnt * stepDurnMks * .000001:N3} / {freqDurnList.Sum(r => r[1]) * .000001:N3}.");
  }
  public static void addSteps(double freqA, double freqB, List<int[]> freqDurnList, double durationSec = 1, double durnMultr = 1)
  {
    var up = freqA < freqB;
    var stepsTtlCn0 = Math.Log(up ? freqB / freqA : freqA / freqB, _freqMultplr);
    var stepsTtlCnt = (int)Math.Round(stepsTtlCn0);
    var stepDurnMks = 1000000 * durationSec / stepsTtlCnt;

    var frMultr = up ? _freqMultplr : 1.0 / _freqMultplr;

    var freq = freqA;

    for (var i = 0; i <= stepsTtlCnt; i++)
    {
      fixDurnAndAdd((int)(stepDurnMks * Math.Pow(durnMultr, i)), (int)Math.Round(freq), freqDurnList);
      freq *= frMultr;
    }

    Console.WriteLine($"old frMultr: {frMultr:N3}   Steps: {stepsTtlCnt}   Step Duration: {stepDurnMks:N0} mks  =>  total time (s) requested / actual / took: {durationSec:N3} / {stepsTtlCnt * stepDurnMks * .000001:N3} / {freqDurnList.Sum(r => r[1]) * .000001:N3}.");
  }

  public static async Task NoteWalk(int noteA = 108, int noteB = 11, int durationMks = 60000, double delta = 1, ushort volume = ushort.MaxValue)
  {
    var fullScale = new List<int[]>();

    if (durationMks < 10000)
      durationMks *= 1000;

    if (noteA < noteB)
      for (int note = noteA, j = 1; note < noteB; note++, j++) add((int)(durationMks * Math.Pow(delta, j)), fullScale, note);
    else
      for (int note = noteA, j = 1; note > noteB; note--, j++) add((int)(durationMks * Math.Pow(delta, j)), fullScale, note);

    await Bpr.BeepMks(fullScale.ToArray(), volume);

    static void add(int dur, List<int[]> scale, int note)
    {
      var hz = (int)Freq(note);
      //Console.WriteLine($"chime - {note,5} => {Freq(note),8:N0}  {dur,8:N0}  ");
      scale.Add(new[] { hz, Bpr.FixDuration(hz, dur) });
    }
  }

  public static void FreqTable() { for (var note = 108; note > 0; note--) Console.WriteLine($"{note,5} => {Freq(note),8:N0}"); } //  1,12,108 => 27.5, 52.0, 13289.75

  static void fixDurnAndAdd(int durnOrgnl, int freqHz, List<int[]> freqDurn)
  {
    var durnFixed = Bpr.FixDuration(freqHz, durnOrgnl);
    Console.WriteLine($"  cut - {freqHz,5} Hz :  {durnOrgnl,8:N0} ~> {durnFixed,8:N0} mks. ");
    if (durnFixed > 0)
    {
      freqDurn.Add(new[] { freqHz, durnFixed });
    }
  }
  public static double Freq(int note) => Math.Pow(2, (note - 49) / 12.0) * 440;
  public static async Task Chime(int min)
  {
    switch (min)
    {
      case 01: await Bpr.BeepMks(new[] { new[] { _a, _p + _p } }, ushort.MaxValue); break;
      case 02: await Bpr.BeepMks(new[] { _cd, _p2, _ad }); break;
      case 03: await Bpr.BeepMks(new[] { _ad, _bd, _cd, }); break;
      //se 04: await Bpr.BeepMks(new[] { _cd, _ad, __1, _cd, _ad }); break;
      case 04: await Bpr.BeepMks(new[] { _ad, _bd, _cd, _p1, _cd }); break;
      //se 05: await Bpr.BeepMks(new[] { _cd, _ad, __1, _cd, _ad, __1, _cd }); break;
      case 05: await Bpr.BeepMks(new[] { _ad, _bd, _cd, _p1, _cd, _ad }); break;
      case 06: await Bpr.BeepMks(new[] { _ad, _bd, _cd, _ad, _bd, _cd }); break;
      case 07: await Bpr.BeepMks(new[] { _ad, _bd, _cd, _ad, _bd, _cd, _p1, _ad }); break;
      case 08: await Bpr.BeepMks(new[] { _ad, _bd, _cd, _ad, _bd, _cd, _p1, _ad, _p1, _bd }); break;
      case 09: await Bpr.BeepMks(new[] { _ad, _bd, _cd, _ad, _bd, _cd, _p1, _ad, _p1, _bd, _p1, _cd }); break;
      case 10: await Bpr.BeepMks(new[] { _a4 }); ; break;
      case 11: await Bpr.BeepMks(new[] { _a4, _p1, _cd }); break;
      case 12: await Bpr.BeepMks(new[] { _a4, _p1, _cd, _ad }); break;
      case 13: await Bpr.BeepMks(new[] { _a4, _p1, _ad, _bd, _cd }); break;
      case 14: await Bpr.BeepMks(new[] { _a4, _p1, _cd, _ad, _p1, _cd, _ad }); break;
      case 15: await Bpr.BeepMks(new[] { _a4, _p1, _cd, _ad, _p1, _cd, _ad, _p1, _cd }); break;
      case 20: await Bpr.BeepMks(new[] { _a4, _p1, _a4 }); break;
      case 25: await Bpr.BeepMks(new[] { _a4, _p1, _a4, _cd, _ad, _p1, _cd, _ad, _p1, _cd }); break;
      case 30: await Bpr.BeepMks(new[] { _a4, _p1, _a4, _p1, _a4 }); break;
      case 40: await Bpr.BeepMks(new[] { _a4, _p1, _a4, _p1, _a4, _p1, _a4 }); break;
      case 50: await Bpr.BeepMks(new[] { _a4, _p1, _a4, _p1, _a4, _p1, _a4, _p1, _a4 }); break;
      case 60: await Bpr.BeepMks(new[] { _a4, _p1, _a2, _p1, _a4, _p1, _a4, _p1, _a4, _p1, _a4 }); break;
      default: break;
    }
  }
  #region chimes 1 - 10

  const int _a = 4435, _b = 4699, _c = 4978, _1 = 60000, _2 = 90000, _3 = 120000, _p = 160000; // https://en.wikipedia.org/wiki/Piano_key_frequencies
  static readonly int[]
    _p1 = new[] { 20000, _p },
    _p2 = new[] { 20000, 2 * _p },
    _a2 = new[] { _a, 2 * _p },
    _a4 = new[] { _a, 4 * _p },
    _ad = new[] { _a, _p },
    _bd = new[] { _b, _p },
    _cd = new[] { _c, _p };

  static async Task pause() => await Task.Delay(0);

  #endregion
}