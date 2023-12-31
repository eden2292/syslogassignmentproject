﻿namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// Used to update the values of the list that the live feed represents
  /// and update the table component that displays them.
  /// </summary>
  public class ListServicer
  {
    public List<SyslogMessage> SyslogMessageList { get; private set; }

    public event Action ListChanged;
    /// <summary>
    /// Creates a new global list that can be accessed due to it being a singleton.
    /// </summary>
    public ListServicer()
    {
      SyslogMessageList = new List<SyslogMessage>();
    }
    /// <summary>
    /// Invokes event to update components with new version of the syslog message list.
    /// </summary>
    public void RefreshList()
    {
      ListChanged?.Invoke();
    }
    /// <summary>
    /// Returns whether the parsed syslog message should be displayed on the live feed according to the active filters.
    /// </summary>
    /// <param name="element">Syslog message that is being accepted/rejected.</param>
    /// <param name="selectedIp">IP address that the user wishes to filter by.</param>
    /// <param name="selectedSeverity">Severity the user wishes to filter by.</param>
    /// <returns>Returns whether the syslog message should be part of the displayed messages with the given filters.</returns>
    public static bool FilterFunction(SyslogMessage element, string selectedIp, string selectedSeverity)
    {
      bool _ipCondition = false;
      bool _severityCondition = false;
      if (string.IsNullOrEmpty(selectedIp))
      {
        _ipCondition = true;
      }
      else if (selectedIp.Equals(element.SenderIP))
      {
        _ipCondition = true;
      }
      if (string.IsNullOrEmpty(selectedSeverity))
      {
        _severityCondition = true;
      }
      else if (selectedSeverity.Equals(SeverityNumberToText(Convert.ToInt32(element.Severity))))
      {
        _severityCondition = true;
      }
      return _ipCondition && _severityCondition;
    }
    public static List<SyslogMessage> FilterListRetriever(string selectedIp, string selectedSeverity, List<SyslogMessage> allMessages)
    {
      List<SyslogMessage> _listOfFilteredMessages = new List<SyslogMessage>();
      foreach(SyslogMessage _msg in allMessages)
      {
        if(FilterFunction(_msg, selectedIp, selectedSeverity))
        {
          _listOfFilteredMessages.Add(_msg);
        }
      }
      return _listOfFilteredMessages;
    }
    /// <summary>
    /// Takes the severity number and outputs its equivalent in words.
    /// </summary>
    /// <param name="severity">Severity number from 0-7.</param>
    /// <returns>Returns the string type of severity that the entered number corresponds to.</returns>
    private static string SeverityNumberToText(int severity)
    {
      string _severity;
      switch (severity)
      {
        case 7:
          _severity = "Debug";
          break;
        case 4:
          _severity = "Warning";
          break;
        case 3:
          _severity = "Error";
          break;
        default:
          _severity = "Info";
          break;
      }
      return _severity;
    }
  }
}
