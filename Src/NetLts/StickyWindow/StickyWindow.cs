using StandardLib.Helpers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;

namespace StickyWindow;
public class StickyWindow : Window
{
  protected readonly DateTimeOffset _mvwStarted = DateTimeOffset.Now;
  protected readonly ILogger _logger;
  const double _defaultZoomV = 1.25;
  const string _defaultTheme = "No Theme";
  const int _swShowNormal = 1, _swShowMinimized = 2, _margin = 0;
  static int _currentTop = 0, _currentLeft = 0;
  string WinCoordinatesFile => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @$"AppSettings\{AppDomain.CurrentDomain.FriendlyName}\{GetType().Name}{(IsDbg ? ".dbg" : "")}.json");
  public StickyWindow() : this(new LoggerFactory().CreateLogger<Window>()) { } // no logging needed; for cases like error popups, etc.
  public StickyWindow(ILogger logger)
  {
    _logger = logger;

    if (Debugger.IsAttached) Topmost = true;

    MouseLeftButtonDown += (s, e) => OnMouseLeftButtonDown_(e);
    MouseWheel += (s, e) => OnMouseWheel_(e);
    KeyUp += (s, e) => OnKeyUp_(e);
  }
  protected bool IgnoreWindowPlacement { get; set; }
  protected bool DragMovable { get; set; } = true;
  public static readonly DependencyProperty ZVProperty = DependencyProperty.Register("ZV", typeof(double), typeof(StickyWindow), new PropertyMetadata(_defaultZoomV)); public double ZV { get => (double)GetValue(ZVProperty); set => SetValue(ZVProperty, value); }
  public static readonly DependencyProperty ThmProperty = DependencyProperty.Register("Thm", typeof(string), typeof(StickyWindow), new PropertyMetadata(_defaultTheme)); public string Thm { get => (string)GetValue(ThmProperty); set => SetValue(ThmProperty, value); }
  public string? KeepOpenReason { get; set; } = null; // """ [KeepOpenReason = "Changes have not been saved."] """;
  protected void ApplyTheme(string themeName, [CallerMemberName] string? cmn = "")
  {
    _logger.Log(LogLevel.Trace, $"► WinBase  shw{(DateTimeOffset.Now - _mvwStarted).TotalSeconds,4:N1}s  {themeName,-26}{cmn} -> {nameof(StickyWindow)}.{nameof(ApplyTheme)}().");

    const string pref = "/WpfUserControlLib;component/ColorScheme/Theme.Color.";

    try
    {
      if (_defaultTheme.Equals(themeName, StringComparison.Ordinal) || Thm.Equals(themeName, StringComparison.Ordinal))
      {
        return;
      }

      //~Write($"TrcW:>     ~> ThemeApplier()   '{themeName}'  to  '{WinFile}' ... Dicts --/++:\r\n");
      //~Application.Current.Resources.MergedDictionaries.ToList().ForEach(r => _logger.Log(LogLevel.Trace, $"~~WinBase       ~> -- Removing: {((System.Windows.Markup.IUriContext)r)?.BaseUri?.AbsolutePath.Replace(pref, "..."/*, StringComparison.OrdinalIgnoreCase*/)}"));

      var suri = $"{pref}{themeName}.xaml";
      if (Application.LoadComponent(new Uri(suri, UriKind.RelativeOrAbsolute)) is ResourceDictionary dict)
      {
        ResourceDictionary? rd;
        while ((rd = Application.Current.Resources.MergedDictionaries.FirstOrDefault(r => ((IUriContext)r)?.BaseUri?.AbsolutePath?.Contains(pref
#if MockingCore3
#else
          , StringComparison.OrdinalIgnoreCase
#endif
          ) == true)) != null)
          Application.Current.Resources.MergedDictionaries.Remove(rd);

        Application.Current.Resources.MergedDictionaries.Add(dict);
      }

      Thm = themeName;

      //~Application.Current.Resources.MergedDictionaries.ToList().ForEach(r => _logger.Log(LogLevel.Trace, $"~~WinBase       ~> ++ Adding:   {((System.Windows.Markup.IUriContext)r)?.BaseUri?.AbsolutePath.Replace(pref, "..."/*, StringComparison.OrdinalIgnoreCase*/)}"));
      //~Write($"TrcW:>     ~> ThemeApplier()   '{themeName}'  to  '{WinFile}' is done. \r\n");
    }
    catch (Exception ex) { _logger.LogError(ex, "ApplyTheme()"); throw; }
  }
  protected async void CloseBaseWindow(object s, RoutedEventArgs e)
  {
    _logger.Log(LogLevel.Trace, $"██WinBase  ..CloseBaseWindow({s.GetType().Name}) ");
    await Task.Delay(333);
    Close();
  }
  void CloseShutdown(string from, [CallerMemberName] string? cmn = "")
  {
    _logger.Log(LogLevel.Trace, $"██WinBase  {cmn}.CloseShutdown({from}) ");
    try
    {
      //Hide();
      //await Task.Delay(2500);
      Close();
    }
    catch (Exception ex) { _logger.LogError(ex, "CloseShutdown()"); throw; }

    /*Application.Current.Shutdown();*/
  }
  void OnMouseLeftButtonDown_(MouseButtonEventArgs e) //tu: workaround for  "Can only call DragMove when primary mouse button is down." (2021-03-10: pre-opened dropdown seemingly caused the error)
  {
    if (DragMovable && e.LeftButton == MouseButtonState.Pressed) DragMove();
  }
  void OnMouseWheel_(MouseWheelEventArgs e)
  {
    if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) return; ZV += e.Delta * .000834; e.Handled = true; _logger.Log(LogLevel.Trace, Title = $"~~WinBase  >>ZV:{ZV,12}   (Zoom Value)");
  }
  void OnKeyUp_(KeyEventArgs e)
  {
    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
      switch (e.Key)
      {
        default: break;
        case Key.NumPad0:
        case Key.D0:        /**/ ZV = 1.00; break;
        case Key.OemMinus:  /**/ ZV /= 1.1; break;
        case Key.OemPlus:   /**/ ZV *= 1.1; break;
      }
    else if (e.Key == Key.Escape)
      CloseShutdown("WinBase.KeyUp");

    base.OnKeyUp(e);
  }
  protected override void OnSourceInitialized(EventArgs e)
  {
    base.OnSourceInitialized(e);

    if (IgnoreWindowPlacement) return;

    try
    {
      StandardLib.Helpers.WinAPI.WindowPlacement winPlcmnt;

      try
      {
        var wpContainer = JsonFileSerializer.Load<StandardLib.Helpers.WinAPI.WPContainer>(WinCoordinatesFile);
        ZV = wpContainer.Zb == 0 ? 1 : wpContainer.Zb;
        winPlcmnt = wpContainer.WindowPlacement;

        ApplyTheme(string.IsNullOrEmpty(wpContainer.Thm) ? _defaultTheme : wpContainer.Thm); // -- for Mail.sln - causes the error: Cannot find resource named 'WindowStyle_Aav0'. Resource names are case sensitive 
      }
      catch (InvalidOperationException ex1)
      {
        _logger.LogError(ex1, $"■▄▀■ Logged/Ignored    EventArgs: '{e}' is trouble.");

        ZV = 1d;
        try
        {
          winPlcmnt = JsonFileSerializer.Load<StandardLib.Helpers.WinAPI.WindowPlacement>(WinCoordinatesFile);
        }
        catch (Exception ex2)
        {
          var note = $"Apparently EventArgs: '{e}' is trouble.";
          _logger.LogError(ex2, note);
          winPlcmnt = new StandardLib.Helpers.WinAPI.WindowPlacement { normalPosition = new StandardLib.Helpers.WinAPI.Rect(40, 40, 1200, 800) };
        }
      }
      catch (Exception ex3) { _logger.LogError(ex3, $"EventArgs: '{e}' is trouble.   I'm about to throw..."); throw; }

      winPlcmnt.length = Marshal.SizeOf(typeof(StandardLib.Helpers.WinAPI.WindowPlacement));
      winPlcmnt.flags = 0;
      winPlcmnt.showCmd = winPlcmnt.showCmd == _swShowMinimized ? _swShowNormal : winPlcmnt.showCmd;

      if (winPlcmnt.normalPosition.Bottom == 0 && winPlcmnt.normalPosition.Top == 0 && winPlcmnt.normalPosition.Left == 0 && winPlcmnt.normalPosition.Right == 0)
      {
        _logger.Log(LogLevel.Trace, $"~~WinBase   {WinCoordinatesFile,20}: 1st time: Window Positions - all zeros!   {SystemParameters.WorkArea.Width}x{SystemParameters.WorkArea.Height} is this the screen dims?");

        winPlcmnt.normalPosition.Left = _currentLeft + _margin;
        winPlcmnt.normalPosition.Top = _currentTop + _margin;

        winPlcmnt.normalPosition.Right = winPlcmnt.normalPosition.Left + (int)ActualWidth;
        if (winPlcmnt.normalPosition.Right > SystemParameters.WorkArea.Width)
        {
          winPlcmnt.normalPosition.Left = _margin;
          winPlcmnt.normalPosition.Top += _margin + (int)ActualHeight;
          _currentTop += (int)ActualHeight;
          winPlcmnt.normalPosition.Right = winPlcmnt.normalPosition.Left + (int)ActualWidth;
          _currentLeft = _margin;
        }

        winPlcmnt.normalPosition.Bottom = winPlcmnt.normalPosition.Top + (int)ActualHeight;
        if (winPlcmnt.normalPosition.Bottom > SystemParameters.WorkArea.Height)
        {
          _currentLeft =
          winPlcmnt.normalPosition.Top =
            winPlcmnt.normalPosition.Left = _margin;
        }

        _currentLeft += (int)ActualWidth;
      }

      _ = NativeMethods.SetWindowPlacement_(new WindowInteropHelper(this).Handle, ref winPlcmnt); //Note: if window was closed on a monitor that is now disconnected from the computer, SetWindowPlacement will place the window onto a visible monitor.
    }
    catch (InvalidOperationException ex) { _logger.LogError(ex, $"■▄▀■ Logged/Ignored    EventArgs: '{e}' is trouble."); _ = ex.Log(); }
    catch (Exception ex) { _logger.LogError(ex, "..()"); throw; }
  }
  protected override void OnClosing(CancelEventArgs e) // WARNING - Not fired when Application.SessionEnding is fired
  {
    try
    {
      var report = $"WinBase.OnClosing  {GetType().Name}  {(GetHashCode() == Application.Current.MainWindow?.GetHashCode() ? "==App..MainWin" : ""),-16}  ShutdownMode:{Application.Current.ShutdownMode} (//note: OnMainWindowClose causes double call of this method!!!)   ";

      _ = NativeMethods.GetWindowPlacement_(new WindowInteropHelper(this).Handle, out var winPlcmnt);

      if (winPlcmnt.normalPosition.Bottom == 0 && winPlcmnt.normalPosition.Top == 0 && winPlcmnt.normalPosition.Left == 0 && winPlcmnt.normalPosition.Right == 0)
        report += "Window placement NOT saved <==  Window Positions - all zeros!  ..cause it's been closed already, right? ^^";
      else
      {
        JsonFileSerializer.Save(new StandardLib.Helpers.WinAPI.WPContainer { WindowPlacement = winPlcmnt, Zb = ZV, Thm = Thm }, WinCoordinatesFile);  // _logger.Log(LogLevel.Trace, $"### Saved window placement to  {WinFile}.");
        report += "Window placement saved.";
      }

      _logger.Log(LogLevel.Trace, report);

      if (!string.IsNullOrEmpty(KeepOpenReason))
      {
        e.Cancel = MessageBoxResult.Yes != MessageBox.Show(this, $"Cannot close since:\n\n{KeepOpenReason}\n\n...Still insist on closing?", $"{GetType().Name}", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
        if (!e.Cancel)
          KeepOpenReason = null; // prevent double ask ... nogo
      }

      base.OnClosing(e);
    }
    catch (Exception ex) { _logger.LogError(ex, $"■▄▀■ Logged/Ignored  ..since old good values are already there."); _ = ex.Log(); }
  }
  protected override void OnClosed(EventArgs e)
  {
    try
    {
      //tmi: _logger.Log(LogLevel.Trace, $"■ WinBase  OnClosed.");
      base.OnClosed(e);
      KeyUp -= (s, e) => OnKeyUp_(e);
      MouseWheel -= (s, e) => OnMouseWheel_(e);
      MouseLeftButtonDown -= (s, e) => OnMouseLeftButtonDown_(e);
    }
    catch (Exception ex) { _logger.LogError(ex, "OnClosed()"); throw; }
  }
  protected virtual void OnWindowMiniBase(object s, RoutedEventArgs e) => WindowState = WindowState.Minimized;
  protected virtual void OnClosShutdnBase(object s, RoutedEventArgs e) => CloseShutdown("WinBase.BtnClick");

#if DEBUG
  public static bool IsDbg => true;
#else
  public static bool IsDbg => false;
#endif
}

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

public static class FSHelper
{
  public static string GetCreateSafeLogFolderAndFile(string[] fullPaths)
  {
    foreach (var fp in fullPaths)
      if (ExistsOrCreated(Path.GetDirectoryName(fp) ?? throw new ArgumentNullException("▄▀")))
        return fp;

    return "__FallbackName__.log";
  }
  public static string GetCreateSafeLogFolderAndFile(string fullPath) => ExistsOrCreated(Path.GetDirectoryName(fullPath) ?? throw new ArgumentNullException("▄▀")) ? fullPath : "__FallbackName__.log";
  public static bool ExistsOrCreated(string directory) // true if created or exists; false if unable to create.
  {
    try
    {
      if (string.IsNullOrEmpty(directory))
        return true;

      if (Directory.Exists(directory))
        return true;

      _ = Directory.CreateDirectory(directory);

      return Directory.Exists(directory);
    }
    catch (IOException ex) { _ = ex.Log($"Directory.CreateDirectory({directory})"); }
    catch (Exception ex) { _ = ex.Log($"Directory.CreateDirectory({directory})"); throw; }

    return false;
  }
}

public static class ExnLogr // the one and only .net core 3 (Dec2019)
{
  public const string CRLF = "\n   ";
  public static TraceSwitch AppTraceLevelCfg => new("CfgTraceLevelSwitch", "Switch in config file:  <system.diagnostics><switches><!--0-off, 1-error, 2-warn, 3-info, 4-verbose. --><add name='CfgTraceLevelSwitch' value='3' /> ");

  public static string Log(this Exception ex, string? optl = null, [CallerMemberName] string? cmn = "", [CallerFilePath] string cfp = "", [CallerLineNumber] int cln = 0)
  {
    var culpritLine = ex.StackTrace?.Split('\n').Where(r => r.Contains(".cs")).ToList().FirstOrDefault() ?? $"{cfp}:line {333}";
    var (csFilename, csFileline) = GetCulpritLineDetails(culpritLine);

    var msgForPopup = $"{ex?.InnerMessages()}  {ex?.GetType().Name} at {CRLF}{csFilename} ({csFileline}):    {CRLF}{cmn}()    {CRLF}{optl}{CRLF}try {{  }} catch ({ex?.GetType().Name} ex) {{ ex.Log(); }} // insert around {Path.GetFileName(csFilename)} ({csFileline})";

    WriteLine(msgForPopup); // WriteLine($"{DateTimeOffset.Now:HH:mm:ss.f}  {msgForPopup.Replace("\n", "  ").Replace("\r", "  ")}"); // .TraceError just adds the "ProgName.exe : Error : 0" line <= no use.

    TraceStackIfVerbose(ex);

    if (Debugger.IsAttached)
      Debugger.Break();
    else //if (VersionHelper.IsDbgOrRBD && (culpritLine is not null || cfp is not null))
      msgForPopup += OpenVsOnTheCulpritLine(culpritLine ?? cfp);

    return msgForPopup; //todo: catch (Exception fatalEx) { Environment.FailFast("An error occured whilst reporting an error.", fatalEx); }//tu: http://blog.functionalfun.net/2013/05/how-to-debug-silent-crashes-in-net.html //tu: Capturing dump files with Windows Error Reporting: Db a key at HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\Windows Error Reporting\LocalDumps\[Your Application Exe FileName]. In that key, create a string value called DumpFolder, and set it to the folder where you want dumps to be written. Then create a DWORD value called DumpType with a value of 2.
  }

  public static string OpenVsOnTheCulpritLine(string callStackEntryLine)
  {
    var (csFilename, csFileline) = GetCulpritLineDetails(callStackEntryLine);
    return OpenVsOnTheCulpritLine(csFilename, csFileline);
  }

  private static (string csFilename, int csFileline) GetCulpritLineDetails(string callStackEntryLine)
  {
    var parts = callStackEntryLine.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
    return parts.Length < 2
      ? ((string csFilename, int csFileline))($"■ Bad callStackEntryLine: {callStackEntryLine}", -1)
      : ((string csFilename, int csFileline))(parts[^3], int.Parse(parts.Last().Trim('\r')));
  }

  static string OpenVsOnTheCulpritLine(string filename, int fileline) => $"";

  static void TraceStackIfVerbose(Exception? ex)
  {
    if (AppTraceLevelCfg.TraceVerbose)
    {
      var prevLv = IndentLevel;
      var prevSz = IndentSize;
      IndentLevel = 2;
      IndentSize = 2;
      WriteLineIf(AppTraceLevelCfg.TraceVerbose, ex?.StackTrace);
      IndentLevel = prevLv;
      IndentSize = prevSz;
    }
  }

  public static string InnerMessages(this Exception? ex, char delimiter = '\n')
  {
    StringBuilder sb = new();
    while (ex != null)
    {
      _ = sb.Append($"{ex.Message}{delimiter}");
      ex = ex.InnerException;
    }

    return sb.ToString();
  }
  public static string InnermostMessage(this Exception ex)
  {
    while (ex != null)
    {
      if (ex.InnerException == null)
        return ex.Message;

      ex = ex.InnerException;
    }

    return "This is very-very odd.";
  }

  #region Proposals - cop
  #endregion
}
