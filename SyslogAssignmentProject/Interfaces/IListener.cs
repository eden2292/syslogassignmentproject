namespace SyslogAssignmentProject.Interfaces
{
  public interface IListener
  {
    void StartListening();
    void StopListening();
    bool EarsFull { get; }
  }
}
