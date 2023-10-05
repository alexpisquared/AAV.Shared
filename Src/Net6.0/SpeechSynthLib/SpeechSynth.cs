using AAV.Sys.Ext;
using AAV.Sys.Helpers;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SpeechSynthLib
{
  [Obsolete(@"Use this: C:\g\AAV.Shared\Src\NetLts\Ambience\SpeechSynth.cs", true)]  
  public class SpeechSynth : IDisposable
  {
    const double _speakingRate = 1.25;
    const string _voiceNameFallback = "en-AU-WilliamNeural", _rgn = "canadacentral", _key = "use proper key here";
    readonly Random _rnd = new(DateTime.Now.Millisecond);
    readonly AzureSpeechCredentials? _asc;
    readonly IConfigurationRoot _cfg;
    readonly bool _azureTtsIsPK, _useSayExe = true;
    SpeechSynthesizer? _synthesizer = null;
    bool _disposedValue;

    public SpeechSynth() 
    {
      try
      {
        _cfg = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.SpeechSynthLib.json").AddUserSecrets<SpeechSynth>().Build(); //tu: appsets + secrets.
        _asc = JsonIsoFileSerializer.Load<AzureSpeechCredentials>();

        var isOk = (_asc?.Rgn == _rgn && _asc?.Key.StartsWith("bdefa0157d1d") == true);
        Trace.WriteLine(isOk ? "■++ Auth Cred OK" : "■ ■ ■ missing Auth Cred");

#if !StillInitializing // supply the key + publish + delete + commit.
        if (!isOk)
        {
          JsonIsoFileSerializer.Save<AzureSpeechCredentials>(new AzureSpeechCredentials { Key = _key, Rgn = _rgn });
          _asc = JsonIsoFileSerializer.Load<AzureSpeechCredentials>();
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
      }
      catch (Exception ex)
      {
        ex.Log($"Find the lates  AzureSpeechCredentials.json  on  '{Environment.MachineName}'  and provide the key from the one with it.");
        try { JsonIsoFileSerializer.Save<AzureSpeechCredentials>(new AzureSpeechCredentials { Key = _key, Rgn = _rgn }); } catch (Exception ex2) { ex2.Log("Not sure ...."); }
      }
      finally { _azureTtsIsPK = _asc?.Rgn == _rgn; }
    }

    SpeechSynthesizer synthesizer => _synthesizer ??= new SpeechSynthesizer(SpeechConfig.FromSubscription(_asc.Key, _asc.Rgn));

    public async Task SpeakAsync(string msg, VMode vmode = VMode.Prosody, string? voice = null, string? styleForExpressOnly = null)
    {
      try
      {
        if (_useSayExe) { UseSayExe(msg); return; } // 2021-03-17 - testing price drop

        if (!_azureTtsIsPK) { Trace.WriteLine("■ ■ ■ No Azure keys ==> using  say.exe  instead ..."); UseSayExe(msg); return; }

        //    std voices from https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#standard-voices
        // neural voices from https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#neural-voices

        var speakingStyles = new[] { // https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/speech-synthesis-markup?tabs=csharp#adjust-speaking-styles
           "angry" ,         // XiaoxiaoNeural only
           "sad" ,           // XiaoxiaoNeural only
           "affectionate",   // XiaoxiaoNeural only
           "newscast-formal" ,  // AriaNeural only 
           "newscast-casual" ,  // AriaNeural only 
           "customerservice" ,  // AriaNeural only 
           "chat"            ,  // AriaNeural only 
           "cheerful"        ,  // AriaNeural only 
           "empathetic"     };  // AriaNeural only 

        var sStyle =
          styleForExpressOnly == "random" ? speakingStyles[_rnd.Next(speakingStyles.Length)] :
          speakingStyles.Contains(styleForExpressOnly) ? styleForExpressOnly :
          "chat";

        var voiceName = voice ?? _voiceNameFallback;
        var lang = "en-US"; // voiceName.Length > 5 ? voiceName.Substring(0, 5) : "en-GB";
        var sw = Stopwatch.StartNew();
        using var result = await synthesizer.SpeakSsmlAsync(vmode == VMode.Prosody ?
          $@" <speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""{lang}""                                              ><voice name=""{voiceName}""><prosody rate=""{_speakingRate}""                      >{msg}</prosody></voice></speak>" : //todo: rate does not work below:
          $@" <speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""{lang}"" xmlns:mstts=""https://www.w3.org/2001/mstts""><voice name=""{voiceName}""><mstts:express-as style=""{sStyle}"" styledegree=""2"" >{msg}</mstts:express-as></voice></speak>");

        Trace.Write($"{DateTimeOffset.Now:yy.MM.dd HH:mm:ss.f}\t{vmode}\t{sw.Elapsed.TotalSeconds,4:N1}s\t{voiceName,-26}\t WhereAmI SpeechSynth'{_cfg["WhereAmI"]}'  ■ ■ {msg,-44}");

        if (result.Reason == ResultReason.Canceled)
        {
          var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
          Trace.Write($"\tCANCELED: {cancellation.Reason}");

          if (cancellation.Reason == CancellationReason.Error)
          {
            Trace.Write($"  Error: {cancellation.ErrorCode}-{cancellation.ErrorDetails}");
            UseSayExe(msg);
          }
        }
        else
          Trace.Write($"  result: '{result.Reason}'");

        Trace.Write("\n");
      }
      catch (Win32Exception ex) { ex.Log(@"say.exe, right?"); UseSayExe(msg); }
      catch (Exception ex) { ex.Log(@"Not sure again"); }
    }

    static void UseSayExe(string msg) => new Process { StartInfo = new ProcessStartInfo("say.exe", $"\"{msg}\"") { RedirectStandardError = true, UseShellExecute = false } }.Start(); 
    public void StopSpeakingAsync() => synthesizer.StopSpeakingAsync();

    protected virtual void Dispose(bool disposing)
    {
      if (!_disposedValue)
      {
        if (disposing)
        {
          _synthesizer?.Dispose();
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

  public enum VMode { Unknown, Prosody, Express }
}/* 
2020-12-17 
//todo: for more voice manipulations see https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/speech-synthesis-markup?tabs=csharp&source=docs
?is it worth the effort: Get started with Custom Voice https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/how-to-custom-voice

https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#text-to-speech

f8653a65f32d4bdefa0157d1d4547958
65f32d4bdefa0157d1f8653ad4547958
bdefa0157d1d4547958f8653a65f32d4

2022-12-30 - revisiting/retracing steps to play cached content instead of say.exe:
Quick Start: https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/get-started-text-to-speech?tabs=windows%2Cterminal&pivots=programming-language-csharp
Synthesize speech to a file ++: https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/how-to-speech-synthesis?tabs=browserjs%2Cterminal&pivots=programming-language-csharp
Microsoft Cognitive Services Speech SDK Samples: https://learn.microsoft.com/en-us/samples/azure-samples/cognitive-services-speech-sdk/sample-repository-for-the-microsoft-cognitive-services-speech-sdk/

  2023-10-05 ^^ 
*/