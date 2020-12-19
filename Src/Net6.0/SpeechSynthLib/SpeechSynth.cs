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
    Random _rnd = new Random(DateTime.Now.Millisecond);

    public SpeechSynth()
    {
      try
      {
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

        //    std voices from https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#standard-voices
        // neural voices from https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#neural-voices

        var styles = new[] { // https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/speech-synthesis-markup?tabs=csharp#adjust-speaking-styles
           "newscast-formal" ,  // Expresses a formal, confident and authoritative tone for news delivery
           "newscast-casual" ,  // Expresses a versatile and casual tone for general news delivery
           "customerservice" ,  // Expresses a friendly and helpful tone for customer support
           "chat"            ,  // Expresses a casual and relaxed tone
           "cheerful"        ,  // Expresses a positive and happy tone
           "empathetic"     };  // Expresses a sense of caring and understanding

        var voices = new[] { 
          // https://docs.microsoft.com/en-us/azure/cognitive-services/containers/container-image-tags?tabs=current
          //"en-gb-george-apollo",  // Container image with the en-GB locale and en-GB-George-Apollo voice.	
          //"en-US-GuyNeural",
          //"en-US-Aria",           
          //"en-US-AriaRUS",
          //"en-gb-susan-apollo",
          // https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#text-to-speech:
          // StandardVoices 
          //foreign "ar-EG-Hoda", "ar-SA-Naayf", 
          //silent "bg-BG-Ivan", "ca-ES-HerenaRUS", 
          //"zh-HK-Danny", "zh-HK-TracyRUS", 
          //"zh-CN-HuihuiRUS", "zh-CN-Kangkang", "zh-CN-Yaoyao", "zh-TW-HanHanRUS", "zh-TW-Yating", "zh-TW-Zhiwei", "hr-HR-Matej", "cs-CZ-Jakub", "da-DK-HelleRUS", "nl-NL-HannaRUS",
          //"en-AU-Catherine", "en-AU-HayleyRUS", "en-CA-HeatherRUS", "en-CA-Linda", "en-IN-Heera", "en-IN-PriyaRUS", "en-IN-Ravi", "en-IE-Sean", "en-GB-George", "en-GB-HazelRUS", "en-GB-Susan", "en-US-BenjaminRUS", "en-US-GuyRUS", "en-US-JessaRUS", "en-US-ZiraRUS",
          //"fi-FI-HeidiRUS", "fr-CA-Caroline", "fr-CA-HarmonieRUS", "fr-FR-HortenseRUS", "fr-FR-Julie", "fr-FR-Paul", "fr-CH-Guillaume", "de-AT-Michael", "de-DE-HeddaRUS", "de-DE-Stefan", "de-CH-Karsten", "el-GR-Stefanos", "he-IL-Asaf", "hi-IN-Hemant", "hi-IN-Kalpana", "hu-HU-Szabolcs", "id-ID-Andika", "it-IT-Cosimo", "it-IT-LuciaRUS",
          //"ja-JP-Ayumi", "ja-JP-HarukaRUS", "ja-JP-Ichiro", "ko-KR-HeamiRUS", "ms-MY-Rizwan", "nb-NO-HuldaRUS", "pl-PL-PaulinaRUS", "pt-BR-Daniel", "pt-BR-HeloisaRUS", "pt-PT-HeliaRUS", "ro-RO-Andrei", "ru-RU-EkaterinaRUS", "ru-RU-Irina", "ru-RU-Pavel", "sk-SK-Filip", "sl-SI-Lado", "es-MX-HildaRUS", "es-MX-Raul", "es-ES-HelenaRUS", "es-ES-Laura", "es-ES-Pablo",
          //"sv-SE-HedvigRUS", "ta-IN-Valluvar", "te-IN-Chitra", "th-TH-Pattara", "tr-TR-SedaRUS", "vi-VN-An",
          // Neural Voices:
          //"ar-EG-SalmaNeural", "ar-EG-ShakirNeural", "ar-SA-ZariyahNeural", "ar-SA-HamedNeural", "bg-BG-KalinaNeural", "bg-BG-BorislavNeural", "ca-ES-AlbaNeural", "ca-ES-JoanaNeural", "ca-ES-EnricNeural",
          //"zh-HK-HiuGaaiNeural", "zh-HK-HiuMaanNeural", "zh-HK-WanLungNeural", "zh-CN-XiaoxiaoNeural", "zh-CN-XiaoyouNeural", "zh-CN-YunyangNeural", "zh-CN-YunyeNeural", "zh-TW-HsiaoChenNeural", "zh-TW-HsiaoYuNeural", "zh-TW-YunJheNeural", "hr-HR-GabrijelaNeural", "hr-HR-SreckoNeural", "cs-CZ-VlastaNeural", "cs-CZ-AntoninNeural", "da-DK-ChristelNeural", "da-DK-JeppeNeural",
          //"nl-NL-ColetteNeural", "nl-NL-FennaNeural", "nl-NL-MaartenNeural", "en-AU-NatashaNeural", "en-AU-WilliamNeural", "en-CA-ClaraNeural", "en-CA-LiamNeural", "en-IN-NeerjaNeural", "en-IN-PrabhatNeural", "en-IE-EmilyNeural", "en-IE-ConnorNeural", 
          //"en-GB-LibbyNeural", 
          "en-GB-MiaNeural", "en-GB-RyanNeural", "en-US-AriaNeural", "en-US-JennyNeural", "en-US-GuyNeural", //"fi-FI-NooraNeural", "fi-FI-SelmaNeural", "fi-FI-HarriNeural",
          //"fr-CA-SylvieNeural", "fr-CA-JeanNeural", "fr-FR-DeniseNeural", "fr-FR-HenriNeural", "fr-CH-ArianeNeural", "fr-CH-FabriceNeural", "de-AT-IngridNeural", "de-AT-JonasNeural", "de-DE-KatjaNeural", "de-DE-ConradNeural", "de-CH-LeniNeural", "de-CH-JanNeural", "el-GR-AthinaNeural", "el-GR-NestorasNeural", "he-IL-HilaNeural", "he-IL-AvriNeural", "hi-IN-SwaraNeural", "hi-IN-MadhurNeural", "hu-HU-NoemiNeural", "hu-HU-TamasNeural",
          //"id-ID-GadisNeural", "id-ID-ArdiNeural", "it-IT-ElsaNeural", "it-IT-IsabellaNeural", "it-IT-DiegoNeural", "ja-JP-NanamiNeural", "ja-JP-KeitaNeural", "ko-KR-SunHiNeural", "ko-KR-InJoonNeural", "ms-MY-YasminNeural", "ms-MY-OsmanNeural", "nb-NO-IselinNeural", "nb-NO-PernilleNeural", "nb-NO-FinnNeural", "pl-PL-AgnieszkaNeural", "pl-PL-ZofiaNeural", "pl-PL-MarekNeural",
          //"pt-BR-FranciscaNeural", "pt-BR-AntonioNeural", "pt-PT-FernandaNeural", "pt-PT-RaquelNeural", "pt-PT-DuarteNeural", "ro-RO-AlinaNeural", "ro-RO-EmilNeural", 
          "ru-RU-DariyaNeural", "ru-RU-SvetlanaNeural", "ru-RU-DmitryNeural"//, "sk-SK-ViktoriaNeural", "sk-SK-LukasNeural", "sl-SI-PetraNeural", "sl-SI-RokNeural", "es-MX-DaliaNeural", "es-MX-JorgeNeural", "es-ES-ElviraNeural", "es-ES-AlvaroNeural",
          //"sv-SE-HilleviNeural", "sv-SE-SofieNeural", "sv-SE-MattiasNeural", "ta-IN-PallaviNeural", "ta-IN-ValluvarNeural", "te-IN-ShrutiNeural", "te-IN-MohanNeural", "th-TH-AcharaNeural", "th-TH-PremwadeeNeural", "th-TH-NiwatNeural", "tr-TR-EmelNeural", "tr-TR-AhmetNeural", "vi-VN-HoaiMyNeural", "vi-VN-NamMinhNeural"
        };

        var voiceName = voices[_rnd.Next(voices.Length)];
        var sw = Stopwatch.StartNew();
        using var result = await SynthReal.SpeakSsmlAsync(
k == "Faf" ? // randomly rotating voices
$@"
<speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""en-US"">
  <voice name=""{voiceName}"">
    <prosody rate=""1.4"">
      {msg}
    </prosody>
  </voice>
</speak>"
: k == "Say voice name" ? 
$@"
<speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""en-US"">
  <voice name=""{voiceName}"">
    <prosody rate=""1.4"">
      {msg}, <break time=""100ms""/> {voiceName.Substring(6).Replace("Neural", " Neural").Replace("RUS", " russian")}.
    </prosody>
  </voice>
</speak>"
:           // async wait
$@"
<speak version=""1.0"" xmlns=""http://www.w3.org/2001/10/synthesis"" xmlns:mstts=""https://www.w3.org/2001/mstts"" xml:lang=""en-US"">
  <voice name=""ru-RU-EkaterinaRUS"">
    <mstts:express-as style=""{styles[_s++ % styles.Length]}"" rate=""1.4"">{msg}</mstts:express-as>
  </voice>
</speak>");

        if (k == "Faf")
          Trace.Write($"{DateTimeOffset.Now:yy.MM.dd HH:mm:ss.f} {sw.Elapsed.TotalSeconds,6:N1} sec   Voice: {voiceName,-26}   {msg,-44}");

        //if (result.Reason == ResultReason.SynthesizingAudioCompleted) Trace.Write($"Speech synthesized to speaker for text [{msg}]"); else
        if (result.Reason == ResultReason.Canceled)
        {
          var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
          Trace.Write($"\tCANCELED: Reason={cancellation.Reason}");

          if (cancellation.Reason == CancellationReason.Error)
            Trace.Write($"   ErrorCode={cancellation.ErrorCode}   ErrorDetails=[{cancellation.ErrorDetails}]   Did you update the subscription info?");
        }

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