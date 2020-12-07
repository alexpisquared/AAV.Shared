using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace MVVM.Common
{
  public class ViewModelBase : INotifyPropertyChanged
  {
    protected bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) // From http://danrigby.com/2012/04/01/inotifypropertychanged-the-net-4-5-way-revisited/
    {
      if (Equals(storage, value)) return false;

      storage = value;
      OnPropertyChanged(propertyName);
      return true;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); //todo: see GH!!! this.PropertyChanged += (s, e) => { this.RefreshCommand.RaiseCanExecuteChanged(); };

    protected static async Task refreshUiAsync() => await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => refreshUiSynch());
    protected static void refreshUiSynch() {; } // CommandManager.InvalidateRequerySuggested(); //tu: Sticky UI state fix for MVVM (May2015)

    /// In WinRT, you must update/raise CanExecuteChanged manually. There is no CommandManager to do this globally. You could look at this as a pain in the neck, or a serious performance boost now that CanExecute is not called constantly. It does mean you have to think about cascading property changes where before you did not have to.But this is how it is. Manual.
    /// public void RaiseCanExecuteChanged()    {      if (CanExecuteChanged != null)        CanExecuteChanged(this, EventArgs.Empty);    }
    /// also, see https://stackoverflow.com/questions/34996198/the-name-commandmanager-does-not-exist-in-the-current-context-visual-studio-2/34996251
  }
}