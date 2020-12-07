using System.Speech.Synthesis;

namespace AAV.Sph.Core
{
  public class SpeechSynth
  {
    bool _isSpeechOn = false;
    SpeechSynthesizer _synth;

    public SpeechSynth(bool isOn) => _isSpeechOn = isOn;

    public SpeechSynthesizer Synth
    {
      get
      {
        if (_synth == null)
        {
          _synth = new SpeechSynthesizer { Rate = 6, Volume = 50 };
          _synth.SelectVoiceByHints(gender: VoiceGender.Female);
        }

        return _synth;
      }
    }

    public bool IsSpeechOn { get => _isSpeechOn; set => _isSpeechOn = value; }

    public void SpeakSynch(string msg) { if (_isSpeechOn) Synth.Speak(msg); }
    public void SpeakAsync(string msg) { if (_isSpeechOn) Synth.SpeakAsync(msg); }
  }
}
