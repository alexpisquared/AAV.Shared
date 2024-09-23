using Microsoft.Graph.Beta;
using Microsoft.Graph.Beta.Models;
using MSGraphGetPhotoToTheLatestVersionPOC;
using Pastel;
using System.Diagnostics;
using System.Drawing;
using static System.Console;

namespace MsGraphLib;

public class MyGraphDriveServiceClient(string clientId) : MyGraphServiceClient
{
  Drive? _drive; public Drive Drive => _drive ?? throw new InvalidOperationException("■ Drive not initialized");
  public GraphServiceClient DriveClient => _graphServiceClient ?? throw new InvalidOperationException("■ GraphServiceClient not initialized");

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
  }
  static void RealTimeStreamOutputting(Stream stream)
  {
    string? line;
    using var reader = new StreamReader(stream);
    while ((line = reader?.ReadLine()) != null)
      Write(line.Pastel(Color.Green));      //Beep(8000, 11);
  }

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
      var rawStream = await _graphServiceClient.Drives[_drive.Id].Items[driveItem.Id].ContentStream.GetAsync() ?? throw new FileNotFoundException($"File found {file} .. but getting content for {driveItem.Id} failed?!?!?!");

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