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
    private static StreamWriter s_fileWriter;
    private static int s_filescounter = 0;
    public static void s_export(string? ipAddress)
    {
      // If ipAddress is null, export all messages regardless of radio
      string _ipAddressFilename = "anyIP";
      string _formattedDateTime = DateTime.Now.ToString("yyyyMMddTHHmmss");
      if (ipAddress != null)
        _ipAddressFilename = ipAddress.Replace(":", "_"); // Windows filenames do not allow for colons

      string _logFileName = $"{_ipAddressFilename}_{_formattedDateTime}_{s_filescounter}.txt";

      s_fileWriter = new StreamWriter(_logFileName, true);

      foreach(SyslogMessage message in S_LiveFeedMessages.SyslogMessageList)
      {
        if (ipAddress == null || message.SenderIP == ipAddress)
        {
          s_fileWriter.WriteLine(message.FullMessage);
        }
      }

      s_fileWriter?.Flush();
      s_fileWriter?.Close();
      string zipPath = @".\Logs.zip";
      string fileToAdd = @$"\{_logFileName}";

      using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Update))
      {
        string fileName = Path.GetFileName(fileToAdd);
        archive.CreateEntryFromFile(_logFileName, fileName);
      }
      File.Delete(_logFileName);
    }
  }
}