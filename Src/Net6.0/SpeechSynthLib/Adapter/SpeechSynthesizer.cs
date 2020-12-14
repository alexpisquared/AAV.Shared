using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace SpeechSynthLib.Adapter
{
  public class SpeechSynthesizer // Adapter to the old .Net 4.8 one
  {
    SpeechSynth _synth = new SpeechSynth();

    public int Rate { get; set; }
    public int Volume { get; set; }

    public void SpeakAsyncCancelAll() => _synth = new SpeechSynth(); //todo: does it work or ...?
    public async Task Speak(string msg) => await _synth.SpeakAsync(msg); //nogo: .Wait();
    public void SpeakFaF(string msg) => Task.Run(async () => await _synth.SpeakAsync(msg));
    public void SelectVoiceByHints(object gender) => Trace.WriteLine($" throw new NotImplementedException(); {gender}");
    public void SelectVoiceByHints(VoiceGender gender, VoiceAge adult, int va, CultureInfo ci) => Trace.WriteLine($" throw new NotImplementedException(); {gender}");
    public void SelectVoice(string name) => Trace.WriteLine($" throw new NotImplementedException(); {name}");
    public IEnumerable<InstalledVoice> GetInstalledVoices() => null;
    public object GetCurrentlySpokenPrompt() => null;
  }

  //
  // Summary:
  //     Contains information about a speech synthesis voice installed in Windows.
  [DebuggerDisplay("{VoiceInfo.Name} [{Enabled ? \"Enabled\" : \"Disabled\"}]")]
  public class InstalledVoice
  {
    //
    // Summary:
    //     Gets information about a voice, such as culture, name, gender, and age.
    //
    // Returns:
    //     The information about an installed voice.
    public VoiceInfo VoiceInfo { get; }
    //
    // Summary:
    //     Gets or sets whether a voice can be used to generate speech.
    //
    // Returns:
    //     Returns a bool that represents the enabled state of the voice.
    public bool Enabled { get; set; }

    //
    // Summary:
    //     Determines if a given object is an instance of System.Speech.Synthesis.InstalledVoice
    //     and equal to the current instance of System.Speech.Synthesis.InstalledVoice.
    //
    // Parameters:
    //   obj:
    //     An object that can be cast to an instance of System.Speech.Synthesis.InstalledVoice.
    //
    // Returns:
    //     Returns true if the current instance of System.Speech.Synthesis.InstalledVoice
    //     and that obtained from the obj argument are equal, otherwise returns false.
    public override bool Equals(object obj) => true;
    //
    // Summary:
    //     Provides a hash code for an InstalledVoice object.
    //
    // Returns:
    //     A hash code for the current System.Speech.Synthesis.InstalledVoice object.
    public override int GetHashCode() => 123;
  }
  //
  // Summary:
  //     Represents an installed speech synthesis engine.
  [DebuggerDisplay("{(_name != null ? \"'\" + _name + \"' \" : \"\") +  (_culture != null ? \" '\" + _culture.ToString () + \"' \" : \"\") + (_gender != VoiceGender.NotSet ? \" '\" + _gender.ToString () + \"' \" : \"\") + (_age != VoiceAge.NotSet ? \" '\" + _age.ToString () + \"' \" : \"\") + (_variant > 0 ? \" \" + _variant.ToString () : \"\")}")]
  public class VoiceInfo
  {
    //
    // Summary:
    //     Gets the gender of the voice.
    //
    // Returns:
    //     Returns the gender of the voice.
    public VoiceGender Gender { get; }
    //
    // Summary:
    //     Gets the age of the voice.
    //
    // Returns:
    //     Returns the age of the voice.
    public VoiceAge Age { get; }
    //
    // Summary:
    //     Gets the name of the voice.
    //
    // Returns:
    //     Returns the name of the voice.
    public string Name { get; }
    //
    // Summary:
    //     Gets the culture of the voice.
    //
    // Returns:
    //     Returns a System.Globalization.CultureInfo object that provides information about
    //     a specific culture, such as the names of the culture, the writing system, the
    //     calendar used, and how to format dates and sort strings.
    public CultureInfo Culture { get; }
    //
    // Summary:
    //     Gets the ID of the voice.
    //
    // Returns:
    //     Returns the identifier for the voice.
    public string Id { get; }
    //
    // Summary:
    //     Gets the description of the voice.
    //
    // Returns:
    //     Returns the description of the voice.
    public string Description { get; }
    //
    // Summary:
    //     Gets the collection of audio formats that the voice supports.
    //
    // Returns:
    ////     Returns a collection of the audio formats that the voice supports.
    //[EditorBrowsable(EditorBrowsableState.Advanced)]
    //public ReadOnlyCollection<SpeechAudioFormatInfo> SupportedAudioFormats { get; }
    ////
    //// Summary:
    ////     Gets additional information about the voice.
    ////
    //// Returns:
    ////     Returns a collection of name/value pairs that describe and identify the voice.
    //[EditorBrowsable(EditorBrowsableState.Advanced)]
    //public IDictionary<string, string> AdditionalInfo { get; }

    //
    // Summary:
    //     Compares the fields of the voice with the specified System.Speech.Synthesis.VoiceInfo
    //     object to determine whether they contain the same values.
    //
    // Parameters:
    //   obj:
    //     The specified System.Speech.Synthesis.VoiceInfo object.
    //
    // Returns:
    //     true if the fields of the two System.Speech.Synthesis.VoiceInfo objects are equal;
    //     otherwise, false.
    //public override bool Equals(object obj);
    //
    // Summary:
    //     Provides a hash code for a VoiceInfo object.
    //
    // Returns:
    //     A hash code for the current System.Speech.Synthesis.VoiceInfo object.
    //public override int GetHashCode();
  }


  //
  // Summary:
  //     Defines the values for the age of a synthesized voice.
  public enum VoiceAge
  {
    //
    // Summary:
    //     Indicates that no voice age is specified.
    NotSet = 0,
    //
    // Summary:
    //     Indicates a child voice (age 10).
    Child = 10,
    //
    // Summary:
    //     Indicates a teenage voice (age 15).
    Teen = 15,
    //
    // Summary:
    //     Indicates an adult voice (age 30).
    Adult = 30,
    //
    // Summary:
    //     Indicates a senior voice (age 65).
    Senior = 65
  }
  //
  // Summary:
  //     Defines the values for the gender of a synthesized voice.
  public enum VoiceGender
  {
    //
    // Summary:
    //     Indicates no voice gender specification.
    NotSet = 0,
    //
    // Summary:
    //     Indicates a male voice.
    Male = 1,
    //
    // Summary:
    //     Indicates a female voice.
    Female = 2,
    //
    // Summary:
    //     Indicates a gender-neutral voice.
    Neutral = 3
  }

}
