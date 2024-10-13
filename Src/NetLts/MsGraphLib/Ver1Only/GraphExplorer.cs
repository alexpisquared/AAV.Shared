using Microsoft.Graph.Models;
using Pastel;
using System.Diagnostics;
using System.Drawing;
using static System.Console;

namespace MsGraphLibVer1.Ver1Only;

public class GraphExplorer // https://developer.microsoft.com/en-us/graph/graph-explorer:
{
  public static async Task ExploreGraph_RecentItems(MyGraphDriveServiceClient msGraphLibVer1)
  {
    var graphClient = msGraphLibVer1.DriveClient;
    var recentItems = await graphClient.Drives[msGraphLibVer1.Drive.Id].Recent.GetAsRecentGetResponseAsync();

    WriteLine("  * Recents:".Pastel(Color.Beige));
    WriteLine($"  Recent OneDrive Items {recentItems?.Value?.Count}:".Pastel(Color.Cyan));
    for (var i = 0; i < recentItems?.Value?.Count / 20; i++)
    {
      var item = recentItems?.Value[i];
      WriteLine($"  {i + 1,3}. {item?.Name}".Pastel(Color.Cyan));
    }
  }
  public static async Task ExploreGraph_Calendar_List(MyGraphDriveServiceClient msGraphLibVer1, DateTime start, DateTime end)
  {
    try
    {
      var graphClient = msGraphLibVer1.DriveClient;
      var todayEvents = await graphClient.Me.CalendarView.GetAsync((requestConfiguration) =>
      {
        requestConfiguration.QueryParameters.StartDateTime = $"{start:yyyy-MM-ddT00:00:00.000Z}";
        requestConfiguration.QueryParameters.EndDateTime = $"{end:yyyy-MM-ddT00:00:00.000Z}";
      });

      WriteLine($" * Calendar {todayEvents?.Value?.Count} events for {start} - {end}:".Pastel(Color.Beige));
      foreach (var item in todayEvents?.Value)
        WriteLine($" ■ {DateTime.Parse(item.Start?.DateTime).ToLocalTime():ddd HH:mm}  {item.Subject}".Pastel(Color.Cyan));
    }
    catch (Exception ex)
    {
      WriteLine($"▒▒ Error: {ex.Message}".Pastel(Color.FromArgb(255, 160, 0)));
      Trace.WriteLine($"▒▒ Error: {ex.Message}");
      Beep(5000, 750);
    }
  }
  public static async Task ExploreGraph_Calendar_Post(MyGraphDriveServiceClient msGraphLibVer1, DateTime start0, DateTime end0, string[] attendeeEmails, string facility = "North Thornhill Community Centre")
  {
    try
    {
      var start = $"{start0.ToUniversalTime():yyyy-MM-ddTHH:mm:ss.fffZ}";
      var finish = $"{end0.ToUniversalTime():yyyy-MM-ddTHH:mm:ss.fffZ}";

      var requestBody = new Event
      {
        Subject = $"Swim at {facility}",
        Start = new DateTimeTimeZone { DateTime = start, TimeZone = "UTC", },
        End = new DateTimeTimeZone { DateTime = finish, TimeZone = "UTC", },
        Attendees = [],
        Location = new Location
        {
          DisplayName =
            facility.Contains("Outdoor") ? "31 Old Yonge St, Vaughan, ON L4J 9B3" :
            facility.Contains("North Thornhill Community Centre") ? "300 Pleasant Ridge Ave, Vaughan, ON L4J 9B3" : facility
        },
      };

      attendeeEmails.ToList().ForEach(email => requestBody.Attendees.Add(new() { EmailAddress = new EmailAddress { Address = email } })); // foreach (var email in attendeeEmails)      {        requestBody.Attendees.Add(new() { EmailAddress = new EmailAddress { Address = email } });      }

      var bookedEvent = await msGraphLibVer1.DriveClient.Me.Events.PostAsync(requestBody);

      WriteLine($" * Calendar:".Pastel(Color.DarkCyan));
      WriteLine($" ■ {bookedEvent?.Start?.DateTime} {bookedEvent?.Subject}".Pastel(Color.Cyan));
    }
    catch (Exception ex)
    {
      WriteLine($"▒▒ Error: {ex.Message}".Pastel(Color.FromArgb(255, 160, 0)));
      Trace.WriteLine($"▒▒ Error: {ex.Message}");
      Beep(5000, 750);
    }
  }
}