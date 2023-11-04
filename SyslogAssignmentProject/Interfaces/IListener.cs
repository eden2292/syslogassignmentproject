namespace SyslogAssignmentProject.Interfaces
{
  public interface IListener
  {
    Task StartListening();
    void StopListening();
    bool EarsFull { get; }
  }
}
