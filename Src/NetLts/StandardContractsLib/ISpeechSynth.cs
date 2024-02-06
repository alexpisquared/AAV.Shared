namespace AmbienceLib;

public interface ISpeechSynth
{
  void Dispose();
  void SpeakAsyncCancelAll();
  Task SpeakAsync(string msg, double speakingRate = 1, double volumePercent = 100, string voice = "", string style = "friendly", string role = "YoungAdultFemale");
  void SpeakFAF(string msg, double speakingRate = 1, double volumePercent = 100, string voice = "", string style = "friendly");
  void SpeakFree(string msg, int volumePercent = 100); // does not use the Azure Cognitive Services.
}