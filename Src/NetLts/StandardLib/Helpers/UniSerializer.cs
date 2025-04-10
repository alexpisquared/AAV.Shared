﻿namespace StandardLib.Helpers;
public static class JsonFileSerializer
{
  public static void Save<T>(T obj, string filename, bool saveFormatted = false)
  {
    try
    {
      var dir = Path.GetDirectoryName(filename);
      if (!string.IsNullOrEmpty(dir))
        if (!FSHelper.ExistsOrCreated(dir))
          throw new DirectoryNotFoundException(Path.GetDirectoryName(filename));

      //Note: System.Windows.Forms.WindowPlacement type is not directly serializable by the System.Text.Json.JsonSerializer class: use DataContractJsonSerializer  :(       
      if (saveFormatted)
        File.WriteAllText(filename, JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true }));
      else
      {
        using StreamWriter streamWriter = new(filename);
        new DataContractJsonSerializer(typeof(T)).WriteObject(streamWriter.BaseStream, obj);
        streamWriter.Close();
      }
    }
    catch (Exception ex) { _ = ex.Log(); throw; }
  }
  public static T Load<T>(string filename, bool saveFormatted = false) where T : new()
  {
    try
    {
      if (File.Exists(filename))
      {
        //tmi: WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.f}  Load<{typeof(T).Name}>({filename})");
        if (saveFormatted)
          return JsonSerializer.Deserialize<T>(File.ReadAllText(filename), new JsonSerializerOptions { WriteIndented = true }) ?? throw new ArgumentNullException("@123");
        else
        {
          using var streamReader = new StreamReader(filename);
          return (T)(new DataContractJsonSerializer(typeof(T)).ReadObject(streamReader.BaseStream) ?? throw new ArgumentNullException("@432"));
        }
      }
    }
    catch (Exception ex) { _ = ex.Log(); }

    return (T)(Activator.CreateInstance(typeof(T)) ?? new T());
  }
}
public static class XmlFileSerializer
{
  public static void Save<T>(T obj, string filename)
  {
    try
    {
      if (!FSHelper.ExistsOrCreated(Path.GetDirectoryName(filename) ?? throw new ArgumentNullException("▄▀")))
        throw new DirectoryNotFoundException(Path.GetDirectoryName(filename));

      using StreamWriter streamWriter = new(filename);
      new XmlSerializer(typeof(T)).Serialize(streamWriter, obj);
    }
    catch (Exception ex) { _ = ex.Log(); throw; }
  }
  public static T? Load<T>(string filename) where T : new()
  {
    try
    {
      if (File.Exists(filename))
      {
        using var streamReader = /*new StreamReader*/XmlReader.Create(filename);
        return (T?)new XmlSerializer(typeof(T))?.Deserialize(streamReader);
      }
    }
    catch (InvalidOperationException ex) { if (ex.HResult != -2146233079) _ = ex.Log(); throw; } // "Root element is missing." ==> create new at the bottom
    catch (Exception ex) { _ = ex.Log(); throw; }

    return (T)(Activator.CreateInstance(typeof(T)) ?? new T());
  }
}

public static class JsonStringSerializer
{
  public static string Save<T>(T o)
  {
    DataContractJsonSerializer? serializer = new(typeof(T));
    using MemoryStream? ms = new();
    serializer.WriteObject(ms, o);
    var jsonArray = ms.ToArray();
    return Encoding.UTF8.GetString(jsonArray, 0, jsonArray.Length);
  }
  public static T? Load<T>(string str) where T : new() //catch ... : lets the caller handle the failure: it knows better what to do
  {
    if (string.IsNullOrEmpty(str)) return (T)(Activator.CreateInstance(typeof(T)) ?? new T());

    using MemoryStream? ms = new(Encoding.UTF8.GetBytes(str));
    return (T?)new DataContractJsonSerializer(typeof(T))?.ReadObject(ms);      // if ....: return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(str)));
  }
}
public static class XmlStringSerializer
{
  public static string Save<T>(T o)
  {
    StringBuilder? sb = new();
    using (StringWriter? sw = new(sb))
    {
      new XmlSerializer(typeof(T)).Serialize(sw, o);
      sw.Close();
    }

    return sb.ToString();
  }
  public static T? Load<T>(string str) => (T?)new XmlSerializer(typeof(T)).Deserialize(new StringReader(str));
}

public static class JsonIsoFileSerializer
{
  public static void Save<T>(T o, string? filenameONLY = null, IsolatedStorageScope iss = IsoConst.PdFls)
  {
    try
    {
      var isoStore = IsolatedStorageFile.GetStore(iss, null, null);

      using IsolatedStorageFileStream isoStream = new(IsoConst.GetSetFilename<T>(filenameONLY ?? typeof(T).Name, "json"), FileMode.Create, isoStore);
      using StreamWriter streamWriter = new(isoStream);
      new DataContractJsonSerializer(typeof(T)).WriteObject(streamWriter.BaseStream, o); // new XmlSerializer(obj.GetType()).Serialize(streamWriter, obj);
      streamWriter.Close();
    }
    catch (Exception ex) { _ = ex.Log(); throw; }
  }
  public static T? Load<T>(string? filenameONLY = null, IsolatedStorageScope iss = IsoConst.PdFls) where T : new()
  {
    try
    {
      var isoStore = IsolatedStorageFile.GetStore(iss, null, null);

      if (isoStore.FileExists(IsoConst.GetSetFilename<T>(filenameONLY ?? typeof(T).Name, "json")))
      {
        using IsolatedStorageFileStream? isoStream = new(IsoConst.GetSetFilename<T>(filenameONLY ?? typeof(T).Name, "json"), FileMode.Open, FileAccess.Read, FileShare.Read, isoStore);
        using StreamReader streamReader = new(isoStream);
        var obj = (T?)new DataContractJsonSerializer(typeof(T)).ReadObject(streamReader.BaseStream); // var obj = (T)(new XmlSerializer(typeof(T)).Deserialize(streamReader));
        streamReader.Close();
        return obj;
      }
    }
    catch (InvalidOperationException /**/ ex) { if (ex.HResult != -2146233079) { _ = ex.Log(); throw; } }  // "Root element is missing." ==> create new at the bottom
    catch (SerializationException    /**/ ex) { if (ex.HResult != -2146233076) { _ = ex.Log(); throw; } }  // "There was an error deserializing the object of type AlexPi.Scr.Logic.AppSettings. End element 'LastSave' from namespace '' expected. Found element 'DateTimeOffset' from namespace ''."	string
    catch (Exception                 /**/ ex) { _ = ex.Log(typeof(T).Name); throw; }

    return (T)(Activator.CreateInstance(typeof(T)) ?? new T());
  }
}
public static class XmlIsoFileSerializer
{
  public static void Save<T>(T o, string filenameONLY = "", IsolatedStorageScope iss = IsoConst.f33)
  {
    try
    {
      var isoStore = IsolatedStorageFile.GetStore(iss, null, null);

      using IsolatedStorageFileStream? isoStream = new(IsoConst.GetSetFilename<T>(filenameONLY, "xml"), FileMode.Create, isoStore);
      //IsoHelper.DevDbgLookup(isoStore, isoStream);

      using StreamWriter? streamWriter = new(isoStream);
      new XmlSerializer(typeof(T))?.Serialize(streamWriter, o);
    }
    catch (Exception ex) { _ = ex.Log(); throw; }
  }
  public static T Load<T>(string filenameONLY = "", IsolatedStorageScope iss = IsoConst.f33) where T : new()
  {
    try
    {
      var isoStore = IsolatedStorageFile.GetStore(iss, null, null);

      if (isoStore.FileExists(IsoConst.GetSetFilename<T>(filenameONLY, "xml")))
      {
        using IsolatedStorageFileStream? stream = new(IsoConst.GetSetFilename<T>(filenameONLY, "xml"), FileMode.Open, FileAccess.Read, FileShare.Read, isoStore);
        using var streamReader = XmlReader.Create(stream);//ing (var streamReader = new StreamReader(stream))
        var o = (T?)new XmlSerializer(typeof(T)).Deserialize(streamReader);
        streamReader.Close();
        return o ?? (T)(Activator.CreateInstance(typeof(T)) ?? new T());
      }
    }
    catch (InvalidOperationException ex) { if (ex.HResult != -2146233079) _ = ex.Log(); throw; } // "Root element is missing." ==> create new at the bottom
    catch (Exception ex) { _ = ex.Log(); throw; }

    return (T)(Activator.CreateInstance(typeof(T)) ?? new T());
  }
}

public static class IsoConst
{
  public const IsolatedStorageScope
  //
  //PdAsm = (IsolatedStorageScope)(0 | 4 | 16), // C:\ProgramData                  \IsolatedStorage\...\AssemFiles\   //nogo: Requires Admin permissions !!! Jul 2021
    f33 = IsolatedStorageScope.User | IsolatedStorageScope.Application, // 1 | 32
    ULocA = (IsolatedStorageScope)(0 | 1 | 04), // C:\Users\[user]\AppData\ Local  \IsolatedStorage\...\AssemFiles\   //
    URoaA = (IsolatedStorageScope)(1 | 4 | 08), // C:\Users\[user]\AppData\Roaming \IsolatedStorage\...\AssemFiles\   //
    ULocF = (IsolatedStorageScope)(1 | 2 | 04), // C:\Users\[user]\AppData\ Local  \IsolatedStorage\...\Files\        //still changing every MSIX build
    PdFls = (IsolatedStorageScope)(2 | 4 | 16); // C:\ProgramData                  \IsolatedStorage\...\Files\        //still changing every MSIX build

  public static string GetSetFilename<T>(string filenameONLY, string ext) => $"{(string.IsNullOrEmpty(filenameONLY) ? typeof(T).Name : filenameONLY)}.{ext}";
}
