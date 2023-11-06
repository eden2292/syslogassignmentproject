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

    public void UpdateList(SyslogMessage messageToAdd)
    {
      SyslogMessageList.Insert(0, messageToAdd);

      ListChanged?.Invoke();
    }
    public void RefreshList()
    {
      ListChanged?.Invoke();
    }
  }
}
