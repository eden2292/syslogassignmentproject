using Microsoft.AspNetCore.Components;
using SyslogAssignmentProject.Classes;
using SyslogAssignmentProject.Interfaces;
using System.Net.Sockets;

namespace SyslogAssignmentProject.Services
{
  /// <summary>
  /// Starts running at the start of the application, is what
  /// listens to UDP and TCP connections. Ensures there is always
  /// an available listener for each protocol when they are currently
  /// receiving data.
  /// </summary>
  public class BackgroundRunner
  {
    private CancellationTokenSource _tokenToStopListening;
    private Task _listensForAllIncomingConnections;
    /// <summary>
    /// Starts asynchronously listenning for all incoming connections.
    /// </summary>
    public BackgroundRunner()
    {
      _tokenToStopListening = new CancellationTokenSource();
      _listensForAllIncomingConnections = Task.Run(BackgroundListener, _tokenToStopListening.Token);
    }
    /// <summary>
    /// Listens for TCP and UDP connections. If a connection is established,
    /// it is added to a list to continually receive information until it finishes
    /// whilst new instances are created to listen for more radios.
    /// </summary>
    /// <returns>Fire and forget method</returns>
    private async Task BackgroundListener()
    {
      // Contains UDP and TCP listeners that are actively receiving information.
      List<IListener> _listeningOnTcpAndUdp = new List<IListener>();
      UdpSyslogReceiver _udpListener = new UdpSyslogReceiver();
      TcpSyslogReceiver _tcpListener = new TcpSyslogReceiver();
      while (!_tokenToStopListening.Token.IsCancellationRequested)
      {
        _udpListener = new UdpSyslogReceiver();
        _tcpListener = new TcpSyslogReceiver();

        if(_udpListener.EarsFull)
        {
          _listeningOnTcpAndUdp.Add(_udpListener);
          _udpListener = new UdpSyslogReceiver();
        }
        if(_tcpListener.EarsFull)
        {
          _listeningOnTcpAndUdp.Add(_tcpListener);
          _tcpListener = new TcpSyslogReceiver();
        }
        // Removes all listeners that have finished listening.
        _listeningOnTcpAndUdp.RemoveAll(_listener => !_listener.EarsFull);
      }
      _tcpListener.StopListening();
      _udpListener.StopListening();
      _listeningOnTcpAndUdp.ForEach(_listener => _listener.StopListening());
    }
    /// <summary>
    /// Stops the background listener which triggers all UDP and TCP listeners
    /// to stop listening.
    /// </summary>
    public void Stop()
    {
      _tokenToStopListening.Cancel();
    }
  }

}
