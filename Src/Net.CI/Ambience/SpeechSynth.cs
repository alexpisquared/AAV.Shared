namespace AmbienceLib;
public class SpeechSynth : IDisposable
{
  const string _voiceNameFallback = "uk-UA-PolinaNeural", _rgn = "canadacentral", _key = "use proper key here", _pathToCache = @"c:\temp\tts\";
  readonly string _speechKey;
  readonly SpeechSynthesizer _synthesizer;
  readonly bool _useCached;
  bool _disposedValue;

  public SpeechSynth(string speechKey, bool useCached = true, string speechSynthesisLanguage = "uk-UA")
  {
    _useCached = useCached;

    var speechConfig = SpeechConfig.FromSubscription(_speechKey = speechKey, "canadacentral");

    speechConfig.SpeechSynthesisLanguage = speechSynthesisLanguage;

    if (_useCached)
    {
      speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);
      _synthesizer = new SpeechSynthesizer(speechConfig, null);
    }
    else
      _synthesizer = new SpeechSynthesizer(speechConfig);
  }

  public async Task SpeakAsync(string msg)
  {
    var file = @$"{_pathToCache}{RemoveIllegalCharacters(RemoveIllegalCharacters(msg))}.wav";
    await SpeakOr(msg, file, _synthesizer.SpeakTextAsync);
  }
  public async Task SpeakProsodyAsync(string msg, string voice = _voiceNameFallback, double speakingRate = 1.5)
  {
    var file = @$"{_pathToCache}{RemoveIllegalCharacters(voice)}~{speakingRate}~{RemoveIllegalCharacters(msg)}.wav";
    var ssml = $@"<speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""{(voice.Length > 5 ? voice[..5] : "en-US")}""><voice name=""{voice}""><prosody rate=""{speakingRate}"">{msg}</prosody></voice></speak>";
    await SpeakOr(ssml, file, _synthesizer.SpeakSsmlAsync);
  }
  public async Task SpeakExpressAsync(string msg, string voice = _voiceNameFallback, string style = "cheerful")
  {
    var file = @$"{_pathToCache}{RemoveIllegalCharacters(voice)}~{RemoveIllegalCharacters(style)}~{RemoveIllegalCharacters(msg)}.wav";
    var ssml = $@"<speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""{(voice.Length > 5 ? voice[..5] : "en-US")}"" xmlns:mstts=""https://www.w3.org/2001/mstts""><voice name=""{voice}""><mstts:express-as style=""{style}"" styledegree=""2"" >{msg}</mstts:express-as></voice></speak>";
    await SpeakOr(ssml, file, _synthesizer.SpeakSsmlAsync);
  }

  async Task SpeakOr(string msg, string file, Func<string, Task<SpeechSynthesisResult>> action)
  {
    if (_useCached)
    {
      if (!File.Exists(file))
        await SpeakPlus(msg, file, action);

      PlayWavFileAsync(file);
    }
    else
    {
      await SpeakPlus(msg, file, action);
    }
  }
  async Task SpeakPlus(string msg, string file, Func<string, Task<SpeechSynthesisResult>> act)
  {
    try
    {
      using var result = await act(msg);
      if (_useCached)
      {
        await CreateWavFile(file, result);
      }
    }
    catch (Exception ex) { WriteLine($"■■■ {ex.Message}"); if (Debugger.IsAttached) Debugger.Break(); else throw; }
  }

  static async Task CreateWavFile(string file, SpeechSynthesisResult result)
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
    File.Move(temp, file);
  }

  void PlayWavFileAsync(string file) => new SoundPlayer(file).PlaySync();//_player.SoundLocation= file;//_player.Play();
  string RemoveIllegalCharacters(string file) => Regex.Replace(file, "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]", string.Empty);

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
/*
2022-12-30 - revisiting/retracing steps to play cached content instead of say.exe:
Quick Start: https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/get-started-msg-to-speech?tabs=windows%2Cterminal&pivots=programming-language-csharp
Synthesize speech to a file ++: https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/how-to-speech-synthesis?tabs=browserjs%2Cterminal&pivots=programming-language-csharp
Microsoft Cognitive Services Speech SDK Samples: https://learn.microsoft.com/en-us/samples/azure-samples/cognitive-services-speech-sdk/sample-repository-for-the-microsoft-cognitive-services-speech-sdk/


See C:\g\ScSv\Src\AlexPi.Scr\TtsVoicesPocConsoleApp\VoiceTester.cs for detail.
 
 
https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support?tabs=stt-tts#voice-styles-and-roles 


*/

public record Voice_styles_and_roles(string Voice, string[] Styles, string[] Roles);
public class CC
{
  public static Voice_styles_and_roles
    UkuaOstapNeural = new("uk-UA-OstapNeural", new string[] { }, new string[] { }),
    UkuaPolinaNeural = new("uk-UA-PolinaNeural", new string[] { }, new string[] { }),
    EngbRyanNeural = new("en-GB-RyanNeural", new[] { cheerful, chat }, new string[] { }),
    EngbSoniaNeural = new("en-GB-SoniaNeural", new[] { cheerful, sad }, new string[] { }),
    EnusAriaNeural = new("en-US-AriaNeural", new[] { chat, customerservice, narration_professional, newscast_casual, newscast_formal, cheerful, empathetic, angry, sad, excited, friendly, terrified, shouting, unfriendly, whispering, hopeful }, new string[] { }),
    ZhcnXiaomoNeural = new("zh-CN-XiaomoNeural", new[] { embarrassed, calm, fearful, cheerful, disgruntled, serious, angry, sad, depressed, affectionate, gentle, envious }, new[] { YoungAdultFemale, YoungAdultMale, OlderAdultFemale, OlderAdultMale, SeniorFemale, SeniorMale, Girl, Boy });

  const string
    embarrassed = "embarrassed",
    calm = "calm",
    fearful = "fearful",
    cheerful = "cheerful",
    disgruntled = "disgruntled",
    serious = "serious",
    angry = "angry",
    sad = "sad",
    depressed = "depressed",
    affectionate = "affectionate",
    gentle = "gentle",
    envious = "envious",
    chat = "chat",
    customerservice = "customerservice",
    narration_professional = "narration-professional",
    newscast_casual = "newscast-casual",
    newscast_formal = "newscast-formal",
    empathetic = "empathetic",
    excited = "excited",
    friendly = "friendly",
    terrified = "terrified",
    shouting = "shouting",
    unfriendly = "unfriendly",
    whispering = "whispering",
    hopeful = "hopeful",

    YoungAdultFemale = "YoungAdultFemale",
    YoungAdultMale = "YoungAdultMale",
    OlderAdultFemale = "OlderAdultFemale",
    OlderAdultMale = "OlderAdultMale",
    SeniorFemale = "SeniorFemale",
    SeniorMale = "SeniorMale",
    Girl = "Girl",
    Boy = "Boy";
}