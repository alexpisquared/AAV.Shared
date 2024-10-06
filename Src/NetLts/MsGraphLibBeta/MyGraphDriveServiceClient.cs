using Microsoft.Graph.Beta.Models;
using MsGraphLib____;
using MsGraphLibContract;
using System.Diagnostics;

namespace MsGraphLibBeta;

public class MyGraphDriveServiceClient(string clientId) : MyGraphServiceClient, IMyGraphDriveServiceClient
{
  Drive? _drive; public Drive Drive
  {
    get
    {
      _drive = _graphServiceClient?.Me.Drive.GetAsync().Result;

      return _drive ?? throw new InvalidOperationException("■ Drive not initialized");
    }
  }

  public GraphServiceClient DriveClient
  {
    get
    {
      var (success, report, _) = InitializeGraphClientIfNeeded(clientId).Result;
      WriteLine(report.Pastel(Color.FromArgb(success ? 128 : 255, success ? 196 : 160, 0)));
      Trace.WriteLine(report.Pastel(Color.FromArgb(128, 160, 0)));

      return _graphServiceClient ?? throw new InvalidOperationException("■ GraphServiceClient not initialized");
    }
  }

  public async Task TestStreamDirect(string filePath)
  {
    using var stream = await GetGraphFileStream(filePath);

    RealTimeStreamOutputting(stream);
  }
  public async Task TestMemoryStream(string filePath)
  {
    using var stream = await GetGraphFileStream(filePath);

    var memoryStream = new MemoryStream();
    await stream.CopyToAsync(memoryStream);
    _ = memoryStream.Seek(0, SeekOrigin.Begin); //tu: JPG images fix!!!
    stream.Close();

    string? line;
    using var reader = new StreamReader(memoryStream);
    while ((line = reader?.ReadLine()) != null)
      Write(line.Pastel(Color.Magenta));
    Write("\n");
  }
  static void RealTimeStreamOutputting(Stream stream)
  {
    string? line;
    using var reader = new StreamReader(stream);
    while ((line = reader?.ReadLine()) != null)
      Write(line.Pastel(Color.Green));      //Beep(8000, 11);
    Write("\n");
  }

  [Obsolete]
  public async Task<Stream> GetGraphFileStream(string file)
  {
    try
    {
      var (success, report, authResult) = await InitializeGraphClientIfNeeded(clientId);
      WriteLine(report.Pastel(Color.FromArgb(128, 160, 0)));
      Trace.WriteLine(report);

      ArgumentNullException.ThrowIfNull(_graphServiceClient, nameof(_graphServiceClient));

      _drive = await _graphServiceClient.Me.Drive.GetAsync();
      ArgumentNullException.ThrowIfNull(_drive, nameof(_drive));

      var driveItem = await _graphServiceClient.Drives[_drive.Id].Root.ItemWithPath(file).GetAsync() ?? throw new FileNotFoundException($"File not found: {file}");
      var rawStream = await _graphServiceClient.Drives[_drive.Id].Items[driveItem.Id].Content.GetAsync() ?? throw new FileNotFoundException($"File found {file} .. but getting content for {driveItem.Id} failed?!?!?!");

      ///todo: ContentStream:
      /// https://learn.microsoft.com/en-us/graph/api/driveitem-get-contentstream?view=graph-rest-beta&tabs=http
      /// Download a partial range of bytes --- looks like it is much better ... at least promising.
      /// ==> wait for Beta to take over and hope it fixes streaming of videos 
      /// Microsoft.Graph.Beta" Version="5.81.0-preview"  does not work.
      //var rawStrea3 = await _graphServiceClient.Drives[_drive.Id].Items[driveItem.Id].ContentStream.GetAsync() ?? throw new FileNotFoundException($"File found {file} .. but getting content for {driveItem.Id} failed?!?!?!"); // Download the contents of the primary stream (file) of a driveItem. Only driveItem objects with the file property can be downloaded.

      return rawStream;
    }
    catch (Exception ex)
    {
      WriteLine($"▒▒ Error: {ex.Message}".Pastel(Color.FromArgb(255, 160, 0)));
      Trace.WriteLine($"▒▒ Error: {ex.Message}");
      Beep(5000, 750);
      throw;
    }
  }
}

//todo: manage apps:  https://account.live.com/consent/Manage?uaid=80c30464e3c748239e49630886b523d4&fn=email&guat=1
//todo: manage apps:  https://account.live.com/consent/Manage?uaid=80c30464e3c748239e49630886b523d4&fn=email&guat=1