namespace CI.Standard.Lib.Services;

public class IgnoreCaseComparer : IEqualityComparer<string>
{
  public bool Equals(string? b1, string? b2)
  {
    if (b2 == null && b1 == null)
      return true;
    else if (b1 == null || b2 == null)
      return false;
    else if (b1.Equals(b2, StringComparison.OrdinalIgnoreCase))
      return true;
    else
      return false;
  }

  public int GetHashCode(string bx) => bx.Length.GetHashCode();
}