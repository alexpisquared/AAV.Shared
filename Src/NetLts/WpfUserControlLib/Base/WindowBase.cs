namespace WpfUserControlLib.Base;
public partial class WindowBase : Window
{
  protected readonly DateTimeOffset _mvwStarted = DateTimeOffset.Now;
  protected readonly ILogger _logger;
  const double _defaultZoomV = 1.25;
  const string _defaultTheme = "No Theme";
  const int _swShowNormal = 1, _swShowMinimized = 2, _margin = 0;
  static int _currentTop = 0, _currentLeft = 0;
  string WinFile => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @$"AppSettings\{AppDomain.CurrentDomain.FriendlyName}\{GetType().Name}{(VersionHelper.IsDbg ? ".dbg" : "")}.json");
  public WindowBase() : this(new LoggerFactory().CreateLogger<Window>()) { } // no logging needed; for cases like error popups, etc.
  public WindowBase(ILogger logger)
  {
    _logger = logger;

    Topmost = Debugger.IsAttached;

    MouseLeftButtonDown += (s, e) => OnMouseLeftButtonDown_(e);
    MouseWheel += (s, e) => OnMouseWheel_(e);
    KeyUp += (s, e) => OnKeyUp_(e);
  }
  protected bool IgnoreEscape { get; set; } = !VersionHelper.IsDbgOrRBD;
  protected bool IgnoreWindowPlacement { get; set; }
  protected bool DragMovable { get; set; } = true;
  public static readonly DependencyProperty ZVProperty = DependencyProperty.Register("ZV", typeof(double), typeof(WindowBase), new PropertyMetadata(_defaultZoomV)); public double ZV { get => (double)GetValue(ZVProperty); set => SetValue(ZVProperty, value); }
  public static readonly DependencyProperty ThmProperty = DependencyProperty.Register("Thm", typeof(string), typeof(WindowBase), new PropertyMetadata(_defaultTheme)); public string Thm { get => (string)GetValue(ThmProperty); set => SetValue(ThmProperty, value); }
  public string? KeepOpenReason { get; set; } = null; // """ [KeepOpenReason = "Changes have not been saved."] """;
  protected void ApplyTheme(string themeName, [CallerMemberName] string? cmn = "")
  {
    _logger.Log(LogLevel.Trace, $"► WinBase  shw{(DateTimeOffset.Now - _mvwStarted).TotalSeconds,4:N1}s  {themeName,-26}{cmn} -> {nameof(WindowBase)}.{nameof(ApplyTheme)}().");

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
    catch (Exception ex) { ex.Pop(this, $"New theme '{themeName}' is trouble.", lgr: _logger); }
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
    catch (Exception ex) { ex.Pop(this, $"CloseShutdown()", lgr: _logger); }
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
    else if (e.Key == Key.Escape && !IgnoreEscape)
      CloseShutdown("WinBase.KeyUp");

    base.OnKeyUp(e);
  }
  protected override void OnSourceInitialized(EventArgs e)
  {
    base.OnSourceInitialized(e);

    if (IgnoreWindowPlacement) return;

    try
    {
      NativeMethods.WindowPlacement winPlcmnt;

      try
      {
        var wpContainer = JsonFileSerializer.Load<NativeMethods.WPContainer>(WinFile);
        ZV = wpContainer.Zb == 0 ? 1 : wpContainer.Zb;
        winPlcmnt = wpContainer.WindowPlacement;

        ApplyTheme(string.IsNullOrEmpty(wpContainer.Thm) ? _defaultTheme : wpContainer.Thm); // -- for Mail.sln - causes the error: Cannot find resource named 'WindowStyle_Aav0'. Resource names are case sensitive 
      }
      catch (InvalidOperationException ex1)
      {
        _logger.LogError(ex1, $"■▄▀■ Logged/Ignored    EventArgs: '{e}' is trouble.");
        _ = ex1.Log();
        ZV = 1d;
        try
        {
          winPlcmnt = JsonFileSerializer.Load<NativeMethods.WindowPlacement>(WinFile);
        }
        catch (Exception ex2)
        {
          var note = $"Apparently EventArgs: '{e}' is trouble.";
          ex2.Pop(owner: this, note, lgr: _logger);
          winPlcmnt = new NativeMethods.WindowPlacement { normalPosition = new NativeMethods.Rect(40, 40, 1200, 800) };
        }
      }
      catch (Exception ex3) { ex3.Pop($"EventArgs: '{e}' is trouble.   I'm about to throw...", lgr: _logger); throw; }

      winPlcmnt.length = Marshal.SizeOf(typeof(NativeMethods.WindowPlacement));
      winPlcmnt.flags = 0;
      winPlcmnt.showCmd = winPlcmnt.showCmd == _swShowMinimized ? _swShowNormal : winPlcmnt.showCmd;

      if (winPlcmnt.normalPosition.Bottom == 0 && winPlcmnt.normalPosition.Top == 0 && winPlcmnt.normalPosition.Left == 0 && winPlcmnt.normalPosition.Right == 0)
      {
        _logger.Log(LogLevel.Trace, $"~~WinBase   {WinFile,20}: 1st time: Window Positions - all zeros!   {SystemParameters.WorkArea.Width}x{SystemParameters.WorkArea.Height} is this the screen dims?");

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
    catch (Exception ex) { ex.Pop("I'm about to throw... NOT!!!", lgr: _logger); }
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
        JsonFileSerializer.Save(new NativeMethods.WPContainer { WindowPlacement = winPlcmnt, Zb = ZV, Thm = Thm }, WinFile);  // _logger.Log(LogLevel.Trace, $"### Saved window placement to  {WinFile}.");
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
    catch (Exception ex) { ex.Pop(this, ex.Message, lgr: _logger); }
  }
  protected virtual void OnWindowMiniBase(object s, RoutedEventArgs e) => WindowState = WindowState.Minimized;
  protected virtual void OnClosShutdnBase(object s, RoutedEventArgs e) => CloseShutdown("WinBase.BtnClick");
}