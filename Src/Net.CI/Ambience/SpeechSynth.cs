namespace AmbienceLib;
public class SpeechSynth
{
  readonly string _speechKey;
  readonly SoundPlayer _player = new();

  public SpeechSynth(string speechKey) => _speechKey = speechKey;

  public async Task Speak(string text)
  {

    var filename = @$"c:\temp\tts\{RemoveIllegalCharacters(RemoveIllegalCharacters(text))}.wav";

    if (!File.Exists(filename))
    {
      await SynthesizeAudioAsync(text, filename);
    }

    PlayWavFileAsync(filename);
  }

  void PlayWavFileAsync(string filename)
  {
    new SoundPlayer(filename).Play();
    //_player.SoundLocation= filename;
    //_player.Play();
  }

  async Task SynthesizeAudioAsync(string tts, string filename)
  {
    var speechConfig = SpeechConfig.FromSubscription(_speechKey, "canadacentral");
    speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);

    using var synthesizer = new SpeechSynthesizer(speechConfig, null);
    var result = await synthesizer.SpeakTextAsync(tts);

    using var stream = AudioDataStream.FromResult(result);
    await stream.SaveToWaveFileAsync(filename);
  }

  string RemoveIllegalCharacters(string filename = "My:File*Name?")
  {
    var invalidFileNameChars = Path.GetInvalidFileNameChars();

    filename = Regex.Replace(filename, "[" + Regex.Escape(new string(invalidFileNameChars)) + "]", string.Empty);

    return filename;
  }
}
