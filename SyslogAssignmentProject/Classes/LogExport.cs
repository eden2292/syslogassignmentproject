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
  public class LogExport
  {
    public static void s_export(string? ipAddress)
    {
      // A dictionary of streamwriters, indexed by IP address string
      Dictionary<string, StreamWriter> streamWriterDict = new Dictionary<string, StreamWriter>();
      // If ipAddress is null, export all messages regardless of radio

      string _formattedDateTime = DateTime.Now.ToString("yyyyMMddTHHmmss");

      foreach(SyslogMessage message in S_LiveFeedMessages.SyslogMessageList)
      {
        if(ipAddress == null || message.SenderIP == ipAddress)
        {
          if(!streamWriterDict.ContainsKey(message.SenderIP))
          {
            string ipAddressFilename = message.SenderIP.Replace(":", "_"); // Windows filenames do not allow for colons
            string logFileName = $"{ipAddressFilename}_{_formattedDateTime}.txt";
            streamWriterDict.Add(message.SenderIP, new StreamWriter(logFileName));
          }

          streamWriterDict[message.SenderIP].WriteLine(message.FullMessage);
        }
      }

      string zipPath = @".\Logs.zip";

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