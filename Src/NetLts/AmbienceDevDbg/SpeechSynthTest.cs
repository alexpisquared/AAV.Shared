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

    var key = new ConfigurationBuilder().AddUserSecrets<Program>().Build()["AppSecrets:MagicSpeech"] ?? "no key"; //tu: adhoc usersecrets

    //var ays = 
    //  //"你确定吗?"; // == Are you sure   filename: OUYZ
    //  //"Daddy's hungry. Тато голодний. 爸爸饿了";
    //  "Last minute!";
    // "Wake Lock is off"

    await TestLanguage("Last minute! 最后一分钟！", new SpeechSynth(key, useCached: false));

    //await synth.SpeakAsync(msg, CC.EnusAriaNeural.Voice);
    //await synth.SpeakAsync(msg, CC.UkuaPolinaNeural.Voice);
    //await synth.SpeakAsync(msg, CC.ZhcnXiaomoNeural.Voice);
  }

  static async Task TestLanguage(string msg, SpeechSynth synth)
  {
    await synth.SpeakAsync("Last minute! 最后一分钟！", voice: "zh-CN-XiaomoNeural", style: "serious", role: "SeniorMale");

    if (Debugger.IsAttached) return;

    await TestAllStylesForTheVoice(msg, synth, CC.ZhcnXiaomoNeural);
    //await TestAllStylesForTheVoice(msg, synth, CC.EnusAriaNeural);
    //await synth.SpeakAsync(msg, voice: "en-US-AriaNeural", style: "terrified", speakingRate: .01); // same as .50
  }

  static async Task TestAllStylesForTheVoice(string msg, SpeechSynth synth, VoiceStylesRoles rr)
  {
    foreach (var style in rr.Styles)
    {
      WriteLine($"+++            {rr.Voice}  {style}.");
      await synth.SpeakAsync(msg, voice: rr.Voice, style: style);
    }
  }
}

