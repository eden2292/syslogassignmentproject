using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Globals;

namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// Class for handling log file export functionality.
  /// </summary>
  public class LogExport
  {
    /// <summary>
    /// Exports syslog messages into .txt files and archives them in a zip folder.
    /// </summary>
    /// <param name="ipAddress">The IP address of the radio whose messages you want to export (set it to null for all radios).</param>
    public static void s_export(string? ipAddress)
    {
      // A dictionary of streamwriters, indexed by IP address string.
      Dictionary<string, StreamWriter> streamWriterDict = new Dictionary<string, StreamWriter>();

      string _formattedDateTime = DateTime.Now.ToString("yyyyMMddTHHmmss");

      foreach(SyslogMessage message in S_LiveFeedMessages.SyslogMessageList)
      {
        if(ipAddress == null || message.SenderIP == ipAddress)
        {
          if(!streamWriterDict.ContainsKey(message.SenderIP))
          {
            // Windows filenames do not allow for colons so we replace these with underscores.
            string ipAddressFilename = message.SenderIP.Replace(":", "_");
            string logFileName = $"{ipAddressFilename}_{_formattedDateTime}.txt";
            streamWriterDict.Add(message.SenderIP, new StreamWriter($@"{S_AppDirectory}\{logFileName}"));
          }

          streamWriterDict[message.SenderIP].WriteLine(message.FullMessage);
        }
      }

      string zipPath = $@"{S_AppDirectory}\Logs.zip";

      foreach (StreamWriter streamWriter in streamWriterDict.Values)
      {
        streamWriter?.Flush();
        streamWriter?.Close();
        string logFileName = (streamWriter.BaseStream as FileStream).Name;
        string fileToAdd = @$"\{logFileName}";

        using(ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Update))
        {
          string fileName = Path.GetFileName(fileToAdd);
          archive.CreateEntryFromFile(logFileName, fileName);
        }
        File.Delete(logFileName);
      }
    }
  }
}