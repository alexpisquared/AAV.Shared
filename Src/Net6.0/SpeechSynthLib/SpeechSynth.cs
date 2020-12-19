using AAV.Sys.Ext;
using AAV.Sys.Helpers;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SpeechSynthLib
{
  public class SpeechSynth : IDisposable
  {
    const string _rgn = "canadacentral", _key = "use proper key here";
    readonly AzureSpeechCredentials _asc;
    readonly string[] _voiceNames;
    readonly string _voiceNameWait;
    readonly bool _azureTtsIsPK;
    string _voiceNameRand;
    SpeechSynthesizer _synth = null;
    bool _disposedValue;
    Random _rnd = new Random(DateTime.Now.Millisecond);
    int _idx = 0;

    public SpeechSynth()
    {
      try
      {
        var config = new ConfigurationBuilder()
          .SetBasePath(AppContext.BaseDirectory)
          .AddJsonFile("appsettings.json")
          .AddUserSecrets<SpeechSynth>().Build();

        _voiceNames = config.GetSection("VoiceNames").Get<string[]>(); // needs Microsoft.Extensions.Configuration.Binder
        _voiceNameWait = config["VoiceNameWait"];

        _asc = JsonIsoFileSerializer.Load<AzureSpeechCredentials>();

        if (_asc?.Rgn == _rgn)
          return;

#if StillInitializing // supply the key + publish + delete + commit.
        JsonIsoFileSerializer.Save<AzureSpeechCredentials>(new AzureSpeechCredentials { Key = _key, Rgn = _rgn });
        _asc = JsonIsoFileSerializer.Load<AzureSpeechCredentials>();
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
      }
      catch (Exception ex) { ex.Log(); }
      finally { _azureTtsIsPK = _asc?.Rgn == _rgn; }
    }

    public SpeechSynthesizer SynthReal => _synth ??= new SpeechSynthesizer(SpeechConfig.FromSubscription(_asc.Key, _asc.Rgn));

    public async Task SpeakAsync(string msg, string mode = "")
    {
      try
      {
        if (!_azureTtsIsPK)
        {
          new Process { StartInfo = new ProcessStartInfo("say.exe", msg) { RedirectStandardError = true, UseShellExecute = false } }.Start();
          return;
        }

        //    std voices from https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#standard-voices
        // neural voices from https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#neural-voices

        var styles = new[] { // https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/speech-synthesis-markup?tabs=csharp#adjust-speaking-styles
           "newscast-formal" ,  // Expresses a formal, confident and authoritative tone for news delivery
           "newscast-casual" ,  // Expresses a versatile and casual tone for general news delivery
           "customerservice" ,  // Expresses a friendly and helpful tone for customer support
           "chat"            ,  // Expresses a casual and relaxed tone
           "cheerful"        ,  // Expresses a positive and happy tone
           "empathetic"     };  // Expresses a sense of caring and understanding


        _voiceNameRand = _voiceNames[_rnd.Next(_voiceNames.Length)];
        var sw = Stopwatch.StartNew();
        using var result = await SynthReal.SpeakSsmlAsync(
          mode == "Faf" ? // randomly rotating voices
          $@"
        <speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""en-US"">
          <voice name=""{_voiceNameRand}"">
            <prosody rate=""1.4"">
              {msg}
            </prosody>
          </voice>
        </speak>"
          : mode == "Say voice name" ?
          $@"
        <speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""en-US"">
          <voice name=""{_voiceNameRand}"">
            <prosody rate=""1.4"">
              {msg}, <break time=""100ms""/> {_voiceNameRand.Substring(6).Replace("Neural", " Neural").Replace("RUS", " russian")}.
            </prosody>
          </voice>
        </speak>"
          :           // async wait
          $@"
        <speak version=""1.0"" xmlns=""http://www.w3.org/2001/10/synthesis"" xmlns:mstts=""https://www.w3.org/2001/mstts"" xml:lang=""en-US"">
          <voice name=""{_voiceNameWait}"">
            <mstts:express-as style=""{styles[_idx++ % styles.Length]}"" rate=""1.4"">{msg}</mstts:express-as>
          </voice>
        </speak>");

        Trace.Write($"{DateTimeOffset.Now:yy.MM.dd HH:mm:ss.f}\t{mode[0]}\t{sw.Elapsed.TotalSeconds,6:N1} sec \t{(mode == "Faf" ? _voiceNameRand : _voiceNameWait),-26}\t{msg,-44}");

        if (result.Reason == ResultReason.Canceled)
        {
          var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
          Trace.Write($"\tCANCELED: Reason={cancellation.Reason}");

          if (cancellation.Reason == CancellationReason.Error)
            Trace.Write($"   ErrorCode={cancellation.ErrorCode}   ErrorDetails=[{cancellation.ErrorDetails}]   Did you update the subscription info?");
        }
        else
          Trace.Write($"  result: '{result.Reason}'");

        Trace.Write("\n");
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


https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#text-to-speech

*/