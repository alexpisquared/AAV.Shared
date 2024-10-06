using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using MSGraphGetPhotoToTheLatestVersionPOC;
using Pastel;
using System.Drawing;
using static System.Console;

namespace MsGraphLibVer1;

public class MyGraphServiceClient
{
  protected GraphServiceClient? _graphServiceClient;

  protected async Task<(bool success, string report, AuthenticationResult? authResult)> InitializeGraphClientIfNeeded(string clientId)
  {
    if (_graphServiceClient is not null) return (true, "already done initialization for _graphServiceClient.", null);

    string[] _scopes = [
      "User.Read",
      "Mail.Read",
      "Mail.Send",
      "Mail.ReadWrite", // ReadWrite for convoluted mode only.
      "Files.Read",
      "Calendar.Read"];

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
          return (false, $"■ Error Acquiring Token INTERACTIVELY:   {msalEx}", null);
        }
      }
      catch (Exception ex) { return (false, $"Error Acquiring Token Silently:   {ex}", null); }

      _graphServiceClient = new GraphServiceClient(new MyTokenCredential(authResult.AccessToken), _scopes);

      return (true, ReportResult(authResult), authResult);
    }
    catch (Exception ex) { return (false, $"Error Acquiring Token Silently:   {ex}", null); }
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
  Got a token for:  {result.Account.Username} 
     Token source:  {result.AuthenticationResultMetadata.TokenSource}     
       Expires on:  {result.ExpiresOn.ToLocalTime()}     
       {result.AccessToken[..12]}...{result.AccessToken[^12..]}
 """;
}

//todo: manage apps:  https://account.live.com/consent/Manage?uaid=80c30464e3c748239e49630886b523d4&fn=email&guat=1
//todo: manage apps:  https://account.live.com/consent/Manage?uaid=80c30464e3c748239e49630886b523d4&fn=email&guat=1