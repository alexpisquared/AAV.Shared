namespace AmbienceLib;
public partial class SpeechSynth : IDisposable, ISpeechSynth
{
  const string _rgn = "CanadaCentral", _vName = "en-GB-SoniaNeural", _vStyle = CC.friendly, _vLanguage = "zh-CN", _voice = "Microsoft Zira Desktop",
    _github = @"C:\g\AAV.Shared\Src\NetLts\Ambience\MUMsgs\";
  string _onedrv = $@"C:\Users\{Environment.UserName}\OneDrive\Public\AppData\SpeechSynthCache\";
  const double _speechRate = 1.00, _volumePercent = 33;
  const int _rateMinusPlus10 = 3;
  readonly string _pathToCache, _fallbackVoice;
  readonly ILogger? _lgr;
  readonly bool _useCached;
  readonly Bpr _bpr = new();
  readonly SpeechSynthesizer _synthesizer;
  Old.SpeechSynthesizer? speechSynth0;
  bool _disposedValue;
  public record VoiceStylesRoles(string Voice, string[] Styles, string[] Roles);

  public Old.SpeechSynthesizer Synthesizer => speechSynth0 ??= new Old.SpeechSynthesizer();

  public static SpeechSynth Factory(string speechKey, ILogger lgr, bool useCached = true, string voice = _vName, string speechSynthesisLanguage = _vLanguage) => new(speechKey, useCached, voice, speechSynthesisLanguage, lgr);
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

    try { if (Directory.Exists(pathToCache) == false) _ = Directory.CreateDirectory(pathToCache); } catch (Exception ex) { _lgr?.Log(LogLevel.Error, $"■■■ {ex.Message}"); }
  }

  public void SpeakFree(string msg, int speakingRate = 0, int volumePercent = 100) => _ = SpeakFreeTask(msg, speakingRate, volumePercent, _voice, false);
  public void SpeakFreeFAF(string msg, int speakingRate = _rateMinusPlus10, int volumePercent = (int)_volumePercent, string voice = _voice) => _ = SpeakFreeTask(msg, speakingRate, volumePercent, voice, false);
  public async Task SpeakFreeAsync(string msg, int speakingRate = _rateMinusPlus10, int volumePercent = (int)_volumePercent, string voice = _voice) => await SpeakFreeTask(msg, speakingRate, volumePercent, voice, true);
  public void    /**/ SpeakFAF(string msg, double speakingRate = _speechRate, double volumePercent = _volumePercent, string voice = "", string style = _vStyle, string role = CC.YoungAdultFemale) => _ = Task.Run(() => SpeakAsync(msg, speakingRate, volumePercent, voice, style, role));
  public async Task SpeakAsync(string msg, double speakingRate = _speechRate, double volumePercent = _volumePercent, string voice = "", string style = _vStyle, string role = CC.YoungAdultFemale)
  {
    var voicE = voice == "" ? _fallbackVoice : voice;
    var msg2 = msg == "" ? "Empty" : msg;

    var file = role == CC.YoungAdultFemale ?
      $"{_pathToCache}{voicE}~{speakingRate:N2}~{volumePercent:0#}~{style}~{RemoveIllegalCharacters(msg2)}.wav" :
      $"{_pathToCache}{voicE}~{speakingRate:N2}~{volumePercent:0#}~{style}~{role}~{RemoveIllegalCharacters(msg2)}.wav";
    var lang = voicE.Length > 5 ? voicE[..5] : "en-US";
    var ssml = style == "" ?
      $@"<speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""{lang}""                                              ><voice name=""{voicE}""><prosody volume=""{volumePercent}"" rate=""{speakingRate}""                                                       role=""{role}"">{msg2}</prosody></voice></speak>" :
      $@"<speak version=""1.0"" xmlns=""https://www.w3.org/2001/10/synthesis"" xml:lang=""{lang}"" xmlns:mstts=""https://www.w3.org/2001/mstts""><voice name=""{voicE}""><prosody volume=""{volumePercent}"" rate=""{speakingRate}""><mstts:express-as style=""{style}"" styledegree=""2"" role=""{role}"">{msg2}</mstts:express-as></prosody></voice></speak>";

    await SpeakOr(ssml, file, _synthesizer.SpeakSsmlAsync, msg2);
  }

  async Task SpeakOr(string msg, string file, Func<string, Task<SpeechSynthesisResult>> speak, string? orgMsg = null)
  {
    if (_useCached && File.Exists(file) && new FileInfo(file).Length > 10)
      PlayWavFileSync(file);
    else if (await SpeakPlusCreateWavFile(msg, file, speak) == false)
      SpeakFreeFAF(orgMsg ?? msg);
  }
  async Task<bool> SpeakPlusCreateWavFile(string msg, string file, Func<string, Task<SpeechSynthesisResult>> speak)
  {
    try
    {
      var max = 160;
      var fileSafe = file.Length > max ? file[..max] : file;
      using var result = await speak(msg);
      return !_useCached || await CreateWavFile(fileSafe, result);
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
        _lgr?.Log(LogLevel.Warning, $"TTS failed:  Reason:{result.Reason}  ErrorCode:{cancellationDetails.ErrorCode}  Details:{cancellationDetails.ErrorDetails.Replace("\n", "  ")}  :CreateWavFile({file})");
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
      _lgr?.Log(LogLevel.Warning, $"Length is {len} for wav-file  '{file}'. Check the key. ");
      await _bpr.WarnAsync();
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

  public async Task TestMeasureTimedCoeficientForSpeakFreeAsync(string msg)
  {
    _ = Stopwatch.StartNew();

    Synthesizer.Rate = 10; var sw = Stopwatch.StartNew();
    Synthesizer.Speak(msg); Console.WriteLine($"Synch {msg.Length,12} {sw.ElapsedMilliseconds / msg.Length,12} ms   {Synthesizer.Rate,4}");

    for (var i = 10; i >= 6; i--)
    {
      Synthesizer.Rate = i; sw = Stopwatch.StartNew(); Synthesizer.Speak(msg); var el = sw.ElapsedMilliseconds;

      var y = 3659.8 * Math.Pow(Math.E, -0.106 * Synthesizer.Rate) * msg.Length / 48.0;

      Console.WriteLine($"{Synthesizer.Rate,4}  {el,6:N0} - {y,6:N0} = {el - y,6:N0}   {new string('■', (int)(el / msg.Length)),12}");

      Synthesizer.SelectVoice(_voice);
    }

    sw = Stopwatch.StartNew();
    _ = Synthesizer.SpeakAsync(msg);
    Console.WriteLine($"Async {sw.ElapsedMilliseconds,12} ms   {Synthesizer.Rate,4}");
    await Task.Delay(msg.Length * 50);
    Console.WriteLine($"Async {sw.ElapsedMilliseconds,12} ms   {Synthesizer.Rate,4}");
  }
  async Task SpeakFreeTask(string msg, int speakingRate, int volumePercent, string voice, bool isAsync)
  {
    var prev = Synthesizer.Volume;
    Synthesizer.Rate = speakingRate;
    Synthesizer.Volume = volumePercent;
    Synthesizer.SelectVoice(voice);
    _ = Synthesizer.SpeakAsync(msg); // not await-able and not blocking: essentially - FAF.
    if (isAsync)
      await Task.Delay((int)(Math.Pow(Math.E, -0.106 * Synthesizer.Rate) * msg.Length * 3659.8 / 48.0));
    Synthesizer.Volume = prev;
  }

  [Obsolete("Use ..SpeakFreeFAF()")]
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
}/*
Quickstart 
  https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/get-started-speech-to-text?tabs=windows%2Cterminal&pivots=programming-language-csharp
  https://learn.microsoft.com/en-us/azure/cognitive-services/cognitive-services-apis-create-account?tabs=multiservice%2Canomaly-detector%2Clanguage-service%2Ccomputer-vision%2Cwindows#get-the-keys-for-your-resource

2022-12-30 - revisiting/retracing steps to play cached content instead of say.exe:
Quick Start: https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/get-started-msg-to-speech?tabs=windows%2Cterminal&pivots=programming-language-csharp
Synthesize speech to a file ++: https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/how-to-speech-synthesis?tabs=browserjs%2Cterminal&pivots=programming-language-csharp
Microsoft Cognitive Services Speech SDK Samples: https://learn.microsoft.com/en-us/samples/azure-samples/cognitive-services-speech-sdk/sample-repository-for-the-microsoft-cognitive-services-speech-sdk/

See C:\g\ScSv\Src\AlexPi.Scr\TtsVoicesPocConsoleApp\VoiceTester.cs for detail.
 
https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support?tabs=stt-tts#voice-styles-and-roles 


   X              Y
   0             76
   1             67
   2             60
   3             54
   4             48
   5             44
   6             40
   7             35
   8             33
   9             29
  10             26

-10,225,?????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
-9,202,??????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
-8,181,?????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
-7,162,??????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
-6,144,????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
-5,130,??????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
-4,116,????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
-3,104,????????????????????????????????????????????????????????????????????????????????????????????????????????
-2,93,?????????????????????????????????????????????????????????????????????????????????????????????
-1,84,????????????????????????????????????????????????????????????????????????????????????
0,77,?????????????????????????????????????????????????????????????????????????????
1,67,???????????????????????????????????????????????????????????????????
2,60,????????????????????????????????????????????????????????????
3,54,??????????????????????????????????????????????????????
4,49,?????????????????????????????????????????????????
5,44,????????????????????????????????????????????
6,39,???????????????????????????????????????
7,36,????????????????????????????????????
8,33,?????????????????????????????????
9,29,?????????????????????????????
10,27,???????????????????????????

-10,225,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
-9,202,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
-8,181,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
-7,162,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
-6,144,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
-5,130,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
-4,116,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
-3,104,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
-2,93,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
-1,84,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
0,77,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
1,67,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
2,60,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
3,54,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
4,49,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
5,44,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
6,39,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
7,36,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
8,33,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
9,29,■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
10,27,■■■■■■■■■■■■■■■■■■■■■■■■■■■

 -10,               225,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
  -9,               202,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
  -8,               181,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
  -7,               162,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
  -6,               144,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
  -5,               130,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
  -4,               116,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
  -3,               104,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
  -2,                93,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
  -1,                84,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   0,                77,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   1,                67,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   2,                60,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   3,                54,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   4,                49,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   5,                44,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   6,                39,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   7,                36,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   8,                33,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   9,                29,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
  10,                27,             ■■■■■■■■■■■■■■■■■■■■■■■■■■■

Synch           27 ms     10
  10          1327         0.104    ■■■■■■■■■■■■■■■■■■■■■■■■■■■
   9          1276         9.408    ■■■■■■■■■■■■■■■■■■■■■■■■■■
   8          1385        18.712    ■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   7          1545        28.016    ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   6          1729        37.320    ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   5          1931        46.623    ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   4          2153        55.927    ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   3          2370        65.231    ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   2          2630        74.535    ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   1          2944        83.839    ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
   0          3228        93.143    ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
  -1          3696       102.447    ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
  -2          4045       111.751    ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
  -3          4477       121.055    ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
  -4          5015       130.359    ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
  -5          5593       139.662    ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■




 
 
 */