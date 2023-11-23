using SyslogAssignmentProject.Classes;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;


/// <summary>
/// The syslog message with a parser to get its information.
/// </summary>
public class SyslogMessage
{
  // The priority number which we can derive the facility and severity from.
  public uint Priority { get; private set; }
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
      string _hexColour;
      if(Severity == 6)
      {
        _hexColour = _injectedGlobals.CurrentDebugColour;
      }
      else if(Severity == 4)
      {
        _hexColour = _injectedGlobals.CurrentWarningColour;
      }
      else if(Severity == 3)
      {
        _hexColour = _injectedGlobals.CurrentErrorColour;
      }
      else if(Severity == 7)
      {
        _hexColour = _injectedGlobals.CurrentInfoColour;
      }
      else
      {
        _hexColour = "#A9A9A9";
      }
      return _hexColour;
    }
  }

  public string ReceivingIP { get; set; }

  private GlobalInjection _injectedGlobals;
  /// <summary>
  /// Receives parsed message from listeners so that it can be converted into a syslog message.
  /// </summary>
  /// <param name="injectedGlobals">Singleton used to access global colour values for each severity type.</param>
  /// <param name="receivingIP">IP address that message was transmitted to.</param>
  /// <param name="receivingPortNumber">Port number that message was transmitted to.</param>
  /// <param name="senderIP">IP address that message was transmitted from.</param>
  /// <param name="senderPortNumber">Port number that message was transmitted from.</param>
  /// <param name="receivedDateTime">The time that the message was received by the listener.</param>
  /// <param name="fullMessage">The full parsed message.</param>
  /// <param name="protocolType">Which transport protocol the message arrived using.</param>
  public SyslogMessage(GlobalInjection injectedGlobals, string receivingIP, int receivingPortNumber,
    string senderIP, int senderPortNumber, DateTimeOffset receivedDateTime, string fullMessage, string protocolType)
  {
    ReceivingIP = receivingIP;
    ReceivingPortNumber = receivingPortNumber;
    SenderIP = senderIP;
    SenderPortNumber = senderPortNumber;
    ReceivedDateTime = receivedDateTime;
    FullMessage = fullMessage;
    ProtocolType = protocolType;
    _injectedGlobals = injectedGlobals;
  }
  /// <summary>
  /// Formats IP address and port number based on whether it is IPv4/6.
  /// </summary>
  /// <param name="ipAddress">IP address to format.</param>
  /// <param name="portNumber">Port number to format.</param>
  /// <returns>Returns formatted IP address with port number based on if it is IPv4/6.</returns>
  public string FormatIp(string ipAddress, int portNumber)
  {
    string _formattedIp;
    if (IPAddress.Parse(ipAddress).AddressFamily == AddressFamily.InterNetwork)
    {
      _formattedIp = $"{ipAddress} : {portNumber}";
    }
    else if (IPAddress.Parse(ReceivingIP).AddressFamily == AddressFamily.InterNetworkV6)
    {
      _formattedIp = $"[{ipAddress}] : {portNumber}";
    }
    else
    {
      _formattedIp = "Unknown";
    }
    return _formattedIp;
  }
  public int ReceivingPortNumber { get; set; }
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


    if (priorityRegex.Matches(FullMessage).Count > 0)
    {
      string priorityString = priorityRegex.Matches(FullMessage)[0].Groups[1].Value;
      uint priorityResult = 0;
      // If the priority in the message is blank then it is set to 0.
      //
      // Uint.TryParse attempts to parse the priority into an unsigned integer. If it fails then it returns false, but if it succeeds
      // it casts the output of the parse into the variable of its second argument. In this case, priorityResult.
      if (priorityString == "" || uint.TryParse(priorityString, out priorityResult))
      {
        Priority = priorityResult;
        messageParsedFailures &= ~ParseFailure.Priority;

        if (sentDateTimeRegex.Matches(FullMessage).Count > 0)
        {
          string sentDateTimeString = sentDateTimeRegex.Matches(FullMessage)[0].Groups[1].Value;
          DateTimeOffset sentDateTimeResult = new DateTimeOffset();

          // Similar to byte.TryParse but for date-time, using ISO 8601 as its standard.
          if (DateTimeOffset.TryParseExact(sentDateTimeString, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.None, out sentDateTimeResult))
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

