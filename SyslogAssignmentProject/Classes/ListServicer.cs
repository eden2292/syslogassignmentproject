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

    public event Action IpAndPortUpdate;

    private int _sortOption = 2;

    public ListServicer()
    {
      SyslogMessageList = new List<SyslogMessage>();
    }

    public void UpdateList(SyslogMessage messageToAdd)
    {
      SyslogMessageList.Insert(0, messageToAdd);
      SortList(_sortOption);
      ListChanged?.Invoke();
    }
    public void RefreshList()
    {
      SortList(_sortOption);
      ListChanged?.Invoke();
    }
    public void ClearList()
    {
      SyslogMessageList = new List<SyslogMessage>();
      ListChanged?.Invoke();
    }
    /// <summary>
    /// Sorts list based on criteria in live feed.
    /// </summary>
    /// <param name="option">Which category is going to be sorted and whether it is asc/desc</param>
    public void SortList(int option)
    {
      // 1 DateTime ASC, 2 DateTime DESC.
      // 3 Origin ASC, 4 Origin DESC.
      // 5 Severity ASC, 6 Severity DESC.
      _sortOption = option;
      switch (_sortOption) 
      {
        case 1:
          SyslogMessageList.Sort((_item1, _item2) => _item1.ReceivedDateTime.CompareTo(_item2.ReceivedDateTime)); 
          break;
        case 2:
          SyslogMessageList.Sort((_item1, _item2) => _item2.ReceivedDateTime.CompareTo(_item1.ReceivedDateTime));
          break;
        case 3:
          SyslogMessageList.Sort((_item1, _item2) => string.Compare(_item1.SenderIP, _item2.SenderIP));
          break;
        case 4:
          SyslogMessageList.Sort((_item1, _item2) => string.Compare(_item2.SenderIP, _item1.SenderIP));
          break;
        case 5:
          SyslogMessageList.Sort((_item1, _item2) => Convert.ToInt32(_item1.Severity.ToString()).CompareTo(Convert.ToInt32(_item2.Severity.ToString())));
          break;
        case 6:
          SyslogMessageList.Sort((_item1, _item2) => Convert.ToInt32(_item2.Severity.ToString()).CompareTo(Convert.ToInt32(_item1.Severity.ToString())));
          break;
        default:
          break;
      }
      ListChanged?.Invoke();
    }
    public List<SyslogMessage> FilterList(string ipAddress, string severity)
    {
      List<SyslogMessage> _filteredListOfMessages = new List<SyslogMessage>();
      foreach (SyslogMessage _message in SyslogMessageList)
      {
        if (ipAddress == "None" && severity == "None")
        {
          _filteredListOfMessages.Add(_message);
          continue;
        }
        else if (_message.SenderIP.Equals(ipAddress) &&
          SeverityToString(Convert.ToInt32(_message.Severity)).Equals(severity))
        {
          _filteredListOfMessages.Add(_message);
          continue;
        }
        else if (SeverityToString(Convert.ToInt32(_message.Severity)).Equals(severity) &&
            ipAddress == "None")
        {
          _filteredListOfMessages.Add(_message);
          continue;
        }
        else if (_message.SenderIP.Equals(ipAddress) && severity == "None")
        {
          _filteredListOfMessages.Add(_message);
        }
      }
      return _filteredListOfMessages;
    }
    private string SeverityToString(int severity)
    {
      string _severityInString = string.Empty;
      if (severity == 0)
      {
        _severityInString = "Debug";
      }
      else if (severity == 1)
      {
        _severityInString = "Warning";
      }
      else if (severity == 2 || severity == 3)
      {
        _severityInString = "Error";
      }
      else
      {
        _severityInString = "Info";
      }
      return _severityInString;
    }
    public void UpdateIpAndPort()
    {
      IpAndPortUpdate?.Invoke();
    }
  }
}
