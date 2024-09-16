using Azure.Core;

namespace MSGraphGetPhotoToTheLatestVersionPOC;

class MyTokenCredential(string accessToken) : Azure.Core.TokenCredential
{
  public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken) => new(accessToken, DateTimeOffset.Now.AddDays(11));

  public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken) => new(GetToken(requestContext, cancellationToken));
}

//todo: manage apps:  https://account.live.com/consent/Manage?uaid=80c30464e3c748239e49630886b523d4&fn=email&guat=1
//todo: manage apps:  https://account.live.com/consent/Manage?uaid=80c30464e3c748239e49630886b523d4&fn=email&guat=1