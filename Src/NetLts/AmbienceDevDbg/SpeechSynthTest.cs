global using static System.Diagnostics.Trace;
using System.Diagnostics;
using AmbienceLib;
using Microsoft.Extensions.Configuration;

namespace AmbienceDevDbg;
public static class SpeechSynthTest
{
  public static async Task TestTTS()
  {
    if (!Debugger.IsAttached) return;

    var key = new ConfigurationBuilder().AddUserSecrets<Program>().Build()["AppSecrets:MagicSpeech"] ?? "no key"; //tu: adhoc usersecrets for Console app :: program!!!

    var synth = new SpeechSynth(key, useCached: true);
    await synth.SpeakAsync("Wake Lock released!", voice: "en-US-AriaNeural", style: CC.whispering, role: CC.Girl);;
    await synth.SpeakAsync("Wake Lock released!", voice: "en-US-AriaNeural", style: CC.cheerful, role: CC.Girl);;

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

  static async Task TestAllStylesForTheVoice(string msg, SpeechSynth synth, VoiceStylesRoles rr)
  {
    foreach (var style in rr.Styles)
    {
      WriteLine($"+++            {rr.Voice}  {style}.");
      await synth.SpeakAsync(msg, voice: rr.Voice, style: style, role: "Boy");
    }
  }
}

