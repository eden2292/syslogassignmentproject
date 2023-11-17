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

    public void RefreshList()
    {
      ListChanged?.Invoke();
    }

    public static bool FilterFunction(SyslogMessage element, string selectedIp, string selectedSeverity)
    {
      bool _ipCondition = false;
      bool _severityCondition = false;
      if (string.IsNullOrEmpty(selectedIp))
      {
        _ipCondition = true;
      }
      else if (selectedIp.Equals(element.ReceivingIP))
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
    private static string SeverityNumberToText(int severity)
    {
      string _severity;
      switch (severity)
      {
        case 0:
          _severity = "Debug";
          break;
        case 1:
          _severity = "Warning";
          break;
        case (2 or 3):
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
