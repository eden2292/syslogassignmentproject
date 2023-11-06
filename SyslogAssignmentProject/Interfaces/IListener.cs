﻿namespace SyslogAssignmentProject.Interfaces
{
  /// <summary>
  /// Interface used to group UDP and TCP listeners in a 
  /// list together that are currently receiving messages from their
  /// respective connections.
  /// </summary>
  public interface IListener
  {
    Task StartListening();
    void StopListening();
    bool EarsFull { get; }
  }
}