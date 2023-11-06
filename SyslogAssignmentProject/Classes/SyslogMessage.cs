using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

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


  /// <summary>
  /// Parses Syslog message strings and extracts the fields needed for a SyslogMessage object
  /// </summary>
  /// <returns>Byte codes relating to the success level of the parse</returns>
  public byte ParseMessage()
  {
    string fullMessage = FullMessage;
    byte messageParsedSuccessfully = 4;
    // 0 = Everything parsed successfully including additional fields
    // 1 = Only date-time and priority parsed successfully
    // 2 = Only the message at the end and priority parsed successfully
    // 3 = Date-time and message at the end were unsuccessful to parse, but priority parsed successfully
    // 4 = Nothing parsed successfully
    // If the priority level cannot be parsed then we return 4 always

    if(fullMessage[0] == '<')
      messageParsedSuccessfully = ParseEndArrowPosition(fullMessage);

    return messageParsedSuccessfully;
  }

  /// <summary>
  /// (Part of ParseMessage) Finds where the end arrow position of the priority is
  /// </summary>
  /// <param name="fullMessage">The full syslog message</param>
  /// <returns>Boolean for if the parse is successful</returns>
  private byte ParseEndArrowPosition(string fullMessage)
  {
    byte messageParsedSuccessfully = 4;

    for(int endArrowPosition = 1; ; endArrowPosition++)
    {
      if(endArrowPosition == 5) // Facility's numerical code can only be 3 digits at most so return false if there's more
        break;

      if(fullMessage[endArrowPosition] == '>')
        messageParsedSuccessfully = ParsePriority(fullMessage, endArrowPosition);

      if(!Char.IsDigit(fullMessage[endArrowPosition]))
        break;

    }

    return messageParsedSuccessfully;
  }

  /// <summary>
  /// (Part of ParseMessage) Finds what the priority of the syslog message is, sets the Priority field of the object accordingly
  /// </summary>
  /// <param name="fullMessage">The full syslog message</param>
  /// <param name="endArrowPosition">The position of the end arrow of the priority</param>
  /// <returns>Boolean for if the parse is successful</returns>
  private byte ParsePriority(string fullMessage, int endArrowPosition)
  {
    byte messageParsedSuccessfully = 4;

    byte priority = 0;
    string priorityString = fullMessage.Substring(1, (endArrowPosition - 1));
    if(priorityString.Length > 0)
      priority = byte.Parse(priorityString);
    else
      priority = 0;

    if(priority < 192)
    {
      // The program will try to parse the date and time in the syslog message, and if it is unsuccessful, it will NOT affect the returned boolean of the ParseMessage() method
      Priority = priority;
      messageParsedSuccessfully = ParseSyslogVersion(fullMessage, endArrowPosition);
    }
    else
      messageParsedSuccessfully = 4;

    return messageParsedSuccessfully;
  }

  /// <summary>
  /// (Part of ParseMessage) Finds what the version of the syslog message is (unneeded, this is just so we can get to the datetime)
  /// </summary>
  /// <param name="fullMessage">The full syslog message</param>
  /// <param name="endArrowPosition">The position of the end arrow of the priority</param>
  /// <returns>Boolean for if the parse is successful</returns>
  private byte ParseSyslogVersion(string fullMessage, int endArrowPosition)
  {
    byte additionalFieldsParsedSuccessfully = 0;
    // 0 = All additional fields parsed successfully
    // 1 = Only date and time parsed successfully
    // 2 = Only the message at the end parsed successfully
    // 3 = Neither date and time nor message at the end were parsed successfully
    // Some arithmetic and logic is used for setting this value, based on the return values of the date-time parser and end message parser

    for(int versionSpacePosition = endArrowPosition + 1; ; versionSpacePosition++)
    {
      if(versionSpacePosition == endArrowPosition + 3)
        break;

      if(fullMessage[versionSpacePosition] == ' ')
      {
        string dateTimeString = fullMessage.Substring(versionSpacePosition + 1);

        // Get date and time
        if(!ParseDateTime(dateTimeString))
        {
          additionalFieldsParsedSuccessfully += 2; // We know the value cannot be 0 or 1 because those are reserved for the date/time parsing successfully
          SentDateTime = null;
        }

        // Get the message at the end
        if(!ParseMessageAtEnd(fullMessage))
        {
          additionalFieldsParsedSuccessfully += 1; // We know the value cannot be 0 or 2 because those are reserved for the message parsing successfully
          EndMessage = null;
        }
      }

      if(!Char.IsDigit(fullMessage[versionSpacePosition]))
        break;
    }

    return additionalFieldsParsedSuccessfully;
  }

  /// <summary>
  /// (Part of ParseMessage) Finds what the date and time of the syslog message is, sets the SentDateTime field of the object accordingly
  /// </summary>
  /// <param name="syslogMessageFromDateTime">The syslog message without the priority or version</param>
  /// <returns>Boolean for if the parse is successful</returns>
  private bool ParseDateTime(string syslogMessageFromDateTime)
  {
    bool dateTimeParsedSuccessfully = false;

    string dateTimeString;
    for(int dateTimeSpacePosition = 0; ; dateTimeSpacePosition++)
    {
      if(syslogMessageFromDateTime[dateTimeSpacePosition] == ' ')
      {
        dateTimeString = syslogMessageFromDateTime.Substring(0, dateTimeSpacePosition);
        break;
      }
    }

    DateTimeOffset sentDateTime;
    dateTimeParsedSuccessfully = DateTimeOffset.TryParseExact(dateTimeString, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.None, out sentDateTime);
    SentDateTime = sentDateTime;

    return dateTimeParsedSuccessfully;
  }

  /// <summary>
  /// (Part of ParseMessage) Finds what the syslog message at the end is, sets the EndMessage field of the object accordingly
  /// </summary>
  /// <param name="syslogMessage">The full syslog message</param>
  /// <returns>Boolean for if the parse is successful</returns>
  private bool ParseMessageAtEnd(string syslogMessage)
  {
    bool messageParsedSuccessfully = false;
    int characterPositionOfMessageStart = syslogMessage.IndexOf(" - - - - ");
    if (characterPositionOfMessageStart != -1)
    {
      EndMessage = syslogMessage.Substring(characterPositionOfMessageStart + 9);
      messageParsedSuccessfully = true;
    }
    return messageParsedSuccessfully;
  }
}

