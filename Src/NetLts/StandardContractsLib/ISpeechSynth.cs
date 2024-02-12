namespace AmbienceLib;

public interface ISpeechSynth
{
  const string _voice = "Microsoft Zira Desktop";

  void Dispose();
  void SpeakAsyncCancelAll();
  void SpeakFAF(string msg, double speakingRate = 1, double volumePercent = 33, string voice = "", string style = "friendly");
  Task SpeakAsync(string msg, double speakingRate = 1, double volumePercent = 33, string voice = "", string style = "friendly", string role = "YoungAdultFemale");
  void SpeakFreeFAF(string msg, int speakingRate1010 = 3, int volumePercent = 33, string voice = _voice);    // does not use the Azure Cognitive Services.
  Task SpeakFreeAsync(string msg, int speakingRate1010 = 3, int volumePercent = 33, string voice = _voice);  // does not use the Azure Cognitive Services.
}