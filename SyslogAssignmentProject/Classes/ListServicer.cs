namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// Used to update the values of the list that the live feed represents
  /// and update the table component that displays them.
  /// </summary>
  public class ListServicer
  {
    public List<SyslogMessage> SyslogMessageList { get; private set; }

    public event Action ListChanged;

    public ListServicer()
    {
      SyslogMessageList = new List<SyslogMessage>();
    }

    public void invoke()
    {
      ListChanged?.Invoke();
    }

    /// <summary>
    /// Filters the list of syslogs based on IP and/or severtiy.
    /// </summary>
    /// <param name="ipAddress">The IP address to filter (the string "None" disables the filter).</param>
    /// <param name="severity">The severity to filter (the string "None" disables the filter).</param>
    /// <returns>A list of syslog messages with the applied filters.</returns>
    public List<SyslogMessage> FilterList(string ipAddress, string severity)
    {
      List<SyslogMessage> _filteredListOfMessages = new List<SyslogMessage>();
      foreach(SyslogMessage _message in SyslogMessageList)
      {
        if(ipAddress == "None" && severity == "None")
        {
          _filteredListOfMessages.Add(_message);
          continue;
        }
        else if(_message.SenderIP.Equals(ipAddress) &&
          SeverityToString(Convert.ToInt32(_message.Severity)).Equals(severity))
        {
          _filteredListOfMessages.Add(_message);
          continue;
        }
        else if(SeverityToString(Convert.ToInt32(_message.Severity)).Equals(severity) &&
            ipAddress == "None")
        {
          _filteredListOfMessages.Add(_message);
          continue;
        }
        else if(_message.SenderIP.Equals(ipAddress) && severity == "None")
        {
          _filteredListOfMessages.Add(_message);
        }
      }
      return _filteredListOfMessages;
    }

    /// <summary>
    /// Converts the severity integer to a human-readable string.
    /// </summary>
    /// <param name="severity">The syslog severity number.</param>
    /// <returns>The syslog severity as a readable string.</returns>
    private string SeverityToString(int severity)
    {
      string _severityInString = string.Empty;
      if(severity == 0)
      {
        _severityInString = "Debug";
      }
      else if(severity == 1)
      {
        _severityInString = "Warning";
      }
      else if(severity == 2 || severity == 3)
      {
        _severityInString = "Error";
      }
      else
      {
        _severityInString = "Info";
      }
      return _severityInString;
    }
  }
}
