﻿using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using static Globals;

/// <summary>
/// The syslog message with a parser to get its information.
/// </summary>
public class SyslogMessage
{
  // The priority number which we can derive the facility and severity from.
  public uint Priority { get; private set; }
  public uint Facility
  {
    get
    {
      return (Priority / 8);
    }
  }
  public uint Severity
  {
    // 0 = Debug
    // 1 = Warning
    // 2-3 = Error
    // 4-7 = Info
    get
    {
      return (Priority % 8);
    }
  }
  public string HexColour
  {
    get
    {
      string _hexCode = S_CurrentInfoColour;
      switch (Convert.ToInt32(Severity))
      {
        case 0:
          _hexCode = S_CurrentDebugColour;
          break;
        case 1:
          _hexCode = S_CurrentWarningColour;
          break;
        case (2 or 3):
          _hexCode = S_CurrentErrorColour;
          break;
        default:
          break;
      }
      return _hexCode;
    }
        set { HexColour = value; }
  }
  public string ReceivingIP { get; set; }
  public int ReceivingPort { get; set; }
  public string SenderIP { get; set; }
  public int SenderPortNumber { get; set; }
  public string ProtocolType { get; set; }
  // The date/time in the syslog message itself, can be null if the format in the syslog message fails to parse.
  public DateTimeOffset? SentDateTime { get; private set; }
  // The date/time when the message was received, using .NET DateTime.Now when the remote store gets the message.
  public DateTimeOffset ReceivedDateTime { get; set; }
  public string? EndMessage { get; private set; }
  // The full syslog message.
  public string FullMessage { get; set; }
  public SyslogMessage(string senderIp, int portNumber, DateTimeOffset receivedDateTime, string fullMessage, string protocolType)
  {
    ReceivingIP = S_ReceivingIpAddress;
    ReceivingPort = S_ReceivingPortNumber;
    SenderIP = senderIp;
    SenderPortNumber = portNumber;
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
  /// Parses Syslog message strings and extracts the fields needed for a SyslogMessage object.
  /// </summary>
  /// <returns>Enumerable relating to the success level of the parse.</returns>
  public ParseFailure ParseMessage()
  {
    SentDateTime = null;
    EndMessage = null;

    // Bitwise return values. 0 = success, 1 = failure.
    // From left-to-right is the success bit of priority, sent date-time, and end message respectively.
    // This is to handle any syslog messages that could be in unexpected formats.
    ParseFailure messageParsedFailures = ParseFailure.Priority | ParseFailure.SentDateTime | ParseFailure.EndMessage;

    // At the moment we do not parse the hostname and process so we do not have regexes for those.
    Regex priorityRegex = new Regex(@"^<(\d{0,3})>\d+");
    Regex sentDateTimeRegex = new Regex(@"^<\d{0,3}>\d+ ([\dTZ:.-]+)");
    Regex endMessageRegex = new Regex(@"^<\d{0,3}>\d+ .+ - - - - ([\dA-Z]+)$");


    if(priorityRegex.Matches(FullMessage).Count > 0)
    {
      string priorityString = priorityRegex.Matches(FullMessage)[0].Groups[1].Value;
      uint priorityResult = 0;
      // If the priority in the message is blank then it is set to 0.
      //
      // Uint.TryParse attempts to parse the priority into an unsigned integer. If it fails then it returns false, but if it succeeds
      // it casts the output of the parse into the variable of its second argument. In this case, priorityResult.
      if(priorityString == "" || uint.TryParse(priorityString, out priorityResult))
      {
        Priority = priorityResult;
        messageParsedFailures &= ~ParseFailure.Priority;

        if(sentDateTimeRegex.Matches(FullMessage).Count > 0)
        {
          string sentDateTimeString = sentDateTimeRegex.Matches(FullMessage)[0].Groups[1].Value;
          DateTimeOffset sentDateTimeResult = new DateTimeOffset();

          // Similar to byte.TryParse but for date-time, using ISO 8601 as its standard.
          if(DateTimeOffset.TryParseExact(sentDateTimeString, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.None, out sentDateTimeResult))
          {
            SentDateTime = sentDateTimeResult;
            messageParsedFailures &= ~ParseFailure.SentDateTime;
          }
        }

        if(endMessageRegex.Matches(FullMessage).Count > 0)
        {
          EndMessage = endMessageRegex.Matches(FullMessage)[0].Groups[1].Value;
          messageParsedFailures &= ~ParseFailure.EndMessage;
        }
      }
    }

    return messageParsedFailures;
  }
}

