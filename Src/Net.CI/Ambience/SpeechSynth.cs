namespace AmbienceLib;
public class SpeechSynth : IDisposable
{
  const string _rgn = "canadacentral", _onedrv = @"C:\Users\alexp\OneDrive\Public\AppData\SpeechSynthCache\", _github = @"C:\g\AAV.Shared\Src\Net.CI\Ambience\MUMsgs\",
    vn = "en-GB-SoniaNeural", vs = "whispering", vl = "uk-UA";
  const double sr = 1.25, vp = 33;
  readonly string _voice, _style, _pathToCache;
  readonly ILogger? _lgr;
  readonly SpeechSynthesizer _synthesizer;
  readonly bool _useCached;
  bool _disposedValue;

  public static SpeechSynth Factory(string speechKey, ILogger lgr, bool useCached = true, string voiceNameFallback = vn, string styleFallback = vs, string speechSynthesisLanguage = vl)
  {
    return new SpeechSynth(speechKey, useCached, voiceNameFallback, styleFallback, speechSynthesisLanguage, lgr);
  }
  public SpeechSynth(string speechKey, bool useCached = true, string voiceNameFallback = vn, string styleFallback = vs, string speechSynthesisLanguage = vl, ILogger? lgr = null, string pathToCache = _github)
  {
    _pathToCache = pathToCache;
    _useCached = useCached;
    _voice = voiceNameFallback;
    _style = styleFallback;
    _lgr = lgr;
    
    var speechConfig = SpeechConfig.FromSubscription(speechKey, _rgn);

    speechConfig.SpeechSynthesisLanguage = speechSynthesisLanguage; // seems like no effect ... keep it though for a new language on a new machine usecase to validate; seems like it helps kicking off new ones.

    if (_useCached)
    {
      speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);
      _synthesizer = new SpeechSynthesizer(speechConfig, null);
    }
    else
      _synthesizer = new SpeechSynthesizer(speechConfig);
  }

  public async Task SpeakDefaultAsync(string msg) { await SpeakOr(msg, @$"{_pathToCache}{RemoveIllegalCharacters(RemoveIllegalCharacters(msg))}.wav", _synthesizer.SpeakTextAsync, msg); }

  public void    /**/ SpeakProsodyFAF(string msg, double speakingRate = sr, double volumePercent = vp, string voice = vn) => _ = Task.Run(() => SpeakProsodyAsync(msg, speakingRate, volumePercent, voice));
  public async Task SpeakProsodyAsync(string msg, double speakingRate = sr, double volumePercent = vp, string voice = vn)
  {
    var file = @$"{_pathToCache}{RemoveIllegalCharacters(voice)}~{speakingRate:N2}~{volumePercent:0#}~{RemoveIllegalCharacters(msg)}.wav";
    var ssml = $@"<speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""{(voice.Length > 5 ? voice[..5] : "en-US")}""><voice name=""{voice}""><prosody volume=""{volumePercent}"" rate=""{speakingRate}"">{msg}</prosody></voice></speak>";
    await SpeakOr(ssml, file, _synthesizer.SpeakSsmlAsync, msg);
  }

  public void    /**/ SpeakExpressFAF(string msg, double speakingRate = sr, double volumePercent = vp, string voice = vn, string style = vs) => _ = Task.Run(() => SpeakExpressAsync(msg, speakingRate, volumePercent, voice, style));
  public async Task SpeakExpressAsync(string msg, double speakingRate = sr, double volumePercent = vp, string voice = vn, string style = vs)
  {
    var file = @$"{_pathToCache}{RemoveIllegalCharacters(voice)}~{speakingRate:N2}~{volumePercent:0#}~{RemoveIllegalCharacters(style)}~{RemoveIllegalCharacters(msg)}.wav";
    var ssml = $@"<speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""{(voice.Length > 5 ? voice[..5] : "en-US")}"" xmlns:mstts=""https://www.w3.org/2001/mstts""><voice name=""{voice}""><prosody volume=""{volumePercent}"" rate=""{speakingRate}""><mstts:express-as style=""{style}"" styledegree=""2"" >{msg}</mstts:express-as></prosody></voice></speak>";
    await SpeakOr(ssml, file, _synthesizer.SpeakSsmlAsync, msg);
  }

  async Task SpeakOr(string msg, string file, Func<string, Task<SpeechSynthesisResult>> action, string? orgMsg = null)
  {
    if (_useCached && File.Exists(file) && new FileInfo(file).Length > 10)
    {
      PlayWavFileAsync(file);
    }
    else if (await SpeakPlus(msg, file, action) == false)
      UseSayExe(orgMsg ?? msg);
  }
  async Task<bool> SpeakPlus(string msg, string file, Func<string, Task<SpeechSynthesisResult>> act)
  {
    try
    {
      using var result = await act(msg);
      if (_useCached)
      {
        return await CreateWavFile(file, result);
      }
    }
    catch (Exception ex) { WriteLine($"■■■ {ex.Message}"); if (Debugger.IsAttached) Debugger.Break(); else throw; }

    return false;
  }

  async Task<bool> CreateWavFile(string file, SpeechSynthesisResult result)
  {
    if (result.Reason == ResultReason.Canceled)
    {
      var cancellationDetails = SpeechSynthesisCancellationDetails.FromResult(result);
      if (cancellationDetails.Reason == CancellationReason.Error)
      {
        Write($"  result.Reason: '{result.Reason}'  Error: {cancellationDetails.ErrorCode}-{cancellationDetails.ErrorDetails}\n");
      }
    }

    using var stream = AudioDataStream.FromResult(result);

    var temp = Path.GetTempFileName();
    await stream.SaveToWaveFileAsync(temp); // :does not like foreign chars ==>  ^^ + >>
    if (new FileInfo(temp).Length > 10)
    {
      File.Move(temp, file);
      PlayWavFileAsync(file);
    }
    else
    {
      UseSayExe($"Bad key or wav-file length is zero.");
      _lgr?.Log(LogLevel.Warning, $"result.Reason: {result.Reason}    Bad key or wav-file length is zero.   {file} ");
      await Task.Delay(2500);
      return false;
    }

    return true;
  }

  void PlayWavFileAsync(string file) => new SoundPlayer(file).PlaySync();//_player.SoundLocation= file;//_player.Play();
  string RemoveIllegalCharacters(string file) => Regex.Replace(file, "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]", "·");

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

  public async Task SayExe(string msg) { UseSayExe(msg); await Task.Yield(); }
  public static void UseSayExe(string msg) => new Process
  {
    StartInfo = new ProcessStartInfo("say.exe", msg)
    {
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      UseShellExecute = false,
      CreateNoWindow = true               // Set command to run in a hidden window
    }
  }.Start();
}
public record VoiceStylesRoles(string Voice, string[] Styles, string[] Roles);
public class CC
{
  public static VoiceStylesRoles
    UkuaPolinaNeural = new("uk-UA-PolinaNeural", Array.Empty<string>(), Array.Empty<string>()),
    UkuaOstapNeural = new("uk-UA-OstapNeural", Array.Empty<string>(), Array.Empty<string>()),
    EngbRyanNeural = new("en-GB-RyanNeural", new[] { cheerful, chat }, Array.Empty<string>()),
    EnusAriaNeural = new("en-US-AriaNeural", new[] { cheerful, chat, customerservice, narration_professional, newscast_casual, newscast_formal, empathetic, angry, sad, excited, friendly, terrified, shouting, unfriendly, whispering, hopeful }, Array.Empty<string>()),
    EngbSoniaNeural = new("en-GB-SoniaNeural", new[] { cheerful, sad }, Array.Empty<string>()),
    ZhcnXiaomoNeural = new("zh-CN-XiaomoNeural", new[] { cheerful, embarrassed, calm, fearful, disgruntled, serious, angry, sad, depressed, affectionate, gentle, envious }, new[] { YoungAdultFemale, YoungAdultMale, OlderAdultFemale, OlderAdultMale, SeniorFemale, SeniorMale, Girl, Boy });

  const string
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