namespace WpfUserControlLib.Extensions;

public static class WpfUtils
{
  public static Window FindParentWindow(this FrameworkElement element)
  {
    if (element.Parent as Window != null) return element.Parent as Window ?? Application.Current.MainWindow;

    if (element.Parent is FrameworkElement frel) return frel.FindParentWindow();

    return Application.Current.MainWindow;
  }

  public static void AutoInvokeOnUiThread(Action action)
  {
    _ = action ?? throw new ArgumentNullException(paramName: nameof(action)); // == if (action == null) throw new ArgumentNullException(paramName: nameof(action));

    if (Application.Current == null || Application.Current.Dispatcher.CheckAccess())
      action();
    else _ = Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
      action()));
  }
  public static bool? AutoInvokeOnUiThread(Func<bool?> action)
  {
    _ = action ?? throw new ArgumentNullException(paramName: nameof(action)); // == if (action == null) throw new ArgumentNullException(paramName: nameof(action));

    bool? rv = false;

    if (Application.Current == null || Application.Current.Dispatcher.CheckAccess())
      rv = action();
    else _ = Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
      rv = action()));

    return rv;
  }
  public static async Task AutoInvokeOnUiThreadAsync(Func<Task> action)
  {
    _ = action ?? throw new ArgumentNullException(paramName: nameof(action)); // == if (action == null) throw new ArgumentNullException(paramName: nameof(action));

    if (Application.Current == null || Application.Current.Dispatcher.CheckAccess())
      await action()/*.ConfigureAwait(false)*/;
    else await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(async () =>
      await action()/*.ConfigureAwait(false)*/));
  }
  public static void AutoInvokeOnUiThread(Action<string> action, string param0)
  {
    _ = action ?? throw new ArgumentNullException(paramName: nameof(action)); // == if (action == null) throw new ArgumentNullException(paramName: nameof(action));

    if (Application.Current == null || Application.Current.Dispatcher.CheckAccess())
      action(param0);
    else _ = Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
      action(param0)));
  }
  public static void AutoInvokeOnUiThread(Action<DependencyProperty, object> action, DependencyProperty dp, object value)
  {
    _ = action ?? throw new ArgumentNullException(paramName: nameof(action)); // == if (action == null) throw new ArgumentNullException(paramName: nameof(action));

    if (Application.Current == null || Application.Current.Dispatcher.CheckAccess())
      action(dp, value);
    else _ = Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
      action(dp, value)));
  }
}
