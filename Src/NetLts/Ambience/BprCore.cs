﻿namespace AmbienceLib;
public partial class Bpr
{
  readonly SoundPlayer _p = new();
  const int d1 = 100_000, _dMin = 75_000; //below .075 sec nogo on razer1.
  const int d2 = d1 + d1;
  readonly int[][] _Beethoven6thWarn = new[] {
    FixDuration(392, d1), FixDuration(10, d1),
    FixDuration(392, d1), FixDuration(10, d1),
    FixDuration(392, d1), FixDuration(10, d1),
    FixDuration(311, d2), FixDuration(10, d2),
    FixDuration(349, d1), FixDuration(10, d1),
    FixDuration(349, d1), FixDuration(10, d1),
    FixDuration(349, d1), FixDuration(10, d1),
    FixDuration(294, d2), FixDuration(10, d2) };
  readonly int[][] _Beethoven6thErr = new[] {
    FixDuration(3920, d1), FixDuration(10, d1),
    FixDuration(3920, d1), FixDuration(10, d1),
    FixDuration(3920, d1), FixDuration(10, d1),
    FixDuration(3110, d2), FixDuration(10, d2),
    FixDuration(3490, d1), FixDuration(10, d1),
    FixDuration(3490, d1), FixDuration(10, d1),
    FixDuration(3490, d1), FixDuration(10, d1),
    FixDuration(2940, d2), FixDuration(10, d2) };
  readonly int[]
     _fd11 = FixDuration(5000, _dMin),
     _fd22 = FixDuration(6000, _dMin * 2),
     _fdw1 = FixDuration(2000, _dMin),
     _fdw2 = FixDuration(2500, _dMin * 2),
     _fd21 = FixDuration(6000, _dMin),
     _fd23 = FixDuration(5000, _dMin * 6);
  readonly ushort _smartVolume = ushort.MaxValue / 20; //  = (ushort)(ushort.MaxValue / (200 - 6 * DateTime.Today.Day)); // 194 - 14

  public async Task BeepHzMks(int[][] HzMks, bool isAsync = true) => await BeepHzMks(HzMks, _smartVolume, isAsync);
  public async Task BeepHzMks(int[][] HzMks, ushort volume, bool isAsync = true)
  {
    if (SuppressTicks || SuppressAlarm) return;

    const double TAU = 2 * Math.PI;
    const int formatChunkSize = 16, headerSize = 8, samplesPerSecond = 44100, waveSize = 4;
    const short formatType = 1, tracks = 1, bitsPerSample = 16, frameSize = tracks * ((bitsPerSample + 7) / 8);
    const int bytesPerSecond = samplesPerSecond * frameSize;

    var ttlmks = HzMks.Sum(r => r[1]);
    var samples = (int)(samplesPerSecond * 0.000001m * ttlmks);
    var dataChunkSize = samples * frameSize;
    var fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;

    using var stream = new MemoryStream();
    using var writer = new BinaryWriter(stream);
    //                         var encoding = new System.Text.UTF8Encoding();
    writer.Write(0x46464952); // = encoding.GetBytes("RIFF")
    writer.Write(fileSize);
    writer.Write(0x45564157); // = encoding.GetBytes("WAVE")
    writer.Write(0x20746D66); // = encoding.GetBytes("fmt ")
    writer.Write(formatChunkSize);
    writer.Write(formatType);
    writer.Write(tracks);
    writer.Write(samplesPerSecond);
    writer.Write(bytesPerSecond);
    writer.Write(frameSize);
    writer.Write(bitsPerSample);
    writer.Write(0x61746164); // = encoding.GetBytes("data")
    writer.Write(dataChunkSize);

    foreach (var hzms in HzMks)
    {
      var hz = hzms[0];
      long ms = hzms[1];
      var theta = hz * TAU / samplesPerSecond;

      //WriteLine($"[{DateTime.Now:HH:mm:ss} Trc]  ** Beep():  {hz,8:N0} hz   {ms,8:N0} ms     =>     2Pi *{hz,5} hz  /  {samplesPerSecond} sampl/s  =  {theta:N4}");

      // 'volume' is UInt16 with range 0 thru Uint16.MaxValue (= 65 535);
      // we need 'amp' to have   range 0 thru  Int16.MaxValue (= 32 767);
      double amp = volume >> 2; // so we simply set amp = volume / 2
      var stepCount = (int)(samples * ms / ttlmks);
      for (var step = 0; step < stepCount; step++)
      {
        //WriteLine((short)(amp * Math.Sin(theta * step)));
        writer.Write((short)(amp * Math.Sin(theta * step)));
      }
    }

    _ = stream.Seek(0, SeekOrigin.Begin);
    _p.Stream = stream;

    try //todo: the exceptions are not being caught here!!!
    {
      if (isAsync)
      {
        _p.Play();       //
        await Task.Delay(ttlmks / 1000).ConfigureAwait(false);
      }
      else
      {
        _p.PlaySync(); // <== pauses all animations!!!
      }
    }
    catch (Exception ex) { WriteLine($"Hz: {HzMks.Length} tones   {ex.Message}"); if (Debugger.IsAttached) Debugger.Break(); }

    writer.Close();
    stream.Close();
  }
}