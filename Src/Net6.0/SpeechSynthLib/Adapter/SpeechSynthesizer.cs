using System.Diagnostics;
using System.Threading.Tasks;

namespace SpeechSynthLib.Adapter
{
  public class SpeechSynthesizer // Adapter to the old .Net 4.8 one
  {
    SpeechSynth _synth;

    public int Rate { get; set; }
    public int Volume { get; set; }

    public void SpeakAsyncCancelAll() => _synth = new SpeechSynth();
    public void Speak(string msg) => _synth.SpeakAsync(msg).Wait();
    public void SpeakAsync(string msg) => Task.Run(async () => await _synth.SpeakAsync(msg));
    public void SelectVoiceByHints(object gender) => Trace.WriteLine($" throw new NotImplementedException(); {gender}");
  }
}
