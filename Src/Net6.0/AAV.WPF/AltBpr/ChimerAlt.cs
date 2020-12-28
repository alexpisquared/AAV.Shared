﻿using AAV.Sys.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AAV.WPF.AltBpr
{
  public class ChimerAlt
  {
    static readonly IConfigurationRoot _config;
    static readonly int _freqUp, _freqDn, _wakeMks;
    static readonly int[] _freqs;
    static ChimerAlt()
    {
      _config = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json")
        .AddUserSecrets<ChimerAlt>()
        .Build();

      _freqs = _config.GetSection("Freqs").Get<int[]>();
      _freqUp = _config.GetSection("FreqUp").Get<int?>() ?? 32;
      _freqDn = _config.GetSection("FreqDn").Get<int?>() ?? 8000;
      _wakeMks = _config.GetSection("WakeMks").Get<int?>() ?? 8000;

      Tracer.SetupTracingOptions("AAV.WPF", new TraceSwitch("Info", "Haha.") { Level = TraceLevel.Info });
      Trace.Write($"{DateTimeOffset.Now:yy.MM.dd HH:mm:ss.f}\t\t WhereAmI " +
        $"'{_config["WhereAmIString"]}'  " +
        $"'{_config["WhereAmIArray"]}'  " +
        $"'{_config.GetSection("WhereAmIString").Get<string>()}'  " +
        $"'{_config.GetSection("WhereAmIArray").Get<string[]>()?.First()}'  " +
        $"\n");
    }

    public static async Task BeepFD(int freq = 98, int durationMks = 250111) => await Bpr.BeepMks(new[] { new[] { freq, Bpr.FixDuration(freq, durationMks) } });
    public static async Task BeepFD(int freq = 98, double durationSec = .25) => await Bpr.BeepMks(new[] { new[] { freq, Bpr.FixDuration(freq, (int)(durationSec * 1000000)) } });

    public static async Task FreqWalkUp() => await PlayFreqList(new[] { _freqDn, _freqUp }, 30, 0.9925);
    public static async Task FreqWalkDn() => await PlayFreqList(new[] { _freqUp, _freqDn }, durationSec: 05, durnMultr: 1.0075);
    public static async Task FreqWalkUpDn() { await FreqWalkUp(); await Task.Delay(333); await FreqWalkDn(); }

    public static async Task FreqRunUpHiPh() => await PlayFreqList(new[] { 4000, 9000 }, durationSec: .3);
    public static async Task FreqRunUpDn() { await FreqRunUp(); await Task.Delay(999); await FreqRunDn(); }
    public static async Task FreqRunUp() => await PlayFreqList(new[] { 48, 100 }, durationSec: 1.9);
    public static async Task FreqRunDn() => await PlayFreqList(new[] { 128, 48 }, durationSec: 1.9);

    public static async Task WakeAudio() => await NoteWalk(100, 101, _wakeMks); // .15 sec is audible?

    public static async Task FreqWalkUpDnUp(double freqA = 200, double freqB = 20, double freqC = 300, double durationSec = 1, double durnMultr = 1, double frMultr = 1.02)
    {
      var freqDurnList = new List<int[]>();

      connectTheDots_Old(freqA, freqB, freqDurnList, durationSec, durnMultr, frMultr);
      connectTheDots_Old(freqB, freqA, freqDurnList, durationSec, durnMultr, frMultr);
      connectTheDots_Old(freqA, freqC, freqDurnList, durationSec, durnMultr, frMultr);

      await Bpr.BeepMks(freqDurnList.ToArray());
    }

    public static async Task PlayFreqList() => await PlayFreqList(_freqs, 1.3);
    public static async Task PlayFreqList(int[] freqs, double durationSec = 1, double durnMultr = 1, double frMultr = 1.02)
    {
      var freqDurnList = new List<int[]>();

      for (var i = 0; i < freqs.Length - 1; i++) { connectTheDots_New(freqs[i], freqs[i + 1], freqDurnList, durationSec, durnMultr, frMultr); }
      await Bpr.BeepMks(freqDurnList.ToArray());
    }
    public static async Task PlayFreqListNew(int[] freqs, double durationSec = 1, double durnMultr = 1, double frMultr = 1.02)
    {
      var freqDurnList = new List<int[]>();

      for (var i = 0; i < freqs.Length - 1; i++) { connectTheDots_New(freqs[i], freqs[i + 1], freqDurnList, durationSec, durnMultr, frMultr); }
      await Bpr.BeepMks(freqDurnList.ToArray());
    }
    public static async Task PlayFreqListOld(int[] freqs, double durationSec = 1, double durnMultr = 1, double frMultr = 1.02)
    {
      var freqDurnList = new List<int[]>();

      for (var i = 0; i < freqs.Length - 1; i++) { connectTheDots_Old(freqs[i], freqs[i + 1], freqDurnList, durationSec, durnMultr, frMultr); }
      await Bpr.BeepMks(freqDurnList.ToArray());
    }

    public static void connectTheDots_New(double freqA, double freqB, List<int[]> freqDurnList, double durationSec = 1, double durnMultr = 1, double frMultr = 1.02)
    {
      const double stepDurnSec = 0.001;
      var stepDurnMks = stepDurnSec * 1e6;

      var stepsTtlCnt = (int)(durationSec / stepDurnSec);

      var stepMultiplier = Math.Pow(freqB / freqA, 1.0 / stepsTtlCnt);
      var freq = freqA;

      for (var i = 0; i <= stepsTtlCnt; i++)
      {
        Console.Write($"  {1 + i,4}.  Playing {freq,14:N1} hz  for {stepDurnSec} sec      NEW     ");
        fixDurnAndAdd((int)stepDurnMks, (int)Math.Round(freq), freqDurnList);
        freq *= stepMultiplier;
      }

      Console.WriteLine($"   NEW ===> {(freqDurnList.Sum(r => r[1]) * .000001):N3} sec");
    }
    public static void connectTheDots_Old(double freqA, double freqB, List<int[]> freqDurnList, double durationSec = 1, double durnMultr = 1, double frMultr = 1.02)
    {
      var up = freqA < freqB;
      var stepsTtlCnt = (int)(1.0 * Math.Log(up ? freqB / freqA : freqA / freqB, frMultr));
      var stepDurnMks = 1000000 * durationSec / stepsTtlCnt;
      Console.WriteLine($"Steps: {stepsTtlCnt,8}   Step Duration: {stepDurnMks:N1} mks  => total time requested / actual: {durationSec} / {stepsTtlCnt * stepDurnMks * .000001}.");

      for (double freq = freqA, j = 1;
        up ? freq <= freqB : freq >= freqB;
        freq = up ? freq * frMultr : freq / frMultr, j++)
      {
        Console.Write($"  {1 + j,4}.  Playing {freq,14:N1} hz  for {(stepDurnMks / 1e6):N3} sec      old     ");
        fixDurnAndAdd((int)(stepDurnMks * Math.Pow(durnMultr, j)), (int)Math.Round(freq), freqDurnList);
      }

      Console.WriteLine($"   old ===> {(freqDurnList.Sum(r => r[1]) * .000001):N3} sec");
    }

    public static async Task NoteWalk(int noteA = 108, int noteB = 11, int durationMks = 60000, double delta = 1)
    {
      var fullScale = new List<int[]>();

      if (durationMks < 10000)
        durationMks *= 1000;

      if (noteA < noteB)
        for (int note = noteA, j = 1; note < noteB; note++, j++) add((int)(durationMks * Math.Pow(delta, j)), fullScale, note);
      else
        for (int note = noteA, j = 1; note > noteB; note--, j++) add((int)(durationMks * Math.Pow(delta, j)), fullScale, note);

      await Bpr.BeepMks(fullScale.ToArray());

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
        case 01: await Bpr.BeepMks(new[] { new[] { _a, _p + _p } }); break;
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
}