#define brave 
using AAV.WPF.Ext;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MVVM.Common
{
  public abstract class BindableBaseViewModel : BindableBase
  {
    static readonly TraceSwitch appTraceLevelLcl = new TraceSwitch("Display name", "Descr-n") { Level = TraceLevel.Error };

    protected BindableBaseViewModel()
    {
      CloseAppCmd = new RelayCommand(x => OnRequestClose(), param => CanClose())
      {
#if DEBUG
        GestureKey = Key.Escape
#else
        GestureKey = Key.F4,
        GestureModifier = ModifierKeys.Alt
#endif
      };
    }

    protected virtual void AutoExec() => Trace.WriteLine("AutoExec()");
    protected virtual void AutoExecSynch() => Trace.WriteLine("AutoExecSynch()");
    protected virtual async Task AutoExecAsync() { Trace.WriteLine("AutoExecAsync()"); await Task.Yield(); }

    public ICommand CloseAppCmd { get; }

    public event EventHandler? RequestClose;
    protected virtual void OnRequestClose() => RequestClose?.Invoke(this, EventArgs.Empty);
    protected virtual bool CanClose() => true;
    protected virtual async Task ClosingVM() => await Task.Delay(250).ConfigureAwait(false);  // public abstract void Closing();

    protected static bool _cancelClosing = false;
    public static void CloseEvent(Window view, BindableBaseViewModel vwMdl)
    {
      async void handler(object? sender, EventArgs e)
      {
        await vwMdl.ClosingVM();
        if (_cancelClosing) return;

        vwMdl.RequestClose -= handler;
        try
        {
          if (view != null)
          {
            WpfUtils.AutoInvokeOnUiThread(view.Close); // if (Application.Current.Dispatcher.CheckAccess()) view.Close(); else await Application.Current.Dispatcher. BeginInvoke(DispatcherPriority.Normal, new Action(() => view.Close()));
          }
        }
        catch (Exception ex) { Trace.WriteLine(ex.Message, System.Reflection.MethodInfo.GetCurrentMethod()?.Name); if (Debugger.IsAttached) Debugger.Break(); }
      } // When the ViewModel asks to be closed, close the window.

      vwMdl.RequestClose += handler;
    }

    public static void ShowMvvm(BindableBaseViewModel vMdl, Window view)
    {
      view.DataContext = vMdl;
      BindableBaseViewModel.CloseEvent(view, vMdl);
      Task.Run(() => vMdl.AutoExec()); //			await 
      //vMdl.AutoExec();
      view.Show();
    }
    public static bool? ShowModalMvvm(BindableBaseViewModel vMdl, Window view, Window? owner = null)
    {
      if (owner != null)
      {
        view.Owner = owner;
        view.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      }

      view.DataContext = vMdl;
      BindableBaseViewModel.CloseEvent(view, vMdl);

      WpfUtils.AutoInvokeOnUiThread(vMdl.AutoExec);

      return view.ShowDialog();
    }
    public static bool? ShowModalMvvmAsync(BindableBaseViewModel vMdl, Window view, Window? owner = null) // public static async Task<bool?> ShowModalMvvmAsync(BindableBaseViewModel vMdl, Window view, Window owner = null)
    {
      if (owner != null)
      {
        view.Owner = owner;
        view.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      }

      view.DataContext = vMdl;
      CloseEvent(view, vMdl);

#if brave
      Task.Run(async () => await WpfUtils.AutoInvokeOnUiThreadAsync(vMdl.AutoExecAsync));
#else
            await vMdl.AutoExecAsync(); //todo:             
#endif

      return view.ShowDialog();
    }

    protected static async Task refreshUi() => await Application.Current.Dispatcher.BeginInvoke(new ThreadStart(() => refreshUiSynch()));
    protected static void refreshUiSynch() => CommandManager.InvalidateRequerySuggested();  //tu: Sticky UI state fix for MVVM (May2015)
  }
}