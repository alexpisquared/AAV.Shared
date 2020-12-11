using AAV.Sys.Helpers;
using Microsoft.CognitiveServices.Speech;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SpeechSynthLib
{
  public class SpeechSynth : IDisposable
  {
    SpeechSynthesizer _synth = null;
    readonly AzureSpeechCredentials _asc = JsonIsoFileSerializer.Load<AzureSpeechCredentials>();
    bool _disposedValue;

#if seeding
    public SpeechSynth() =>      JsonIsoFileSerializer.Save<AzureSpeechCredentials>(new AzureSpeechCredentials { Key = "provide the key", Rgn = "most likely canadacentral" });
#endif

    public SpeechSynthesizer Synth => _synth ??= new SpeechSynthesizer(SpeechConfig.FromSubscription(_asc.Key, _asc.Rgn));

    public async Task SpeakAsync(string msg)
    {
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
