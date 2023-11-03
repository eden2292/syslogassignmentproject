using static Globals;
namespace SyslogAssignmentProject.Classes
{
  public class ListServicer
  {
    public List<SyslogMessage> ListToService {
      get
      {
        if (ListToService is null)
        {
          ListToService = new List<SyslogMessage>();
        }
        return ListToService;
      }
      private set
      {
        ListToService = value;
      }  
  }
    public event Action OnListChange;

    public void UpdateList()
    {
      ListToService = S_liveFeedMessages;
      OnListChange?.Invoke();
    }
  }
}
