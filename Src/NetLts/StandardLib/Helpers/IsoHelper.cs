﻿using System.IO.IsolatedStorage;

namespace StandardLib.Helpers;

public static class IsoHelper
{
  [Obsolete("AAV-> ..instead use:\r\n System.IO.IsolatedStorage.IsolatedStorageFile.GetStore(System.IO.IsolatedStorage.IsolatedStorageScope.User | System.IO.IsolatedStorage.IsolatedStorageScope.Assembly, null, null)", true)] public static IsolatedStorageFile GetIsolatedStorageFile() => IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);// #if TEMPO//      return IsolatedStorageFile.GetUserStoreForAssembly();       //todo: GetUserStoreForApplication does not work// #else//      if (AppDomain.CurrentDomain.ActivationContext == null)//        return IsolatedStorageFile.GetMachineStoreForDomain();    // C:\ProgramData\IsolatedStorage\...			http://stackoverflow.com/questions/72626/could-not-find-file-when-using-isolated-storage				//      //return IsolatedStorageFile.GetMachineStoreForAssembly(); 			//      else//        return IsolatedStorageFile.GetUserStoreForApplication();	// C:\Users\Alex\AppData\Local\Apps\...		http://stackoverflow.com/questions/202013/clickonce-and-isolatedstorage/227218#227218// #endif

  [Obsolete("""Use '\c\AsLink\UniSerializer.cs' instead!!!""")]
  public static void SaveIsoFile(string filename, string json)
  {
    try
    {
      using IsolatedStorageFileStream? strm = new(filename, FileMode.Create, GetIsolatedStorageFile());
      using StreamWriter? streamWriter = new(strm);
      streamWriter.Write(json);
      streamWriter.Close();
    }
    catch (Exception ex) { _ = ex.Log(); throw; }
  }

  [Obsolete("""Use '\c\AsLink\UniSerializer.cs' instead!!!""")]
  public static string ReadIsoFile(string filename)
  {
    try
    {
      var isf = IsoHelper.GetIsolatedStorageFile();

      if (isf.GetFileNames(filename).Length <= 0) return "";

      using IsolatedStorageFileStream? stream = new(filename, FileMode.OpenOrCreate, isf);
      if (stream.Length <= 0) return "";

      //WriteLine("ISO:/> " + stream.GetType().GetField(_FullPath, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(stream).ToString()); //Retrieve the actual path of the file using reflection.

      using //(var streamReader = XmlReader.Create(stream))//ing 
StreamReader? streamReader = new(stream);
      var rv = streamReader.ReadToEnd();
      streamReader.Close();
      return rv;
    }
    catch (Exception ex) { _ = ex.Log(); throw; }
  }

#if ExploreIso
  const string _RootDir = "m_RootDir", _FullPath = "m_FullPath";
  public static void DevDbgLookup(IsolatedStorageFile isoStoreF, IsolatedStorageFileStream isoStream)
  {
    WriteLine(isoStoreF.GetType().GetField(_RootDir, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(isoStoreF), ":>iso.m_rootDir  "); //instead of creating a temp file and get the location you can get the path from the store directly: 
    WriteLine(isoStream.GetType().GetField(_FullPath, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(isoStream), ":>iso.m_fullPath "); //Retrieve the actual path of the file using reflection.
  }
  static void ListIsoFolders()
  {
    try
    {
      //..WriteLine(AppDomain.CurrentDomain.ActivationContext == null, "ActivationContext==null");

      ////Unable to determine application identity of the caller. try { twl(IsolatedStorageFile.GetMachineStoreForApplication   /**/ (), "\r\n"); } catch (Exception ex) { WriteLine(ex.InnerMessages()); }
      //try { twl(IsolatedStorageFile.GetMachineStoreForAssembly      /**/ (), "\r\n"); } catch (Exception ex) { WriteLine(ex.InnerMessages()); }
      //try { twl(IsolatedStorageFile.GetMachineStoreForDomain        /**/ (), "\r\n"); } catch (Exception ex) { WriteLine(ex.InnerMessages()); }
      ////Unable to determine application identity of the caller. try { twl(IsolatedStorageFile.GetUserStoreForApplication      /**/ (), "\r\n"); } catch (Exception ex) { WriteLine(ex.InnerMessages()); }
      //try { twl(IsolatedStorageFile.GetUserStoreForAssembly         /**/ (), "\r\n"); } catch (Exception ex) { WriteLine(ex.InnerMessages()); }
      //try { twl(IsolatedStorageFile.GetUserStoreForDomain           /**/ (), "\r\n"); } catch (Exception ex) { WriteLine(ex.InnerMessages()); }
      ////The Site scope is currently not supported.: try { twl(IsolatedStorageFile.GetUserStoreForSite             /**/ (), "\r\n"); } catch (Exception ex) { WriteLine(ex.InnerMessages()); }

      //for (int i = 1; i <= 32; i *= 2)          for (int j = 1; j <= 32; j *= 2)              try { twl(IsolatedStorageFile.GetStore((IsolatedStorageScope)(i | j ), null, null)); Write($"TrcW:>  \t {i,2}:{j,2} \r\n"); } catch { }// (Exception ex) { WriteLine(ex.InnerMessages()); }        WriteLine("");        foreach (var s in _l.OrderBy(r => r)) WriteLine(s);        WriteLine("");

      for (var i = 1; i <= 32; i *= 2) for (var j = 1; j <= 32; j *= 2) for (var k = 1; k <= 32; k *= 2) try { twl(IsolatedStorageFile.GetStore((IsolatedStorageScope)(i | j | k), null, null)); Write($"TrcW:>  \t {i,2}:{j,2}:{k,2} \r\n"); } catch { }// (Exception ex) { WriteLine(ex.InnerMessages()); }
      WriteLine(""); foreach (var s in _l.OrderBy(r => r)) WriteLine(s); WriteLine("");


      try { twl(IsolatedStorageFile.GetStore((IsolatedStorageScope)(0 | 1 | 04), null, null));  /**/ WriteLine(@"     C:\Users\apigida\AppData\ Local \IsolatedStorage\...\AssemFiles\   "); } catch { }
      try { twl(IsolatedStorageFile.GetStore((IsolatedStorageScope)(1 | 2 | 04), null, null));  /**/ WriteLine(@"     C:\Users\apigida\AppData\ Local \IsolatedStorage\...\Files\        "); } catch { }
      try { twl(IsolatedStorageFile.GetStore((IsolatedStorageScope)(1 | 4 | 08), null, null));  /**/ WriteLine(@"     C:\Users\apigida\AppData\Roaming\IsolatedStorage\...\AssemFiles\   "); } catch { }
      try { twl(IsolatedStorageFile.GetStore((IsolatedStorageScope)(2 | 4 | 16), null, null));  /**/ WriteLine(@"     C:\ProgramData\IsolatedStorage\...\Files\                          "); } catch { }
      try { twl(IsolatedStorageFile.GetStore((IsolatedStorageScope)(0 | 4 | 16), null, null));  /**/ WriteLine(@"     C:\ProgramData\IsolatedStorage\...\AssemFiles\                     "); } catch { }

    }
    catch (Exception ex) { ex.Log(); throw; }
  }
  public static string GetIsoFolder() //todo: no go for .net core 3: all are ""!
  {
    //#if NotOnDotNetStandard // works only on .NET Framework 4.8, NOT on Core.
    try { var isf = IsolatedStorageFile.GetStore(IsoConst.PdFls, null, null); /**/ logFolder(isf); } catch (Exception ex) { WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] Nogo for:  GetStore(IsoConst.PdFls, null, null);  <= {ex.InnerMessages()}"); }
    try { var isf = IsolatedStorageFile.GetMachineStoreForApplication();      /**/ logFolder(isf); } catch (Exception ex) { WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] Nogo for:  GetMachineStoreForApplication();       <= {ex.InnerMessages()}"); }
    try { var isf = IsolatedStorageFile.GetUserStoreForApplication();         /**/ logFolder(isf); } catch (Exception ex) { WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] Nogo for:  GetUserStoreForApplication();          <= {ex.InnerMessages()}"); }
    try { var isf = IsolatedStorageFile.GetMachineStoreForAssembly();         /**/ logFolder(isf); } catch (Exception ex) { WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] Nogo for:  GetMachineStoreForAssembly();          <= {ex.InnerMessages()}"); }
    try { var isf = IsolatedStorageFile.GetMachineStoreForDomain();           /**/ logFolder(isf); } catch (Exception ex) { WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] Nogo for:  GetMachineStoreForDomain();            <= {ex.InnerMessages()}"); }
    try { var isf = IsolatedStorageFile.GetUserStoreForAssembly();            /**/ logFolder(isf); } catch (Exception ex) { WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] Nogo for:  GetUserStoreForAssembly();             <= {ex.InnerMessages()}"); }
    try { var isf = IsolatedStorageFile.GetUserStoreForDomain();              /**/ logFolder(isf); } catch (Exception ex) { WriteLine($"[{DateTime.Now:HH:mm:ss} Trc] Nogo for:  GetUserStoreForDomain();               <= {ex.InnerMessages()}"); }
    //#endif

    try
    {
      var isf = IsolatedStorageFile.GetMachineStoreForAssembly();

      return isf.GetType().GetField(_RootDir, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(isf)?.ToString() ?? @"C:\temp\"; //todo: smarter fallback needed.
    }
    catch (Exception ex) { ex.Log(); throw; }
  }
  static void logFolder(IsolatedStorageFile dir) => WriteLine($"[{DateTime.Now:HH:mm:ss} Trc]  iso dir: '{dir.GetType().GetField(_RootDir, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(dir)}'."); //instead of creating a temp file and get the location you can get the path from the store directly: 

  static readonly System.Collections.Generic.List<string> _l = new System.Collections.Generic.List<string>();
  static void twl(IsolatedStorageFile f, string crlf = "")
  {
    var s = f.GetType().GetField(_RootDir, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(f).ToString();

    Write($"TrcW:> :isf:> {s,-155}{ crlf}");

    if (_l.Any(r => r.Equals(s/*, StringComparison.OrdinalIgnoreCase*/))) return;

    _l.Add(s);
  }
#endif
}
