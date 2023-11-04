using static Globals;
namespace SyslogAssignmentProject.Classes
{
  public class ListServicer
  {
    public List<SyslogMessage> ListToService { get; private set; }

    public event Action ListChanged;

    public ListServicer()
    {
      ListToService = new List<SyslogMessage>();
    }

    public void UpdateList(SyslogMessage messageToAdd)
    {
      ListToService.Insert(0, messageToAdd);

      ListChanged?.Invoke();
    }
  }
}
