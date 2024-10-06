### 2024-09-22

misc:  
  https://learn.microsoft.com/en-us/graph/sdks/choose-authentication-providers?tabs=csharp

///todo: ContentStream:
/// https://learn.microsoft.com/en-us/graph/api/driveitem-get-contentstream?view=graph-rest-beta&tabs=http
/// Download a partial range of bytes --- looks like it is much better ... at least promising.

#  RESOLUTION:
## ==> wait for Beta to take over and hope it fixes streaming of videos 
## Microsoft.Graph.Beta" 5.81.0-preview  does not work.  ...nor:
## Microsoft.Graph       5.58.0  

### 2024-10-05  tested:
5.59: up to 50 mb works fine 
5.81: beta gives "API not found" error for all movie and doc files.

See for more code samples: https://developer.microsoft.com/en-us/graph/graph-explorer

### 2024-10-06  Trying Calendar:
The App
  https://portal.azure.com/?feature.quickstart=true#view/Microsoft_AAD_RegisteredApps/ApplicationMenuBlade/~/Overview/appId/af27ddbb-f29b-4588-af5f-4d03c03c03d0/isMSAApp~/false
Permission fot the App:
  https://portal.azure.com/?feature.quickstart=true#view/Microsoft_AAD_RegisteredApps/ApplicationMenuBlade/~/CallAnAPI/appId/af27ddbb-f29b-4588-af5f-4d03c03c03d0/isMSAApp~/false
 Add POermission:
  - Microsoft Graph (top big button)
  - Applicatoin Permissinn
  - Calendars - add all three
  -   Still getting the error: ▒▒ Error: Access is denied. Check credentials and try again.