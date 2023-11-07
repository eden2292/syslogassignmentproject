using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

public class SyslogMessage
{
  public byte Priority { get; private set; } // The priority number which we can derive the facility and severity from
                                             // Note: We could use enums for Facility and Severity for easier readability
  public byte Facility
  {
    get
    {
      return (byte)(Priority / 8);
    }
  }
  public byte Severity
  {
    get
    {
      return (byte)(Priority % 8);
    }
  }
  public string SenderIP { get; set; }
  public string ProtocolType { get; set; }
  public DateTimeOffset? SentDateTime { get; private set; } // The date/time in the syslog message itself, can be null if the format in the syslog message fails to parse
  public DateTimeOffset ReceivedDateTime { get; set; } // The date/time when the message was received, using .NET DateTime.Now when the remote store gets the message
  public string? EndMessage { get; private set; }
  public string FullMessage { get; set; } // The full syslog message
  public SyslogMessage(string senderIp, DateTimeOffset receivedDateTime, string fullMessage, string protocolType)
  {
    SenderIP = senderIp;
    ReceivedDateTime = receivedDateTime;
    FullMessage = fullMessage;
    ProtocolType = protocolType;
  }


  public enum ParseFailure
  {
    Priority = 4,
    SentDateTime = 2,
    EndMessage = 1
  }

  /// <summary>
  /// Parses Syslog message strings and extracts the fields needed for a SyslogMessage object
  /// </summary>
  /// <returns>Enumerable relating to the success level of the parse</returns>
  public ParseFailure ParseMessage()
  {
    SentDateTime = null;
    EndMessage = null;

    ParseFailure messageParsedFailures = ParseFailure.Priority | ParseFailure.SentDateTime | ParseFailure.EndMessage;
    // Bitwise return values. 0 = success, 1 = failure.
    // From left-to-right is the success bit of priority, sent date-time, and end message respectively.
    // This is to handle any syslog messages that could be in unexpected formats.

    // At the moment we do not parse the hostname and process so we do not have regexes for those.
    Regex priorityRegex = new Regex(@"^<(\d{0,3})>\d+");
    Regex sentDateTimeRegex = new Regex(@"^<\d{0,3}>\d+ ([\dTZ:.-]+)");
    Regex endMessageRegex = new Regex(@"^<\d{0,3}>\d+ .+ - - - - ([\dA-Z]+)$");


    if(priorityRegex.Matches(FullMessage).Count > 0)
    {
      string priorityString = priorityRegex.Matches(FullMessage)[0].Groups[1].Value;
      byte priorityResult = 0;
      if(priorityString == "" || byte.TryParse(priorityString, out priorityResult))
      {
        Priority = priorityResult;
        messageParsedFailures &= ~ParseFailure.Priority;

        if (sentDateTimeRegex.Matches(FullMessage).Count > 0)
        {
          string sentDateTimeString = sentDateTimeRegex.Matches(FullMessage)[0].Groups[1].Value;
          DateTimeOffset sentDateTimeResult = new DateTimeOffset();
          if(DateTimeOffset.TryParseExact(sentDateTimeString, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.None, out sentDateTimeResult))
          {
            SentDateTime = sentDateTimeResult;
            messageParsedFailures &= ~ParseFailure.SentDateTime;
          }
        }

        if (endMessageRegex.Matches(FullMessage).Count > 0)
        {
          EndMessage = endMessageRegex.Matches(FullMessage)[0].Groups[1].Value;
          messageParsedFailures &= ~ParseFailure.EndMessage;
        }
      }
    }

    return messageParsedFailures;
  }
}

