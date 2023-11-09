namespace SyslogAssignmentProject.Interfaces
{
  /// <summary>
  /// Interface used to group UDP and TCP listeners in a 
  /// list together that are currently receiving messages from their
  /// respective connections.
  /// </summary>
  public interface IListener
  {
    CancellationTokenSource TokenToStopListening { get; }
    void StartListening();
    Task StopListening();
    bool EarsFull { get; }
  }
}
