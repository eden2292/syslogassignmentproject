using System.IO.Compression;

namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// Class for handling log file export functionality.
  /// </summary>
  public class LogExport
  {
    private ListServicer _liveFeedMessages = new ListServicer();
    private string _appDirectory;
    /// <summary>
    /// Creates a new instance of a log class that sets the destination folder as the parsed directory.
    /// </summary>
    /// <param name="liveFeedMessages">Set of messages to be logged.</param>
    /// <param name="appDirectory">The directory of the application.</param>
    public LogExport(ListServicer liveFeedMessages, string appDirectory)
    {
      _appDirectory = appDirectory;
      _liveFeedMessages = liveFeedMessages;

    }
    /// <summary>
    /// Exports syslog messages into .txt files and archives them in a zip folder.
    /// </summary>
    /// <param name="ipAddress">The IP address of the radio whose messages you want to export (set it to null for all radios).</param>
    public void Export(string? ipAddress)
    {
      // A dictionary of streamwriters, indexed by IP address string.
      Dictionary<string, StreamWriter> _streamWriterDict = new Dictionary<string, StreamWriter>();

      string _formattedDateTime = DateTime.Now.ToString("yyyyMMddTHHmmss");

      foreach (SyslogMessage _message in _liveFeedMessages.SyslogMessageList)
      {
        if (ipAddress == null || _message.SenderIP == ipAddress)
        {
          if (!_streamWriterDict.ContainsKey(_message.SenderIP))
          {
            // Windows filenames do not allow for colons so we replace these with underscores.
            string _ipAddressFilename = _message.SenderIP.Replace(":", "_");
            string _logFileName = $"{_ipAddressFilename}_{_formattedDateTime}.txt";
            _streamWriterDict.Add(_message.SenderIP, new StreamWriter($@"{_appDirectory}\{_logFileName}"));
          }

          _streamWriterDict[_message.SenderIP].WriteLine(_message.FullMessage);
        }
      }

      string _zipPath = $@"{_appDirectory}\Logs.zip"; 

      foreach (StreamWriter _streamWriter in _streamWriterDict.Values)
      {
        _streamWriter?.Flush();
        _streamWriter?.Close();
        string _logFileName = (_streamWriter.BaseStream as FileStream).Name;
        string _fileToAdd = @$"\{_logFileName}";

        using (ZipArchive _archive = ZipFile.Open(_zipPath, ZipArchiveMode.Update))
        {
          string _fileName = Path.GetFileName(_fileToAdd);
          _archive.CreateEntryFromFile(_logFileName, _fileName);
        }
        File.Delete(_logFileName);
      }
    }
  }
}