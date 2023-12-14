
namespace AmbienceLib;

public interface ISpeechSynth
{
  void Dispose();
  Task SpeakAsync(string msg, double speakingRate = 1, double volumePercent = 100, string voice = "", string style = "friendly", string role = "YoungAdultFemale");
  void SpeakAsyncCancelAll();
  void SpeakFAF(string msg, double speakingRate = 1, double volumePercent = 100, string voice = "", string style = "friendly");
}