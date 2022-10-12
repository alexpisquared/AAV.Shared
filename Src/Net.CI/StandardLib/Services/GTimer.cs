namespace CI.Standard.Lib.Services;

public interface IGTimer
{
  DateTimeOffset AppStart { get; }
}

public class GTimer : IGTimer
{
  public GTimer(DateTimeOffset t) => AppStart = t;
  public DateTimeOffset AppStart { get; }
}
