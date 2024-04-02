namespace WpfUserControlLib.Extensions;

public static class MiscCollectionSupport
{
  public static void ClearAddRangeAuto<T>(this Collection<T> trg, IEnumerable<T> src)
  {
    try
    {
      if (Application.Current.Dispatcher.CheckAccess()) // if on UI thread
        ClearAddRange(trg, src);
      else
        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ClearAddRange(trg, src)));
    }
    catch (Exception ex) { ex.Log(); throw; }
  }

  static void ClearAddRange<T>(Collection<T> trg, IEnumerable<T> src)
  {
    trg.Clear();
    src.ToList().ForEach(trg.Add);
  }
}