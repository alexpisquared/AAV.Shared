namespace CI.Standard.Lib.Helpers;

public class Emailer
{
  public static (int exitCode, string errMsg) PrepAndPop(string trgEmail, string subj, string body, string? attachmentFilename = null)
  {
    var report = "";
    try
    {
      var psi = new ProcessStartInfo("OUTLOOK.EXE",
        attachmentFilename == null ?
        $"/c ipm.note /m \"{trgEmail}?v=1&subject={subj}&body={body}\"" :
        $"/c ipm.note /m \"{trgEmail}?v=1&subject={subj}&body={body}\" /a \"{attachmentFilename}\"") //Feb 2020: '?v=1' from https://answers.microsoft.com/en-us/msoffice/forum/all/outlook-command-line-parameters-stopped-working/7abe60b2-be29-4426-bf17-f23e1de9f04b
      {
        UseShellExecute = true, // to pick up the exe from the path!!!
        WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
      };

      using var process = new Process { StartInfo = psi };

      process.Start();
      var tries = 5;

      do
      {
        if (!process.HasExited) process.Refresh();
      } while (!process.WaitForExit(1000) && --tries > 0);

      var exitCode = process.ExitCode;
      process.Close();

      return (exitCode, report);
    }
    catch (Win32Exception ex) { ex.Log(); return (-2, ex.Message); }
    catch (Exception ex) { ex.Log(); return (-1, ex.Message); }
  }
}
