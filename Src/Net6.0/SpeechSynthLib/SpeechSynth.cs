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
    const string _rgn = "canadacentral", _key = "use proper key here";
    readonly AzureSpeechCredentials _asc;
    readonly bool _azureTtsIsPK;
    SpeechSynthesizer _synth = null;
    bool _disposedValue;

#if StillInitializing // supply the key + publish + delete + commit.
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
#elif UseProjSecrets
      //DI:
      Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((ctx, builder) => builder.AddAzureKeyVault("https://demopockv.vault.azure.net/", new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback)), new DefaultKeyVaultSecretManager())) //tu: !!! MVP for Azure Key Vault utilization !!! 
        .Build().Run();
      IConfiguration _configuration = ...
      var keys = _configuration["CognSvcRsc0-Key-001"];
      var _key = keys.Split(' ')[0];
      var _rgn = keys.Split(' ')[3];
#endif

    public SpeechSynthesizer SynthReal => _synth ??= new SpeechSynthesizer(SpeechConfig.FromSubscription(_asc.Key, _asc.Rgn));

    int _s = 0, _v = 0;
    public async Task SpeakAsync(string msg, string k = "")
    {
      try
      {
        if (!_azureTtsIsPK)
        {
          new Process { StartInfo = new ProcessStartInfo("say.exe", msg) { RedirectStandardError = true, UseShellExecute = false } }.Start();
          return;
        }

        var styles = new[] { // https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/speech-synthesis-markup?tabs=csharp#adjust-speaking-styles
           "newscast-formal"   ,  // Expresses a formal, confident and authoritative tone for news delivery
           "newscast-casual"   ,  // Expresses a versatile and casual tone for general news delivery
           "customerservice"   ,  // Expresses a friendly and helpful tone for customer support
           "chat"              ,  // Expresses a casual and relaxed tone
           "cheerful"          ,  // Expresses a positive and happy tone
           "empathetic" };        // Expresses a sense of caring and understanding

        var voices = new[] { // https://docs.microsoft.com/en-us/azure/cognitive-services/containers/container-image-tags?tabs=current
          "en-gb-george-apollo",  // Container image with the en-GB locale and en-GB-George-Apollo voice.	
          "en-gb-hazel",    //??
          "en-gb-hazelrus",       // Container image with the en-GB locale and en-GB-HazelRUS voice. 
          "en-gb-hazelNeural",
          "en-US-Guy",      //??
          "en-US-Guy24kRUS",
          "en-US-GuyNeural",
          "en-US-Aria",     //??
          "en-US-AriaRUS",
          "en-US-AriaNeural",
          "zh-CN-Xiaoxiao", //??
          "zh-CN-XiaoxiaoNeural",
          "zh-CN-Xiaomo",   //??
          "zh-CN-XiaomoNeural",
          "en-gb-susan-apollo"};	// Container image with the en-GB locale and en-GB-Susan-Apollo voice.	

        using var result = await SynthReal.SpeakSsmlAsync(
k == "Faf" ? // std voice from https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#standard-voices
$@"
<speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""en-US"">
  <voice name=""{voices[_v % voices.Length]}"">
    <prosody rate=""1.4"">
      {msg}, <break time=""100ms""/> {voices[_v++ % voices.Length].Substring(4)}.
    </prosody>
  </voice>
</speak>"
:                   // neural voices from https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#neural-voices
$@"
<speak version=""1.0"" xmlns=""http://www.w3.org/2001/10/synthesis"" xmlns:mstts=""https://www.w3.org/2001/mstts"" xml:lang=""en-US"">
  <voice name=""en-US-AriaNeural"">
    <mstts:express-as style=""{styles[_s % styles.Length]}"">
      {msg}, <break time=""100ms""/> {styles[_s++ % styles.Length]}.
    </mstts:express-as>
  </voice>
</speak>");

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

2020-12-17 
//todo: for more voice manipulations see https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/speech-synthesis-markup?tabs=csharp&source=docs
?is it worth the effort: Get started with Custom Voice https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/how-to-custom-voice



*/