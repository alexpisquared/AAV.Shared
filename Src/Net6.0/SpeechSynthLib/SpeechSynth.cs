using AAV.Sys.Ext;
using AAV.Sys.Helpers;
using Microsoft.CognitiveServices.Speech;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SpeechSynthLib
{
  public class SpeechSynth : IDisposable
  {
    const string _rgn = "canadacentral";
    readonly AzureSpeechCredentials _asc;
    readonly bool _azure;
    SpeechSynthesizer _synth = null;
    bool _disposedValue;

#if !seeding
    public SpeechSynth()
    {
      try
      {
        _asc = JsonIsoFileSerializer.Load<AzureSpeechCredentials>();

        if (_asc?.Rgn == _rgn)
          return;

        JsonIsoFileSerializer.Save<AzureSpeechCredentials>(new AzureSpeechCredentials { Key = "egcmk-hdjcdh-gh56bh3-w5nncn-b3d4", Rgn = _rgn });
        _asc = JsonIsoFileSerializer.Load<AzureSpeechCredentials>();
      }
      catch (Exception ex) { ex.Log(); }
      finally { _azure = _asc?.Rgn == _rgn; }
    }
#endif

    public SpeechSynthesizer Synth => _synth ??= new SpeechSynthesizer(SpeechConfig.FromSubscription(_asc.Key, _asc.Rgn));

    public async Task SpeakAsync(string msg)
    {
      try
      {
        if (!_azure)
        {
          new Process { StartInfo = new ProcessStartInfo("say.exe", msg) { RedirectStandardError = true, UseShellExecute = false } }.Start();
          return;
        }

        using var result = await Synth.SpeakTextAsync(msg);

        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
          Trace.WriteLine($"Speech synthesized to speaker for text [{msg}]");
        }
        else if (result.Reason == ResultReason.Canceled)
        {
          var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
          Trace.WriteLine($"CANCELED: Reason={cancellation.Reason}");

          if (cancellation.Reason == CancellationReason.Error)
          {
            Trace.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
            Trace.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
            Trace.WriteLine($"CANCELED: Did you update the subscription info?");
          }
        }
      }
      catch (Exception ex) { ex.Log(); }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!_disposedValue)
      {
        if (disposing)
        {
          _synth.Dispose();
        }

        _disposedValue = true;
      }
    }
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }
  }
}
/*
f8653a65f32d4bdefa0157d1d4547958
65f32d4bdefa0157d1f8653ad4547958
bdefa0157d1d4547958f8653a65f32d4
*/