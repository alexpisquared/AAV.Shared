namespace SerilogHelperLib;

public class Constants
{
  public const string EmptySpecial = " \t \t \t ";
  public const string ExtReleaseNote = "EXT Release Note:";

#if DEBUG
  public static bool IsDbg => true;
#else
  public static bool IsDbg => false;
#endif
}
