﻿namespace AmbienceLib;
public class SpeechSynth : IDisposable
{
  const string _voiceNameFallback = "uk-UA-PolinaNeural", _rgn = "canadacentral", _key = "use proper key here", _pathToCache = @"c:\temp\tts\";
  readonly string _speechKey;
  readonly SpeechSynthesizer _synthesizer;
  bool _disposedValue, useCached = true;

  public SpeechSynth(string speechKey)
  {
    var speechConfig = SpeechConfig.FromSubscription(_speechKey = speechKey, "canadacentral");

    if (useCached)
    {
      speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);
      _synthesizer = new SpeechSynthesizer(speechConfig, null);
    }
    else
      _synthesizer = new SpeechSynthesizer(speechConfig);
  }

  public async Task SpeakAsync(string msg)
  {
    if (useCached)
    {

      var filename = @$"{_pathToCache}{RemoveIllegalCharacters(RemoveIllegalCharacters(msg))}.wav";
      if (!File.Exists(filename))
        await SynthesizeAudioAsync(msg, filename);

      PlayWavFileAsync(filename);
    }
    else
    {
      await SynthesizeAudioAsync(msg);
    }
  }
  public async Task SpeakProsodyAsync(string msg, string voiceName = _voiceNameFallback, double speakingRate = 1.25)
  {
    var filename = @$"{_pathToCache}{RemoveIllegalCharacters(voiceName)}~{speakingRate}~{RemoveIllegalCharacters(msg)}.wav";
    if (!File.Exists(filename))
      await SynthesizeAudioProsodyAsync(msg, voiceName, speakingRate, filename);

    PlayWavFileAsync(filename);
  }
  public async Task SpeakExpressAsync(string msg, string voiceName = _voiceNameFallback, string style = "cheerful")
  {
    var filename = @$"{_pathToCache}{RemoveIllegalCharacters(voiceName)}~{RemoveIllegalCharacters(style)}~{RemoveIllegalCharacters(msg)}.wav";
    if (!File.Exists(filename))
      await SynthesizeAudioExpressAsync(msg, voiceName, style, filename);

    PlayWavFileAsync(filename);
  }

  async Task SynthesizeAudioAsync(string msg, string filename)
  {
    try
    {
      var result = await _synthesizer.SpeakTextAsync(msg);
      using var stream = AudioDataStream.FromResult(result);
      await stream.SaveToWaveFileAsync(filename);
    }
    catch (Exception ex) { WriteLine($"■■■ {ex.Message}"); if (Debugger.IsAttached) Debugger.Break(); else throw; }
  }
  async Task SynthesizeAudioAsync(string msg)
  {
    try
    {
      var result = await _synthesizer.SpeakTextAsync(msg);
    }
    catch (Exception ex) { WriteLine($"■■■ {ex.Message}"); if (Debugger.IsAttached) Debugger.Break(); else throw; }
  }

  async Task SynthesizeAudioProsodyAsync(string msg, string voice, double speakingRate, string filename)
  {
    try
    {
      var lang = voice.Length > 5 ? voice[..5] : "en-US";

      using var result = await _synthesizer.SpeakSsmlAsync($@"<speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""{lang}""><voice name=""{voice}""><prosody rate=""{speakingRate}"">{msg}</prosody></voice></speak>");

      if (result.Reason == ResultReason.Canceled)
      {
        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
        Write($"\tCANCELED: {cancellation.Reason}");

        if (cancellation.Reason == CancellationReason.Error)
        {
          Write($"  Error: {cancellation.ErrorCode}-{cancellation.ErrorDetails}");
        }
      }
      else
        Write($"  result: '{result.Reason}'");

      Write("\n");

      using var stream = AudioDataStream.FromResult(result);
      await stream.SaveToWaveFileAsync(filename);
    }
    catch (Win32Exception) { throw; }
    catch (Exception) { throw; }
  }
  async Task SynthesizeAudioExpressAsync(string msg, string voice, string style, string filename)
  {
    try
    {
      var lang = voice.Length > 5 ? voice[..5] : "en-US";

      using var result = await _synthesizer.SpeakSsmlAsync(
        $@" <speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""{lang}"" xmlns:mstts=""https://www.w3.org/2001/mstts""><voice name=""{voice}""><mstts:express-as style=""{style}"" styledegree=""2"" >{msg}</mstts:express-as></voice></speak>");

      if (result.Reason == ResultReason.Canceled)
      {
        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
        Write($"\tCANCELED: {cancellation.Reason}");

        if (cancellation.Reason == CancellationReason.Error)
        {
          Write($"  Error: {cancellation.ErrorCode}-{cancellation.ErrorDetails}");
        }
      }
      else
        Write($"  result: '{result.Reason}'");

      Write("\n");

      using var stream = AudioDataStream.FromResult(result);

      var temp = Path.GetTempFileName();
      await stream.SaveToWaveFileAsync(temp);
      File.Move(temp, filename);
    }
    catch (Exception ex) { WriteLine($"■■■ {ex.Message}"); if (Debugger.IsAttached) Debugger.Break(); else throw; }
  }

  void PlayWavFileAsync(string filename) => new SoundPlayer(filename).PlaySync();//_player.SoundLocation= filename;//_player.Play();
  string RemoveIllegalCharacters(string filename = "My:File*Name?")
  {
    filename = Regex.Replace(filename, "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]", string.Empty);
    return filename;
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