namespace StandardLib.Helpers;

public class DevOps
{
  public static bool IsDbg
  {
    get
    {
#if DEBUG
      return true;
#else
      return false;
#endif
    }
  }

  public static bool IsDevMachine => IsDevMachineO || IsDevMachineH;
  public static bool IsDevMachineO => HashString(Environment.MachineName).Equals(_devMachineO, StringComparison.OrdinalIgnoreCase);
  public static bool IsDevMachineH => HashString(Environment.MachineName).Equals(_devMachineH, StringComparison.OrdinalIgnoreCase);
  public static bool IsSelectModes => _selects.Contains(HashString(Environment.UserName));

  public static string HashString(string value)
  {
    using var hashAlgo = System.Security.Cryptography.SHA256.Create();

    //needs wpf?: return System.Convert.ToHexString(hashAlgo.ComputeHash(Encoding.UTF8.GetBytes(value)));

    var sb = new StringBuilder();
    foreach (var b in hashAlgo.ComputeHash(Encoding.UTF8.GetBytes(value))) _ = sb.Append(b.ToString("x2"));

    if (DateTime.Now == DateTime.Today)
      WriteLine($"TrWr:> {value,16}  =>  {sb}");

    return sb.ToString();
  }

  static readonly string[] _selects = new[] {
  //"06f8aa28c4c328ccb33b1c595ab1b39d27ed9ddb2bbfc2fc51d556f56d2e699f", // test only of a..xp temp
    "2k8s4a5f7a363f194b1b5916e71c1d2065cf0c4afdaf33kwjhdhfha5a1952888", // test of always False
    "977f4a5f7a363f194b1b5916e71c1d2065cf0c4afdaf33291ec5c6a5a1952f8f"
  };
  const string
    _devMachineO = "c6575bfeb9a42bdc01bac5e621bcc7cc884b574d3078b3d5bbe8b2db7dd1f968", // d21
    _devMachineH = "73d007eb0b0a148c1205f3ee36d0e792ab8d0833054a8a94c50f0d197bb0fd8d"; // rzr (both are lowercase on rzr1 ... )
}