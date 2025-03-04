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
 Add Permission:
  - Microsoft Graph (top big button)
  - Applicatoin Permissinn
  - Calendars - add all three
  -   Still getting the error: ▒▒ Error: Access is denied. Check credentials and try again.

### 2024-12-03  Trying Copilot Commit message customization:
    Created on a new branch:  Feature01.
    Added to settings.json:

Why adding any of these lines to the Settings.json does not work?
Claude:
  "github.copilot.chat.variables": { "branchName": "git rev-parse --abbrev-ref HEAD" },
  "github.copilot.chat.initialPrompt": "When generating commit messages, always format them as: [$branchName] <description>"
  "github.copilot.chat.initialPrompt": "You are a helpful AI programming assistant. When generating git commit messages: 1) Always start with the current branch name 2) Follow with a concise description of changes 3) Use present tense 4) Keep under 72 characters total",
Copilot:
  "github.copilot.chat.initialPrompt.enabled": true,
  "github.copilot.chat.codeGeneration.instructions": [    {      "text": "Include the current branch name at the beginning of the commit message."    }  ],
  "github.copilot.chat.codeGeneration.instructions.enabled": true,

==>  It does not work.  It does not show the branch name in the commit message.  It is not clear why.

  Added this to the  prepare-commit-msg file:
$branchName = git rev-parse --abbrev-ref HEAD
$commitMsgFile = $args[0]
$commitMsg = Get-Content $commitMsgFile
Set-Content $commitMsgFile -Value "[$branchName] $commitMsg"
  Then follower instructions to make it executable:
  
Save the file with a .ps1 extension, for example, prepare-commit-msg.ps1.
Run the Script:

Open PowerShell and navigate to your repository directory.
Run the script manually whenever you need to prepare a commit message:
.\prepare-commit-msg.ps1 .git\hooks\prepare-commit-msg
These methods should help you get around the chmod error on Windows. Let me know if you need further assistance!

forget it for now.  It is not working.  I will try it later.


### 2024-12-03  Tested latest versions of MSGraph:
  Looks like it is a bit better as almost all videos are playing.  But still 500 mb is not not.  I will wait for the next version to see if it is fixed.

### 2025-03-04  Email alarm:
  I have to set up an email alarm for the next week.
