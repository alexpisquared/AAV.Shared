namespace AmbienceLib;
public class SpeechSynth : IDisposable
{
  const string _rgn = "canadacentral", _vName = "zh-CN-XiaomoNeural", _vStyle = CC.friendly, _vLanguage = "zh-CN", _github = @"C:\g\AAV.Shared\Src\NetLts\Ambience\MUMsgs\", _onedrv = @"C:\Users\alexp\OneDrive\Public\AppData\SpeechSynthCache\";
  const double _speechRate = 1.00, _volumePercent = 100;
  readonly string _pathToCache, _fallbackVoice;
  readonly ILogger? _lgr;
  readonly SpeechSynthesizer _synthesizer;
  readonly bool _useCached;
  bool _disposedValue;

  public static SpeechSynth Factory(string speechKey, ILogger lgr, bool useCached = true, string voice = _vName, string speechSynthesisLanguage = _vLanguage) => new SpeechSynth(speechKey, useCached, _vName, speechSynthesisLanguage, lgr);
  public SpeechSynth(string speechKey, bool useCached = true, string voice = _vName, string speechSynthesisLanguage = _vLanguage, ILogger? lgr = null, string pathToCache = _github)
  {
    _fallbackVoice = voice; //todo: 
    _pathToCache = pathToCache;
    _useCached = useCached;
    _lgr = lgr;

    var speechConfig = SpeechConfig.FromSubscription(speechKey, _rgn);  // Error in case of bad key:  AuthenticationFailure-WebSocket upgrade failed: Authentication error (401). Please check subscription information and region name. USP state: 2. Received audio size: 0 bytes.

    speechConfig.SpeechSynthesisLanguage = speechSynthesisLanguage;     // seems like no effect ... keep it though for a new language on a new machine usecase to validate; seems like it helps kicking off new ones.

    if (_useCached)
    {
      speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);
    }

    _synthesizer = _useCached ? new SpeechSynthesizer(speechConfig, null) : new SpeechSynthesizer(speechConfig);

    try { if (Directory.Exists(pathToCache) == false) Directory.CreateDirectory(pathToCache); } catch (Exception ex) { _lgr?.Log(LogLevel.Error, $"■■■ {ex.Message}"); }
  }

  public void    /**/ SpeakFAF(string msg, double speakingRate = _speechRate, double volumePercent = _volumePercent, string voice = "", string style = _vStyle) => _ = Task.Run(() => SpeakAsync(msg, speakingRate, volumePercent, voice, style));
  public async Task SpeakAsync(string msg, double speakingRate = _speechRate, double volumePercent = _volumePercent, string voice = "", string style = _vStyle, string role = CC.YoungAdultFemale)
  {
    if (voice == "") voice = _fallbackVoice;

    var file = @$"{_pathToCache}{(voice)}~{speakingRate:N2}~{volumePercent:0#}~{(style)}~{RemoveIllegalCharacters(msg)}.wav";
    var lang = (voice.Length > 5 ? voice[..5] : "en-US");
    var ssml = style == "" ?
      $@"<speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""{lang}""                                              ><voice name=""{voice}""><prosody volume=""{volumePercent}"" rate=""{speakingRate}""                                                       role=""{role}"">{msg}</prosody></voice></speak>" :
      $@"<speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""{lang}"" xmlns:mstts=""https://www.w3.org/2001/mstts""><voice name=""{voice}""><prosody volume=""{volumePercent}"" rate=""{speakingRate}""><mstts:express-as style=""{style}"" styledegree=""2"" role=""{role}"">{msg}</mstts:express-as></prosody></voice></speak>";

    await SpeakOr(ssml, file, _synthesizer.SpeakSsmlAsync, msg);
  }

  async Task SpeakOr(string msg, string file, Func<string, Task<SpeechSynthesisResult>> speak, string? orgMsg = null)
  {
    if (_useCached && File.Exists(file) && new FileInfo(file).Length > 10)
      PlayWavFileSync(file);
    else if (await SpeakPlusCreateWavFile(msg, file, speak) == false)
      SpeakFree(orgMsg ?? msg);
  }
  async Task<bool> SpeakPlusCreateWavFile(string msg, string file, Func<string, Task<SpeechSynthesisResult>> speak)
  {
    try
    {
      using var result = await speak(msg);
      return (_useCached) ? await CreateWavFile(file, result) : true;
    }
    catch (Exception ex) { _lgr?.Log(LogLevel.Warning, $"■■■ {ex.Message}"); if (Debugger.IsAttached) Debugger.Break(); /*else throw;*/ }

    return false;
  }
  async Task<bool> CreateWavFile(string file, SpeechSynthesisResult result)
  {
    if (result.Reason != ResultReason.Canceled)
      _lgr?.Log(LogLevel.Trace, $"tts result Reason '{result.Reason}'   Created wav file '{file}'.");
    else
    {
      var cancellationDetails = SpeechSynthesisCancellationDetails.FromResult(result);
      if (cancellationDetails.Reason == CancellationReason.Error)
        _lgr?.Log(LogLevel.Warning, $"tts  {result.Reason}  {cancellationDetails.ErrorCode}  {cancellationDetails.ErrorDetails.Replace("\n", "  ")}  :CreateWavFile({file})");
    }

    using var stream = AudioDataStream.FromResult(result);

    var temp = Path.GetTempFileName();
    await stream.SaveToWaveFileAsync(temp); // :does not like foreign chars, like ·, etc.
    var len = new FileInfo(temp).Length;
    if (len > 10)
    {
      File.Move(temp, file);
      PlayWavFileSync(file);
      return true;
    }
    else
    {
      _lgr?.Log(LogLevel.Warning, $"Wav-file length is {len}. Check the key. ");
      await new Bpr().NoAsync();
      File.Delete(temp);
      return false;
    }
  }
  void PlayWavFileSync(string file) => new SoundPlayer(file).PlaySync();//_player.SoundLocation= file;//_player.Play();
  string RemoveIllegalCharacters(string file)
  {
    var r1 = Regex.Replace(file, "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]", "·");
    var r2 = SubstitureUnicodeSymbolsWithAbc(r1);
    return r2;
  }
  string SubstitureUnicodeSymbolsWithAbc(string input)
  {
    var result = new StringBuilder();
    foreach (var c in input)
    {
      if (c is >= (char)0x4e00 and <= (char)0x9fa5)
      {
        var index = (c - 0x4e00) % 26;
        _ = result.Append((char)('A' + index));
      }
      else
      {
        _ = result.Append(c);
      }
    }
    return result.ToString();
  }

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

  public static async Task SpeakFreeAsync(string msg) {    SpeakFree(msg);    await Task.Delay(msg.Length * 100);  }
  public static void SpeakFree(string msg) => new System.Speech.Synthesis.SpeechSynthesizer().Speak(msg);
  [Obsolete("Use ..SpeakFree()")]
  public static void SayExe(string msg) => new Process
  {
    StartInfo = new ProcessStartInfo("say.exe", msg)
    {
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      UseShellExecute = false,        //todo: use true if An error occurred trying to start process 'C:\Users\alexp\OneDrive\Documents\0\Ltd\Invoicing' with working directory 'c:\temp'. Access is denied.   
      CreateNoWindow = true               // Set command to run in a hidden window
    }
  }.Start();
  public void SpeakAsyncCancelAll() { }
}
public record VoiceStylesRoles(string Voice, string[] Styles, string[] Roles);
public class CC
{
  public static VoiceStylesRoles
    UkuaPolinaNeural = new("uk-UA-PolinaNeural", Array.Empty<string>(), Array.Empty<string>()),
    UkuaOstapNeural = new("uk-UA-OstapNeural", Array.Empty<string>(), Array.Empty<string>()),
    EnusAriaNeural = new("en-US-AriaNeural", new[] { cheerful, chat, customerservice, narration_professional, newscast_casual, newscast_formal, empathetic, angry, sad, excited, friendly, terrified, shouting, unfriendly, whispering, hopeful }, Array.Empty<string>()),
    EngbRyanNeural = new("en-GB-RyanNeural", new[] { cheerful, chat }, Array.Empty<string>()),
    EngbSoniaNeural = new("en-GB-SoniaNeural", new[] { cheerful, sad }, Array.Empty<string>()),
    ZhcnXiaomoNeural = new("zh-CN-XiaomoNeural", new[] { cheerful, embarrassed, calm, fearful, disgruntled, serious, angry, sad, depressed, affectionate, gentle, envious }, new[] { YoungAdultFemale, YoungAdultMale, OlderAdultFemale, OlderAdultMale, SeniorFemale, SeniorMale, Girl, Boy });

  public const string
    affectionate = "affectionate",
    angry = "angry",
    calm = "calm",
    chat = "chat",
    cheerful = "cheerful",
    customerservice = "customerservice",
    disgruntled = "disgruntled",
    depressed = "depressed",
    envious = "envious",
    excited = "excited",
    embarrassed = "embarrassed",
    empathetic = "empathetic",
    fearful = "fearful",
    friendly = "friendly",
    gentle = "gentle",
    hopeful = "hopeful",
    narration_professional = "narration-professional",
    newscast_casual = "newscast-casual",
    newscast_formal = "newscast-formal",
    sad = "sad",
    serious = "serious",
    shouting = "shouting",
    terrified = "terrified",
    unfriendly = "unfriendly",
    whispering = "whispering",

    YoungAdultFemale = "YoungAdultFemale",
    YoungAdultMale = "YoungAdultMale",
    OlderAdultFemale = "OlderAdultFemale",
    OlderAdultMale = "OlderAdultMale",
    SeniorFemale = "SeniorFemale",
    SeniorMale = "SeniorMale",
    Girl = "Girl",
    Boy = "Boy";
}

/*
Quickstart 
  https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/get-started-speech-to-text?tabs=windows%2Cterminal&pivots=programming-language-csharp
  https://learn.microsoft.com/en-us/azure/cognitive-services/cognitive-services-apis-create-account?tabs=multiservice%2Canomaly-detector%2Clanguage-service%2Ccomputer-vision%2Cwindows#get-the-keys-for-your-resource

2022-12-30 - revisiting/retracing steps to play cached content instead of say.exe:
Quick Start: https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/get-started-msg-to-speech?tabs=windows%2Cterminal&pivots=programming-language-csharp
Synthesize speech to a file ++: https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/how-to-speech-synthesis?tabs=browserjs%2Cterminal&pivots=programming-language-csharp
Microsoft Cognitive Services Speech SDK Samples: https://learn.microsoft.com/en-us/samples/azure-samples/cognitive-services-speech-sdk/sample-repository-for-the-microsoft-cognitive-services-speech-sdk/

See C:\g\ScSv\Src\AlexPi.Scr\TtsVoicesPocConsoleApp\VoiceTester.cs for detail.
 
https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support?tabs=stt-tts#voice-styles-and-roles 

*/