using Azure.Core;

namespace MsGraphLib____;

public class MyGraphServiceClient
{
  protected GraphServiceClient? _graphServiceClient;

  public async Task<(bool success, string report, AuthenticationResult? authResult, TokenCredential? tokenCredential)> InitializeGraphClientIfNeeded(string clientId)
  {
    if (_graphServiceClient is not null) return (true, "  already done initialization for _graphServiceClient.", null, null);

    string[] _scopes = [ //tu: manage apps:  https://portal.azure.com/?feature.quickstart=true#view/Microsoft_AAD_RegisteredApps/ApplicationMenuBlade/~/Overview/appId/af27ddbb-f29b-4588-af5f-4d03c03c03d0/isMSAApp~/false
      "User.Read",
      //"Mail.Read",
      //"Mail.Send",
      //"Mail.ReadWrite", // ReadWrite for convoluted mode only.
      "Calendars.Read",
      "Calendars.ReadBasic",
      "Calendars.Read.Shared",
      "Calendars.ReadBasic",
      "Calendars.ReadWrite"
      //"Files.Read"
      ];

    try
    {
      var publicClientApplication = PublicClientApplicationBuilder.Create(clientId)
            .WithAuthority(AzureCloudInstance.AzurePublic, tenant: "common")
            .WithRedirectUri("http://localhost").Build();

      (await CreateCacheHelperAsync(clientId).ConfigureAwait(false)).RegisterCache(publicClientApplication.UserTokenCache); //without this line, account1 is null => then it ALWAYS goes interactive !!! => use for testing it.

      var account1 = (await publicClientApplication.GetAccountsAsync().ConfigureAwait(false)).FirstOrDefault();

      AuthenticationResult authResult;
      try
      {
        authResult = await publicClientApplication.AcquireTokenSilent(_scopes, account1).ExecuteAsync().ConfigureAwait(false);
      }
      catch (MsalUiRequiredException ex)
      {
        WriteLine($"■ MsalUiRequiredException: Error Acquiring Token Silently ==> going interactive...    \n  {ex.Message}".Pastel(Color.Orange));

        try
        {
          authResult = await publicClientApplication.AcquireTokenInteractive(_scopes)
              .WithAccount(account1)
              .WithPrompt(Prompt.SelectAccount)
              .ExecuteAsync();
        }
        catch (MsalException msalEx)
        {
          WriteLine($"■ Error Acquiring Token INTERACTIVELY:   \n  {msalEx.Message}".Pastel(Color.IndianRed));
          return (false, $"■ Error Acquiring Token INTERACTIVELY:   {msalEx}", null, null);
        }
      }
      catch (Exception ex) { return (false, $"Error Acquiring Token Silently:   {ex}", null, null); }

      var t = new MyTokenCredential(authResult.AccessToken);
      _graphServiceClient = new GraphServiceClient(t, _scopes);

      return (true, ReportResult(authResult), authResult, t);
    }
    catch (Exception ex) { return (false, $"Error Acquiring Token Silently:   {ex}", null, null); }
  }

  static async Task<MsalCacheHelper> CreateCacheHelperAsync(string clientId)
  {
    try
    {
      var storageProperties = new StorageCreationPropertiesBuilder(AppSettings.CacheFileName, AppSettings.CacheDir)
        .WithLinuxKeyring(
          AppSettings.LinuxKeyRingSchema,
          AppSettings.LinuxKeyRingCollection,
          AppSettings.LinuxKeyRingLabel,
          AppSettings.LinuxKeyRingAttr1,
          AppSettings.LinuxKeyRingAttr2)
        .WithMacKeyChain(
          AppSettings.KeyChainServiceName,
          AppSettings.KeyChainAccountName)
        .WithCacheChangedEvent(
          clientId,
          AppSettings.Authority).Build();

      var cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties).ConfigureAwait(false);
      cacheHelper.VerifyPersistence();
      return cacheHelper;
    }
    catch (MsalCachePersistenceException)
    {
      var storageProperties = new StorageCreationPropertiesBuilder(AppSettings.CacheFileNam2, AppSettings.CacheDir).WithUnprotectedFile().Build();
      var cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties).ConfigureAwait(false);
      cacheHelper.VerifyPersistence();
      return cacheHelper;
    }
  }
  static string ReportResult(AuthenticationResult result) => $""" 
  Got a token for:{result.Account.Username}   Source:{result.AuthenticationResultMetadata.TokenSource}   Expires:{result.ExpiresOn.ToLocalTime():yyyy-MM-dd HH:mm}   {result.AccessToken[..6]}...{result.AccessToken[^6..]}
 """;
}
//todo: manage apps:  https://account.live.com/consent/Manage?uaid=80c30464e3c748239e49630886b523d4&fn=email&guat=1