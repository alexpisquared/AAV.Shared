# //todo: 17 Amazing Community Packages for .NET Developers
> ###### https://www.youtube.com/watch?v=VwxickHIDhc&t=209s
00:00 Community Packages Introduction
00:24 Intellenum
00:56 Vogen
01:38 Fluent Assertions
02:11 LoFuUnit
02:45 GEmojiSharp
03:15 ThoughtStuff.Caching
03:50 FusionCache
04:25 WPF AutoComplete TextBox
05:03 StaticSiteGenerator
05:40 Htmx.Net
06:17 EntityFramework.Exceptions
06:46 PeachPie
07:18 NBomber
07:58 CommandQuery
08:22 LottieSharp
08:51 Configuration.Extensions.EnvironmentFile
09:13 Guard Clauses
09:44 What's Next?

Honorable Mentions:
https://github.com/Bengie23/DeloreanT...
https://github.com/gflisch/Arc4u
https://www.sqlplus.net/


2023-04-19   purge file and history github   Util/Src/Shade/RBC.Intro.md

# THE TRICK WAS TO USE IN-PRIVATE BROWSING OR OTHER BROWSER!!! (as the page was cached)

To remove a file from Git history, you can use the git filter-branch command1. The command rewrites the branch’s history and passes it the previous command. You can use 
It’s important to note that changing history, especially for content you’ve already pushed, is a drastic action and should be avoided if possible1.

# Purges `Src/Shade/RBC.Intro.md` from history
git filter-branch --force --index-filter "git rm --cached --ignore-unmatch Src/Shade/RBC.Intro.md" --prune-empty --tag-name-filter cat -- --all

# Force pushes the changes to the remote repository
git push origin --force --all

Alt1:
pip3 install git-filter-repo
C:\Users\alexp\AppData\Local\Packages\PythonSoftwareFoundation.Python.3.9_qbz5n2kfra8p0\LocalCache\local-packages\Python39\Scripts\git-filter-repo.exe --path Src/Shade/RBC.Intro.md --invert-paths

Alt2:
bfg --delete-files Src/Shade/RBC.Intro.md