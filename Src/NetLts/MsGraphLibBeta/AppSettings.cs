using Microsoft.Identity.Client.Extensions.Msal;

namespace MSGraphGetPhotoToTheLatestVersionPOC;

static class AppSettings 
{
  public static readonly string[] Scopes = ["user.read"]; //, "Files.Read" };     //todo: worked on Nuc2 with this on
  public const string Authority = "https://login.microsoftonline.com/common";

  // Cache settings
  public static readonly string CacheDir = MsalCacheHelper.UserRootDirectory;
  public const string CacheFileName = "myApp_msal_cache.txt";
  public const string CacheFileNam2 = "myApp_msal_cache.plaintext.txt"; // do not use the same file name so as not to overwrite the encrypted version

  public const string KeyChainServiceName = "myApp_msal_service";
  public const string KeyChainAccountName = "myApp_msal_account";

  public const string LinuxKeyRingSchema = "com.AavPro.devTools.tokenCache";
  public const string LinuxKeyRingCollection = MsalCacheHelper.LinuxKeyRingDefaultCollection;
  public const string LinuxKeyRingLabel = "MSAL token cache for all AavPro dev tool apps.";
  public static readonly KeyValuePair<string, string> LinuxKeyRingAttr1 = new("Version", "1");
  public static readonly KeyValuePair<string, string> LinuxKeyRingAttr2 = new("ProductGroup", "MyApps");
}

//todo: manage apps:  https://account.live.com/consent/Manage?uaid=80c30464e3c748239e49630886b523d4&fn=email&guat=1
//todo: manage apps:  https://account.live.com/consent/Manage?uaid=80c30464e3c748239e49630886b523d4&fn=email&guat=1