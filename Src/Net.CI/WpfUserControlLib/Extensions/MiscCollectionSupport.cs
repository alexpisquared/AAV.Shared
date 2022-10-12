namespace WpfUserControlLib.Extensions;

public static class MiscCollectionSupport
{
  public static void ClearAddRangeAuto<T>(this Collection<T> trg, IEnumerable<T> src)
  {
    try
    {
      if (Application.Current.Dispatcher.CheckAccess()) // if on UI thread
      {
        trg.Clear();
        src.ToList().ForEach(trg.Add);
      }
      else
        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
        {
          trg.Clear();
          src.ToList().ForEach(trg.Add);
        }));
    }
    catch (Exception ex) { ex.Log(); throw; }
  }
}
