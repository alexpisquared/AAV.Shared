using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.UI.Xaml.Media.Imaging;

namespace AsLink
{
    public /*static */class IsoStorePoc // https://msdn.microsoft.com/windows/uwp/files/index
  {
    public static async Task<bool> FileExists(StorageFolder roamingFolder, string filename)
    {
      try
      {
        var lstfiles = await roamingFolder.GetFilesAsync();
        return lstfiles.Any(x => x.Name == filename);
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"$#~>{ex.Message}");
        throw;
      }
    }
    public static async Task DeleteIsoFile(StorageFolder roamingFolder, string filename)
    {
      try
      {
        StorageFile storageSrcFile = await roamingFolder.GetFileAsync(filename); //tu: read text contents of the file: String timestamp = await FileIO.ReadTextAsync(storageSrcFile);
        await storageSrcFile.DeleteAsync();
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"$#~>{ex.Message}");
        throw;
      }
    }

    public static async Task<bool> CopyIsoFileToLibFolder(StorageFolder srcFolder, string filename, KnownFolderId trgFolder)
    {
      if (!await FileExists(srcFolder, filename))
        return false;

      StorageFile storageSrcFile = await srcFolder.GetFileAsync(filename); //tu: read text contents of the file: String timestamp = await FileIO.ReadTextAsync(storageSrcFile);
      await CopyIsoFileToLibFolder(storageSrcFile, trgFolder);
      return true;
    }
    public static async Task CopyIsoFileToLibFolder(StorageFile srcFile, KnownFolderId trgFolderId)
    {
      try
      {
        var desiredNewName = $"{srcFile.Name}.NOT.jpg";
        StorageFolder trgFolder = await KnownFolders.GetFolderForUserAsync(NullForCurrentUser, trgFolderId);
        StorageFile fileCopy = await srcFile.CopyAsync(trgFolder, desiredNewName, NameCollisionOption.ReplaceExisting);
        Debug.WriteLine(($"The file '{desiredNewName}' was copied and the new file was named '{fileCopy.Name}'."));
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"$#~>{ex.Message}");
        throw;
      }
    }

    public static async Task<bool> CopyLibFileToIsoFolder(StorageFolder trgFolder, string filename, KnownFolderId srcFolderId)
    {
      try
      {
        var desiredNewName = $"{filename}.NOT.jpg";

        StorageFolder srcDir = await KnownFolders.GetFolderForUserAsync(NullForCurrentUser, srcFolderId);
        var file = (StorageFile)await srcDir.TryGetItemAsync(desiredNewName);
        if (file == null)
        {
          return false;
        }

        StorageFile trgFile = await file.CopyAsync(trgFolder, filename, NameCollisionOption.ReplaceExisting);
        Debug.WriteLine(($"The file '{file.Name}' was copied and the new file was named '{trgFile.Name}'."));

        return true;
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"$#~>{ex.Message}");
        throw;
      }
    }
    public static async Task CopyLibFileToIsoFolder(StorageFile file, KnownFolderId srcFolder)
    {
      try
      {
        StorageFolder trg = await KnownFolders.GetFolderForUserAsync(NullForCurrentUser, srcFolder);
        StorageFile trgFile = await file.CopyAsync(trg, file.Name, NameCollisionOption.ReplaceExisting);
        Debug.WriteLine(($"The file '{file.Name}' was copied and the new file was named '{trgFile.Name}'."));
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"$#~>{ex.Message}");
        throw;
      }
    }

    static readonly Windows.System.User NullForCurrentUser = null;

    public static async Task<IReadOnlyList<StorageFile>> LoadFromLibFolder(KnownFolderId trgFolder)
    {
      try
      {
        var qo = new QueryOptions(CommonFileQuery.OrderByName, MediaHelper.AllMediaExtensionsAry); // Set up thumbnail prefetch if needed, e.g. when creating a picture gallery view:         qOpts.SetThumbnailPrefetch(ThumbnailMode.PicturesView, requestedSize: 100u, options: ThumbnailOptions.UseCurrentScale);

        qo.FolderDepth = FolderDepth.Deep; // https://docs.microsoft.com/en-us/windows/uwp/files/quickstart-managing-folders-in-the-music-pictures-and-videos-libraries

        var query = (await KnownFolders.GetFolderForUserAsync(NullForCurrentUser, trgFolder)).CreateFileQueryWithOptions(qo);
        var files = await query.GetFilesAsync();

        return files;
      }
      catch (Exception ex) { Debug.WriteLine($"$#~>{ex.Message}"); if (Debugger.IsAttached) Debugger.Break(); else throw; }
      return null;
    }
    public static async Task<BitmapImage> GetThumbnailBitmapImage(StorageFile sfile, ThumbnailMode mode = ThumbnailMode.MusicView)
    {
      try
      {
#if _Exp
        for (uint i = 40000; i < 100000; i *= 2)
        {
          var tn = await sfile.GetThumbnailAsync(mode, i);
          var bitmapImage = new BitmapImage();
          bitmapImage.SetSource(tn);
          Debug.WriteLine($"{i,6:N0} ==> {tn.OriginalWidth,6:N0} ==> {bitmapImage.PixelWidth,6:N0} <== {sfile.Name}");
        }
#endif

        using (StorageItemThumbnail thumbnail = await sfile.GetThumbnailAsync(mode, 300))
        {
          if (thumbnail != null)//&& thumbnail.Type == ThumbnailType.Image) // verify the type is ThumbnailType.Image (album art) instead of ThumbnailType.Icon (which may be returned as a fallback if the file does not provide album art)
          {
            //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => 						{
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(thumbnail);
            return bitmapImage;
            //});
          }
        }
      }
      catch (Exception ex) { Debug.WriteLine($"$#~>{ex.Message}"); if (Debugger.IsAttached) Debugger.Break(); /*else throw;*/ }
      return null;
    }


    static async void SearchButton_Click()
    {
      StorageFolder musicFolder = await KnownFolders.GetFolderForUserAsync(NullForCurrentUser, KnownFolderId.VideosLibrary);

      List<string> fileTypeFilter = new List<string>();
      fileTypeFilter.Add("*");

      QueryOptions qOpts = new QueryOptions(CommonFileQuery.OrderBySearchRank, fileTypeFilter);
      //use the user's input to make a query
      qOpts.UserSearchFilter = "*.avi";
      StorageFileQueryResult queryResult = musicFolder.CreateFileQueryWithOptions(qOpts);

      StringBuilder outputText = new StringBuilder();

      //find all files that match the query
      IReadOnlyList<StorageFile> files = await queryResult.GetFilesAsync();
      //output how many files that match the query were found
      if (files.Count == 0)
      {
        outputText.Append("No files found for '" + qOpts.UserSearchFilter + "'");
      }
      else if (files.Count == 1)
      {
        outputText.Append(files.Count + " file found:\n\n");
      }
      else
      {
        outputText.Append(files.Count + " files found:\n\n");
      }

      //output the name of each file that matches the query
      foreach (StorageFile file in files)
      {
        outputText.Append(file.Name + "\n");
      }

      //OutputTextBlock.Text = outputText.ToString();
    }
    static async void GetFilesAndFoldersButton_Click()
    {
      StorageFolder picturesFolder = await KnownFolders.GetFolderForUserAsync(NullForCurrentUser, KnownFolderId.PicturesLibrary);

      IReadOnlyList<StorageFile> fileList = await picturesFolder.GetFilesAsync();
      IReadOnlyList<StorageFolder> folderList = await picturesFolder.GetFoldersAsync();

      var count = fileList.Count + folderList.Count;
      StringBuilder outputText = new StringBuilder(picturesFolder.Name + " (" + count + ")\n\n");

      foreach (StorageFolder folder in folderList)
      {
        outputText.AppendLine("    " + folder.DisplayName + "\\");
      }

      foreach (StorageFile file in fileList)
      {
        outputText.AppendLine("    " + file.Name);
      }

      //OutputTextBlock.Text = outputText.ToString();
    }
    static async Task GroupByHelperAsync(QueryOptions qOpts)
    {
      //OutputPanel.Children.Clear();

      StorageFolder picturesFolder = await KnownFolders.GetFolderForUserAsync(NullForCurrentUser, KnownFolderId.PicturesLibrary);
      StorageFolderQueryResult queryResult = picturesFolder.CreateFolderQueryWithOptions(qOpts);

      IReadOnlyList<StorageFolder> folderList = await queryResult.GetFoldersAsync();
      foreach (StorageFolder folder in folderList)
      {
        IReadOnlyList<StorageFile> fileList = await folder.GetFilesAsync();
        //..OutputPanel.Children.Add(CreateHeaderTextBlock(folder.Name + " (" + fileList.Count + ")"));
        foreach (StorageFile file in fileList)
        {
          Debug.WriteLine((file.Name));
        }
      }
    }

    //<< POC
    internal static async Task RunPoc()
    {
      await copyFromIsoToPics("TestStore_CopyFrom_(binary).txt");
    }

    static async Task copyFromIsoToPics(string fnm) // How to: Read and Write to Files in Isolated Storage   https://msdn.microsoft.com/en-us/library/xf96a1wz(v=vs.110).aspx
    {
      StorageFile srcFile = await createFileInIsoStorage(fnm);

      await CopyIsoFileToLibFolder(srcFile, KnownFolderId.VideosLibrary);
    }
    static async Task<StorageFile> createFileInIsoStorage(string fnm) // https://msdn.microsoft.com/windows/uwp/files/quickstart-reading-and-writing-files
    {
      StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
      StorageFile sampleFile = await storageFolder.CreateFileAsync(fnm, CreationCollisionOption.ReplaceExisting);

      var buffer = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary("What fools these mortals be", Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
      await Windows.Storage.FileIO.WriteBufferAsync(sampleFile, buffer);

      return sampleFile;
    }
    //>>

    public static async Task<ulong> GetRoamingFolderSizeKbFromFiles() //ap: https://github.com/AndrewJByrne/Clearing-Windows-AppData/blob/master/DebugSettings/DebugFlyout.xaml.cs
    {
      ulong total = 0;
      foreach (var file in await Windows.Storage.ApplicationData.Current.RoamingFolder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.DefaultQuery))
      {
        var basicProperties = await file.GetBasicPropertiesAsync();
        total += basicProperties.Size;
      }

      return total;
    }
    public static async Task<string> GetRoamingFilwSizesKb()
    {
      string total = "";
      foreach (var file in await Windows.Storage.ApplicationData.Current.RoamingFolder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.DefaultQuery))
      {
        var basicProperties = await file.GetBasicPropertiesAsync();
        total += $"{basicProperties.Size,12:N0}\t{file.Name,32} *\n";

        //await IsoStorePoc.DeleteIsoFile(ApplicationData.Current.RoamingFolder, file.Name);
      }

      return total;
    }

  }
}
///also see https://github.com/Microsoft/UWPCommunityToolkit/blob/master/Microsoft.Toolkit.Uwp/Helpers/StorageFileHelper.cs
/// or      https://github.com/Microsoft/UWPCommunityToolkit/tree/master/Microsoft.Toolkit.Uwp/Helpers
