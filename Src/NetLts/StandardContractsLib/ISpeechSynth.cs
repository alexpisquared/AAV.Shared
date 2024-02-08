namespace AmbienceLib;

public interface ISpeechSynth
{
  const string _voice = "Microsoft Zira Desktop";

  void Dispose();
  void SpeakAsyncCancelAll();
  Task SpeakAsync(string msg, double speakingRate = 1, double volumePercent = 33, string voice = "", string style = "friendly", string role = "YoungAdultFemale");
  void SpeakFAF(string msg, double speakingRate = 1, double volumePercent = 33, string voice = "", string style = "friendly");
  void SpeakFreeFAF(string msg, int speakingRate = 3, int volumePercent = 33, string voice = _voice);    // does not use the Azure Cognitive Services.
  Task SpeakFreeAsync(string msg, int speakingRate = 3, int volumePercent = 33, string voice = _voice);  // does not use the Azure Cognitive Services.
}