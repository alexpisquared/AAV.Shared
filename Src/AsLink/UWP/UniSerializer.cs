namespace AsLink
{
    interface IFileSerialiser
  {
    void Save<T>(T o, string filename);
    T Load<T>(string filename);
  }
  interface IStringSerializer
  {
    string Save<T>(T data);
    T Load<T>(string json); //catch ... : lets the caller handle the failure: it knows better what to do
  }

//  public static class XmlFileSerializer // ~~: IFileSerialiser
//  {
//    public static void Save<T>(T o, string filename)
//    {
//      try
//      {
//        if (!Directory.Exists(Path.GetDirectoryName(filename)))
//          Directory.CreateDirectory(Path.GetDirectoryName(filename));

//        using (var streamWriter = new StreamWriter(filename))
//        {
//          new XmlSerializer(o.GetType()).Serialize(streamWriter, o);
//          streamWriter.Close();
//        }
//      }
//      catch (Exception ex) { ex.Log(); throw; }
//    }
//    public static T Load<T>(string filename)
//    {
//      try
//      {
//        if (File.Exists(filename))
//          using (var streamReader = new StreamReader(filename))
//          {
//            var o = (T)(new XmlSerializer(typeof(T)).Deserialize(streamReader));
//            streamReader.Close();
//            return o;
//          }
//      }
//      catch (InvalidOperationException ex) { if (ex.HResult != -2146233079) ex.Log(); throw; } // "Root element is missing." ==> create new at the bottom
//      catch (Exception ex) { ex.Log(); throw; }


//      return (T)Activator.CreateInstance(typeof(T));
//    }
//  }
//  public static class JsonFileSerializer
//  {
//    public static void Save<T>(T o, string filename)
//    {
//      try
//      {
//        if (!Directory.Exists(Path.GetDirectoryName(filename)))
//          Directory.CreateDirectory(Path.GetDirectoryName(filename));

//        using (var streamWriter = new StreamWriter(filename))
//        {
//          new DataContractJsonSerializer(typeof(T)).WriteObject(streamWriter.BaseStream, o);
//          streamWriter.Close();
//        }
//      }
//      catch (Exception ex) { ex.Log(); throw; }
//    }
//    public static T Load<T>(string filename)
//    {
//      try
//      {
//        if (File.Exists(filename))
//          using (var streamReader = new StreamReader(filename))
//          {
//            var o = (T)(new DataContractJsonSerializer(typeof(T)).ReadObject(streamReader.BaseStream));
//            streamReader.Close();
//            return o;
//          }
//      }
//      catch (InvalidOperationException ex) { if (ex.HResult != -2146233079) ex.Log(); throw; } // "Root element is missing." ==> create new at the bottom
//      catch (Exception ex) { ex.Log(); throw; }


//      return (T)Activator.CreateInstance(typeof(T));
//    }
//  }
//  public static class JsonIsoFileSerializer
//  {
//    static readonly ApplicationDataContainer _defaultToLocal = ApplicationData.Current.LocalSettings;

//    public static void Save<T>(T o, string filenameONLY = null, ApplicationDataContainer adc = null)
//    {
//      if (adc == null) adc = _defaultToLocal;

//      try
//      {
//        var isoStore = IsolatedStorageFile.GetStore(adc, null, null);

//        using (var isoStream = new IsolatedStorageFileStream(IsoHelper.GetSetFilename<T>(filenameONLY, "json"), FileMode.Create, isoStore))
//        {
//          using (var streamWriter = new StreamWriter(isoStream))
//          {
//            new DataContractJsonSerializer(typeof(T)).WriteObject(streamWriter.BaseStream, o); // new XmlSerializer(o.GetType()).Serialize(streamWriter, o);
//            streamWriter.Close();
//          }
//        }
//      }
//      catch (Exception ex) { ex.Log(); throw; }
//    }
//    public static object Load<T>(string filenameONLY = null, ApplicationDataContainer adc = null)
//    {
//      if (adc == null) adc = _defaultToLocal;

//      try
//      {
//        var isoStore = IsolatedStorageFile.GetStore(adc, null, null);

//        if (isoStore.FileExists(IsoHelper.GetSetFilename<T>(filenameONLY, "json")))
//          using (var isoStream = new IsolatedStorageFileStream(IsoHelper.GetSetFilename<T>(filenameONLY, "json"), FileMode.Open, FileAccess.Read, FileShare.Read, isoStore))
//          {
//            using (StreamReader streamReader = new StreamReader(isoStream))
//            {
//              var o = (T)(new DataContractJsonSerializer(typeof(T)).ReadObject(streamReader.BaseStream)); // var o = (T)(new XmlSerializer(typeof(T)).Deserialize(streamReader));
//              streamReader.Close();
//              return o;
//            }
//          }
//      }
//      catch (InvalidOperationException /**/ ex) { if (ex.HResult != -2146233079) { ex.Log(); throw; } }  // "Root element is missing." ==> create new at the bottom
//      catch (SerializationException    /**/ ex) { if (ex.HResult != -2146233076) { ex.Log(); throw; } }  // "There was an error deserializing the object of type AAV.SS.Logic.AppSettings. End element 'LastSave' from namespace '' expected. Found element 'DateTime' from namespace ''."	string
//      catch (Exception                 /**/ ex) { ex.Log(); throw; }

//      return Activator.CreateInstance(typeof(T));
//    }
//  }
//  public static class XmlIsoFileSerializer
//  {
//    static readonly ApplicationDataContainer _defaultToLocal = ApplicationData.Current.LocalSettings;

//    public static void Save<T>(T o, string filenameONLY = null, ApplicationDataContainer adc = null)
//    {
//      if (adc == null) adc = _defaultToLocal;

//      try
//      {
//        var isoStore = IsolatedStorageFile.GetStore(adc, null, null);

//        using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(IsoHelper.GetSetFilename<T>(filenameONLY, "xml"), FileMode.Create, isoStore))
//        {
//          IsoHelper.DevDbgLookup(isoStore, isoStream);

//          using (StreamWriter streamWriter = new StreamWriter(isoStream))
//          {
//            new XmlSerializer(o.GetType()).Serialize(streamWriter, o);
//            streamWriter.Close();
//          }
//        }
//      }
//      catch (Exception ex) { ex.Log(); throw; }
//    }
//    public static object Load<T>(string filenameONLY = null, ApplicationDataContainer adc = null)
//    {
//      if (adc == null) adc = _defaultToLocal;

//      try
//      {
//        var isoStore = IsolatedStorageFile.GetStore(adc, null, null);

//        if (isoStore.FileExists(IsoHelper.GetSetFilename<T>(filenameONLY, "xml")))
//          using (var isoStream = new IsolatedStorageFileStream(IsoHelper.GetSetFilename<T>(filenameONLY, "xml"), FileMode.Open, FileAccess.Read, FileShare.Read, isoStore))
//          {
//            IsoHelper.DevDbgLookup(isoStore, isoStream);

//            using (StreamReader streamReader = new StreamReader(isoStream))
//            {
//              var o = (T)(new XmlSerializer(typeof(T)).Deserialize(streamReader));
//              streamReader.Close();
//              return o;
//            }
//          }
//      }
//      catch (InvalidOperationException ex) { if (ex.HResult != -2146233079) ex.Log(); throw; } // "Root element is missing." ==> create new at the bottom
//      catch (Exception ex) { ex.Log(); throw; }

//      return Activator.CreateInstance(typeof(T));
//    }
//  }

//  public static class JsonStringSerializer
//  {
//    public static string Save<T>(T o)
//    {
//      var serializer = new DataContractJsonSerializer(typeof(T));
//      using (var ms = new MemoryStream())
//      {
//        serializer.WriteObject(ms, o);
//        var jsonArray = ms.ToArray();
//        return Encoding.UTF8.GetString(jsonArray, 0, jsonArray.Length);
//      }
//    }
//    public static T Load<T>(string str) //catch ... : lets the caller handle the failure: it knows better what to do
//    {
//      if (string.IsNullOrEmpty(str)) return (T)Activator.CreateInstance(typeof(T));

//      using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(str))) { return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms); } // if ....: return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(str)));
//    }
//  }

//  public static class XmlStringSerializer
//  {
//    public static string Save<T>(T o)
//    {
//      var sb = new StringBuilder();
//      using (var sw = new StringWriter(sb))
//      {
//        new XmlSerializer(typeof(T)).Serialize(sw, o);
//        sw.Close();
//      }
//      return sb.ToString();
//    }

//    public static T Load<T>(string str) { return (T)new XmlSerializer(typeof(T)).Deserialize(new StringReader(str)); }
//  }

//  public static class IsoHelper
//  {
//    public static string GetSetFilename<T>(string filenameONLY, string ext)
//    {
//      var s = string.IsNullOrEmpty(filenameONLY) ? $"{typeof(T).Name}.{ext}" : filenameONLY;
//      Trace.WriteLine($":iso:>>>\t\r\n{GetIsoFolder()}\t\r\n{s}");
//      return s;
//    }
//    [Obsolete("ANy useful?", false)]
//    public static IsolatedStorageFile GetIsolatedStorageFile()
//    {
//      var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
//      return isoStore;

//#if DEBUG
//      return IsolatedStorageFile.GetUserStoreForAssembly();       //todo: GetUserStoreForApplication does not work
//#else
//      if (AppDomain.CurrentDomain.ActivationContext == null)
//        return IsolatedStorageFile.GetMachineStoreForDomain();    // C:\ProgramData\IsolatedStorage\...			http://stackoverflow.com/questions/72626/could-not-find-file-when-using-isolated-storage				
//      //return IsolatedStorageFile.GetMachineStoreForAssembly(); 			
//      else
//        return IsolatedStorageFile.GetUserStoreForApplication();	// C:\Users\Alex\AppData\Local\Apps\...		http://stackoverflow.com/questions/202013/clickonce-and-isolatedstorage/227218#227218
//#endif
//    }
//    public static void DevDbgLookup(IsolatedStorageFile isoStoreF, IsolatedStorageFileStream isoStream)
//    {
//#if DEBUG
//      Trace.WriteLine(isoStoreF.GetType().GetField("m_RootDir", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(isoStoreF), "::>>iso.m_rootDir "); //instead of creating a temp file and get the location you can get the path from the store directly: 
//      Trace.WriteLine(isoStream.GetType().GetField("m_FullPath", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(isoStream), "::>>iso.m_fullPath"); //Retrieve the actual path of the file using reflection.
//#endif
//    }
//    public static void ListIsoFolders()
//    {
//      try
//      {
//#if DEBUG
//        Trace.WriteLine(AppDomain.CurrentDomain.ActivationContext == null, "ActivationContext==null");

//        ////Unable to determine application identity of the caller. try { twl(IsolatedStorageFile.GetMachineStoreForApplication   /**/ (), "\r\n"); } catch (Exception ex) { Trace.WriteLine(ex.Message); }
//        //try { twl(IsolatedStorageFile.GetMachineStoreForAssembly      /**/ (), "\r\n"); } catch (Exception ex) { Trace.WriteLine(ex.Message); }
//        //try { twl(IsolatedStorageFile.GetMachineStoreForDomain        /**/ (), "\r\n"); } catch (Exception ex) { Trace.WriteLine(ex.Message); }
//        ////Unable to determine application identity of the caller. try { twl(IsolatedStorageFile.GetUserStoreForApplication      /**/ (), "\r\n"); } catch (Exception ex) { Trace.WriteLine(ex.Message); }
//        //try { twl(IsolatedStorageFile.GetUserStoreForAssembly         /**/ (), "\r\n"); } catch (Exception ex) { Trace.WriteLine(ex.Message); }
//        //try { twl(IsolatedStorageFile.GetUserStoreForDomain           /**/ (), "\r\n"); } catch (Exception ex) { Trace.WriteLine(ex.Message); }
//        ////The Site scope is currently not supported.: try { twl(IsolatedStorageFile.GetUserStoreForSite             /**/ (), "\r\n"); } catch (Exception ex) { Trace.WriteLine(ex.Message); }

//        //for (int i = 1; i <= 32; i *= 2)          for (int j = 1; j <= 32; j *= 2)              try { twl(IsolatedStorageFile.GetStore((IsolatedStorageScope)(i | j ), null, null)); Trace.Write($" \t {i,2}:{j,2} \r\n"); } catch { }// (Exception ex) { Trace.WriteLine(ex.Message); }        Trace.WriteLine("");        foreach (var s in _l.OrderBy(r => r)) Trace.WriteLine(s);        Trace.WriteLine("");

//        for (int i = 1; i <= 32; i *= 2) for (int j = 1; j <= 32; j *= 2) for (int k = 1; k <= 32; k *= 2) try { twl(IsolatedStorageFile.GetStore((IsolatedStorageScope)(i | j | k), null, null)); Trace.Write($" \t {i,2}:{j,2}:{k,2} \r\n"); } catch { }// (Exception ex) { Trace.WriteLine(ex.Message); }
//        Trace.WriteLine(""); foreach (var s in _l.OrderBy(r => r)) Trace.WriteLine(s); Trace.WriteLine("");


//        try { twl(IsolatedStorageFile.GetStore((IsolatedStorageScope)(0 | 1 | 04), null, null));  /**/ Trace.WriteLine(@"     C:\Users\apigida\AppData\ Local \IsolatedStorage\...\AssemFiles\   "); } catch { }
//        try { twl(IsolatedStorageFile.GetStore((IsolatedStorageScope)(1 | 2 | 04), null, null));  /**/ Trace.WriteLine(@"     C:\Users\apigida\AppData\ Local \IsolatedStorage\...\Files\        "); } catch { }
//        try { twl(IsolatedStorageFile.GetStore((IsolatedStorageScope)(1 | 4 | 08), null, null));  /**/ Trace.WriteLine(@"     C:\Users\apigida\AppData\Roaming\IsolatedStorage\...\AssemFiles\   "); } catch { }
//        try { twl(IsolatedStorageFile.GetStore((IsolatedStorageScope)(2 | 4 | 16), null, null));  /**/ Trace.WriteLine(@"     C:\ProgramData\IsolatedStorage\...\Files\                          "); } catch { }
//        try { twl(IsolatedStorageFile.GetStore((IsolatedStorageScope)(0 | 4 | 16), null, null));  /**/ Trace.WriteLine(@"     C:\ProgramData\IsolatedStorage\...\AssemFiles\                     "); } catch { }

//#endif
//      }
//      catch (Exception ex) { ex.Log(); }
//    }
//    public static string GetIsoFolder()
//    {
//      try
//      {
//        var isf = IsolatedStorageFile.GetStore(IsoConst.PdFls, null, null);
//        return isf.GetType().GetField("m_RootDir", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(isf).ToString(); //instead of creating a temp file and get the location you can get the path from the store directly: 
//      }
//      catch (Exception ex) { ex.Log(); }

//      return null;
//    }

//    [Obsolete(@"Use 'C:\c\AsLink\UniSerializer.cs' instead!!!")]
//    public static void SaveIsoFile(string filename, string json)
//    {
//      try
//      {
//        using (var strm = new IsolatedStorageFileStream(filename, FileMode.Create, IsoHelper.GetIsolatedStorageFile()))
//        {
//          using (var streamWriter = new StreamWriter(strm))
//          {
//            streamWriter.Write(json);
//            streamWriter.Close();
//          }
//        }
//      }
//      catch (Exception ex) { ex.Log(); }
//    }

//    [Obsolete(@"Use 'C:\c\AsLink\UniSerializer.cs' instead!!!")]
//    public static string ReadIsoFile(string filename)
//    {
//      try
//      {
//        var isf = IsoHelper.GetIsolatedStorageFile(); //.Trace.WriteLine("ISO:/> "+isf.GetType().GetField("m_RootDir", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(isf)); 

//        if (isf.GetFileNames(filename).Length <= 0) return null;

//        using (var stream = new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, isf))
//        {
//          if (stream.Length <= 0) return null;

//          //.Trace.WriteLine("ISO:/> "+stream.GetType().GetField("m_FullPath", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(stream).ToString()); //Retrieve the actual path of the file using reflection.

//          using (StreamReader streamReader = new StreamReader(stream))
//          {
//            var rv = streamReader.ReadToEnd();
//            streamReader.Close();
//            return rv;
//          }
//        }
//      }
//      catch (Exception ex) { ex.Log(); }

//      return null;
//    }


//    static List<string> _l = new List<string>();
//    static void twl(IsolatedStorageFile f, string crlf = null)
//    {
//      var s = f.GetType().GetField("m_RootDir", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(f).ToString();
//      Trace.Write($":isf:> {s,-155}{ crlf}");
//      if (_l.Any(r => r.Equals(s, StringComparison.OrdinalIgnoreCase))) return;
//      _l.Add(s);
//    }
//  }
}
