namespace AmbienceDevDbg;
public class SpeechSynthTest
{
  readonly string key;
  private SpeechSynth synth;

  public SpeechSynthTest()
  {
    key = new ConfigurationBuilder().AddUserSecrets<Program>().Build()["AppSecrets:MagicSpeech"] ?? "no key"; //tu: adhoc usersecrets for Console app :: program!!!
  }
  public static async Task TestTTS()
  {
    SpeechSynthTest sst = new();

    await sst.TestFree();
  }
  public async Task TestFree()
  {
    synth = new SpeechSynth(key, useCached: false);
    await synth.SpeakFreeAsync0("A big brown dog jumped over the green crocodile.");
  }
  public async Task Test1()
  {
    if (!Debugger.IsAttached) return;


    synth = new SpeechSynth(key, useCached: true);
    await synth.SpeakAsync("Wake Lock released!", voice: "en-US-AriaNeural", style: CC.whispering, role: CC.Girl); ;
    await synth.SpeakAsync("Wake Lock released!", voice: "en-US-AriaNeural", style: CC.cheerful, role: CC.Girl); ;

    await TestAllStylesForTheVoice("Time to rotate! 是时候轮换了！", synth, CC.ZhcnXiaomoNeural);
    await TestAllStylesForTheVoice("Last minute! 最后一分钟！", synth, CC.ZhcnXiaomoNeural);

    if (Debugger.IsAttached) return;

    synth = new SpeechSynth(key, useCached: false);
    await synth.SpeakAsync("Time to rotate! 是时候轮换了！", role: CC.YoungAdultFemale);
    await synth.SpeakAsync("Time to rotate! 是时候轮换了！", role: CC.YoungAdultFemale);
    await synth.SpeakAsync("Time to rotate! 是时候轮换了！", role: CC.YoungAdultMale);
    await synth.SpeakAsync("Time to rotate! 是时候轮换了！", role: CC.OlderAdultFemale);
    await synth.SpeakAsync("Time to rotate! 是时候轮换了！", role: CC.OlderAdultMale);
    await synth.SpeakAsync("Time to rotate! 是时候轮换了！", role: CC.SeniorFemale);
    await synth.SpeakAsync("Time to rotate! 是时候轮换了！", role: CC.SeniorMale);
    await synth.SpeakAsync("Time to rotate! 是时候轮换了！", role: CC.Girl);
    await synth.SpeakAsync("Time to rotate! 是时候轮换了！", role: CC.Boy);
  }

  async Task TestAllStylesForTheVoice(string msg, SpeechSynth synth, VoiceStylesRoles rr)
  {
    foreach (var style in rr.Styles)
    {
      WriteLine($"+++            {rr.Voice}  {style}.");
      await synth.SpeakAsync(msg, voice: rr.Voice, style: style, role: "Boy");
    }
  }
}
/* ReadMe:

C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.wav" -ab 16 "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.16.wav"                    : no size changes
C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.wav" -c:a pcm_s16le -ar 44100 "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.s16.wav" : doubled the rate
C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.wav" -c:a libx264 -crf 24 "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.x24.wav"     : no such codec

C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.wav" -c:a libopus "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.opus"                : 13kb
C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.wav" -c:a aac     "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.aac"                 : 15kb

C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.wav" -c:a libopus -b:a 16k "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.16k.opus"   :  4kb still good  <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.wav" -c:a aac     -b:a 16k "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.16k.aac"    :  4kb noticeable degradation

C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.wav" -c:a libopus -b:a 8k "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.8k.opus"     :  2kb garbled
C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.wav" -c:a aac     -b:a 8k "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.8k.aac"      :  2kb garbled

C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.wav" -c:a libopus -b:a 4k "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.4k.opus"     :  2kb garbled
C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.wav" -c:a aac     -b:a 4k "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.4k.aac"      :  2kb garbled

C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "Good - Fanfare.wav" -c:a libopus -b:a 32k "Good - Fanfare.32k.opus"
C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "Good - Fanfare.wav" -c:a libopus -b:a 16k "Good - Fanfare.16k.opus" : still good
C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "Good - Fanfare.wav" -c:a libopus -b:a 8k  "Good - Fanfare.8k.opus"  : BAD
C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "Good - Fanfare.wav" -c:a libopus          "Good - Fanfare.opus"
C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "Good - Fanfare.wav" -c:a aac              "Good - Fanfare.aac"

-q:a 2 sets the audio quality, where -q:a 0 is the highest quality and -q:a 9 is the lowest. 
C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "Good - Fanfare.wav" -c:a libmp3lame -q:a 9 "Good - Fanfare.9.mp3" : 25kb  slightly muffled
C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "Good - Fanfare.wav" -c:a libmp3lame -q:a 7 "Good - Fanfare.7.mp3" : 29kb  perfect
C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "Good - Fanfare.wav" -c:a libmp3lame -q:a 5 "Good - Fanfare.5.mp3" : 33kb  perfect
C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "Good - Fanfare.wav" -c:a libmp3lame -q:a 2 "Good - Fanfare.2.mp3"
C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "Good - Fanfare.wav" -c:a libmp3lame -q:a 2 "Good - Fanfare.2.mp3"

FOR %%A IN (*.wav) DO (
    ffmpeg.exe -i "%%A" -c:a libopus -b:a 16k "%%~nA.16k.opus"
)

FOR %A IN (*.wav) DO C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "%A" -c:a libopus -b:a 16k  "%~nA.16k.opus"
FOR %A IN (*.wav) DO C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "%A" -c:a aac     -b:a 32k  "%~nA.32k.aac"
FOR %A IN (*.wav) DO C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "%A" -c:a libmp3lame -q:a 7 "%~nA.7.mp3"

FOR %A IN (*.wav) DO C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "%A" -c:a libmp3lame -q:a 7 "%~nA.7.mp3"
  ===
Get-ChildItem *.wav | Foreach-Object {
    & 'C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe' -i $_.FullName -c:a libmp3lame -q:a 7 "$($_.BaseName).7.mp3"
}

C:\gh\yt\Vividl\Vividl\Lib\ffmpeg.exe -i "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.wav" -c:a aac     -b:a 32k "en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.32k.aac"    

en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.wav
en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.16.wav

en-US-AriaNeural~1.00~100~whispering~Wake Lock released!.wav

zh-CN-XiaomoNeural~1.00~100~angry~Last minute! EQAQJ！.wav
zh-CN-XiaomoNeural~1.00~100~calm~Last minute! EQAQJ！.wav
zh-CN-XiaomoNeural~1.00~100~cheerful~Last minute! EQAQJ！.wav
zh-CN-XiaomoNeural~1.00~100~gentle~Last minute! EQAQJ！.wav
zh-CN-XiaomoNeural~1.00~100~sad~Last minute! EQAQJ！.wav
zh-CN-XiaomoNeural~1.00~100~serious~Last minute! EQAQJ！.wav

zh-CN-XiaomoNeural~1.00~100~angry~Time to rotate! DYRGOE！.wav
zh-CN-XiaomoNeural~1.00~100~calm~Time to rotate! DYRGOE！.wav
zh-CN-XiaomoNeural~1.00~100~cheerful~Time to rotate! DYRGOE！.wav
zh-CN-XiaomoNeural~1.00~100~gentle~Time to rotate! DYRGOE！.wav
zh-CN-XiaomoNeural~1.00~100~sad~Time to rotate! DYRGOE！.wav
zh-CN-XiaomoNeural~1.00~100~serious~Time to rotate! DYRGOE！.wav
 */