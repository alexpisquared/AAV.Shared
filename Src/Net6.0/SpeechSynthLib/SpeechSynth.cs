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
    const string _rgn = "canadacentral", _key= "use proper key here";
    readonly AzureSpeechCredentials _asc;
    readonly bool _azureTtsIsPK;
    SpeechSynthesizer _synth = null;
    bool _disposedValue;

#if StillInitializing
    public SpeechSynth()
    {
      try
      {
        _asc = JsonIsoFileSerializer.Load<AzureSpeechCredentials>();

        if (_asc?.Rgn == _rgn)
          return;

        JsonIsoFileSerializer.Save<AzureSpeechCredentials>(new AzureSpeechCredentials { Key = _key, Rgn = _rgn });
        _asc = JsonIsoFileSerializer.Load<AzureSpeechCredentials>();
      }
      catch (Exception ex) { ex.Log(); }
      finally { _azureTtsIsPK = _asc?.Rgn == _rgn; }
    }
#endif

    public SpeechSynthesizer SynthReal => _synth ??= new SpeechSynthesizer(SpeechConfig.FromSubscription(_asc.Key, _asc.Rgn));

    public async Task SpeakAsync(string msg)
    {
      try
      {
        if (!_azureTtsIsPK)
        {
          new Process { StartInfo = new ProcessStartInfo("say.exe", msg) { RedirectStandardError = true, UseShellExecute = false } }.Start();
          return;
        }

        using var result = await SynthReal.SpeakTextAsync(msg);

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
/* f8653a65f32d4bdefa0157d1d4547958
65f32d4bdefa0157d1f8653ad4547958
bdefa0157d1d4547958f8653a65f32d4
*/