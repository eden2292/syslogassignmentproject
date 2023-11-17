using System.IO.Compression;

namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// Class for handling log file export functionality.
  /// </summary>
  public class LogExport
  {
    private ListServicer S_LiveFeedMessages = new ListServicer();
    private string s_AppDirectory;
    public LogExport(ListServicer liveFeedMessages, string appDirectory)
    {
      s_AppDirectory = appDirectory;
      S_LiveFeedMessages = liveFeedMessages;

    }
    /// <summary>
    /// Exports syslog messages into .txt files and archives them in a zip folder.
    /// </summary>
    /// <param name="ipAddress">The IP address of the radio whose messages you want to export (set it to null for all radios).</param>
    public void s_export(string? ipAddress)
    {
      // A dictionary of streamwriters, indexed by IP address string.
      Dictionary<string, StreamWriter> streamWriterDict = new Dictionary<string, StreamWriter>();

      string _formattedDateTime = DateTime.Now.ToString("yyyyMMddTHHmmss");

      foreach (SyslogMessage message in S_LiveFeedMessages.SyslogMessageList)
      {
        if (ipAddress == null || message.SenderIP == ipAddress)
        {
          if (!streamWriterDict.ContainsKey(message.SenderIP))
          {
            // Windows filenames do not allow for colons so we replace these with underscores.
            string ipAddressFilename = message.SenderIP.Replace(":", "_");
            string logFileName = $"{ipAddressFilename}_{_formattedDateTime}.txt";
            streamWriterDict.Add(message.SenderIP, new StreamWriter($@"{s_AppDirectory}\{logFileName}"));
          }

          streamWriterDict[message.SenderIP].WriteLine(message.FullMessage);
        }
      }

      string zipPath = $@"{s_AppDirectory}\Logs.zip"; //Change this to save to root in its own folder <3 It will be fucky when we do it on their machines. 

      foreach (StreamWriter streamWriter in streamWriterDict.Values)
      {
        streamWriter?.Flush();
        streamWriter?.Close();
        string logFileName = (streamWriter.BaseStream as FileStream).Name;
        string fileToAdd = @$"\{logFileName}";

        using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Update))
        {
          string fileName = Path.GetFileName(fileToAdd);
          archive.CreateEntryFromFile(logFileName, fileName);
        }
        File.Delete(logFileName);
      }
    }
  }
}