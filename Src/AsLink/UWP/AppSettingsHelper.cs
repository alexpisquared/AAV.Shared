using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Storage;
namespace AsLink
{
  public static class AppSettingsHelper // from uws (Unversal Win Samples; org name ApplicationSettingsHelper)
  {
    static readonly ApplicationDataContainer _defaultToLocal = ApplicationData.Current.LocalSettings;
    static readonly StorageFolder _defaultToLocalF = ApplicationData.Current.LocalFolder;

    public static object ReadVal(string key, ApplicationDataContainer adc = null)
    {
      if (adc == null) adc = _defaultToLocal;

      if (adc.Values.TryGetValue(key, out object value))
        return value;
      else
      {
        Debug.WriteLine($"Settings value for key '{key}' NOT found!!!");
        return null;
      }
    }
    public static void SaveVal(string key, object value, ApplicationDataContainer adc = null)
    {
      if (adc == null)
        adc = _defaultToLocal;

      //654: Debug.WriteLine($"Storing  settings  key '{key}' : '{value}'");

      try
      {
        if (!adc.Values.ContainsKey(key))
          adc.Values.Add(key, value);
        else
          adc.Values[key] = value;
      }
      catch (COMException ex2) { Debug.WriteLine($"$#~>{ex2.Message}"); if (Debugger.IsAttached) Debugger.Break(); else throw; }
      catch (Exception ex2) { Debug.WriteLine($"$#~>{ex2.Message}"); if (Debugger.IsAttached) Debugger.Break(); else throw; }
    }
    public static void RemoVal(string key, ApplicationDataContainer adc = null)
    {
      if (adc == null)
        adc = _defaultToLocal;

      if (adc.Values.ContainsKey(key))
        adc.Values.Remove(key);
      else
        Debug.WriteLine($"Settings value for key '{key}' NOT found!!!");
    }

    public static async Task<string> ReadStr(string file, StorageFolder sfo = null)
    {
      if (sfo == null) sfo = _defaultToLocalF;

      try
      {
        StorageFile sf = await sfo.GetFileAsync($"{file}.json");
        string value = await FileIO.ReadTextAsync(sf);
        return value;
      }
      catch (Exception ex2) { Debug.WriteLine($"$#~>{ex2.Message}"); if (Debugger.IsAttached) Debugger.Break(); }
      return null;
    }
    public static async void SaveStr(string file, string val, StorageFolder sfo = null)
    {
      if (sfo == null) sfo = _defaultToLocalF;

      //654: Debug.WriteLine($"Storing  settings  key '{key}' : '{value}'");

      try
      {
        StorageFile sf = await sfo.CreateFileAsync($"{file}.json", CreationCollisionOption.ReplaceExisting); // Create file; replace if exists.

        await FileIO.WriteTextAsync(sf, val);
      }
      catch (FileLoadException ex2) { Debug.WriteLine($"$#~>{ex2.Message}"); if (Debugger.IsAttached) Debugger.Break(); }
      catch (COMException ex2) { Debug.WriteLine($"$#~>{ex2.Message}"); if (Debugger.IsAttached) Debugger.Break(); else throw; }
      catch (Exception ex2) { Debug.WriteLine($"$#~>{ex2.Message}"); if (Debugger.IsAttached) Debugger.Break(); }
    }
    public static void RemoStr(string file, StorageFolder sfo = null)
    {
      if (sfo == null)
        sfo = _defaultToLocalF;

      Debug.WriteLine($"//todo: Deletion of storage {file}.json is TBI still.");
    }
  }
}

