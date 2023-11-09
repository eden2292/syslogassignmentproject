using Microsoft.AspNetCore.Components;
using SyslogAssignmentProject.Classes;
using SyslogAssignmentProject.Interfaces;
using System.Net.Sockets;
using static Globals;

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
    /// Starts asynchronously listening for all incoming connections.
    /// </summary>
    public BackgroundRunner()
    {
      _tokenToStopListening = new CancellationTokenSource();
      _listensForAllIncomingConnections = Task.Run(BackgroundListener);
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
        if (_udpListener.EarsFull || _udpListener.TokenToStopListening.Token.IsCancellationRequested)
        {
          _listeningOnTcpAndUdp.Add(_udpListener);
          RadioStore.Add(_udpListener.ToString());
          _udpListener = new UdpSyslogReceiver();
        }
        if (_tcpListener.EarsFull || _tcpListener.TokenToStopListening.Token.IsCancellationRequested)
        {
          _listeningOnTcpAndUdp.Add(_tcpListener);
          RadioStore.Add(_tcpListener.ToString());
          _tcpListener = new TcpSyslogReceiver();
        }

        // Removes all listeners that have finished listening.
        _listeningOnTcpAndUdp.RemoveAll(_listener => !_listener.TokenToStopListening.Token.IsCancellationRequested);
        // put code to change the receiving port number and ip address here.
      }
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
