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
      Console.WriteLine("Run");
      string _listeningIpAddress = S_ReceivingIpAddress;
      int _listeningPortNumber = S_ReceivingPortNumber;
      UdpSyslogReceiver _udpListener = new UdpSyslogReceiver();
      TcpSyslogReceiver _tcpListener = new TcpSyslogReceiver();
      while (!_tokenToStopListening.Token.IsCancellationRequested)
      {
        if (!_listeningIpAddress.Equals(S_ReceivingIpAddress) || _listeningPortNumber != S_ReceivingPortNumber)
        {
          Console.WriteLine($"The changed values, ipAddress: {_listeningIpAddress} and {S_ReceivingIpAddress}, portNumber: {_listeningPortNumber} and {S_ReceivingPortNumber}");
          _listeningOnTcpAndUdp.ForEach(listener => listener.StopListening());
          _udpListener.StopListening();
          _tcpListener.StopListening();
          BackgroundListener();
        }
        if (_udpListener.EarsFull || _udpListener.TokenToStopListening.Token.IsCancellationRequested)
        {
          _listeningOnTcpAndUdp.Add(_udpListener);
          //
          _udpListener = new UdpSyslogReceiver();
        }
        if (_tcpListener.EarsFull || _tcpListener.TokenToStopListening.Token.IsCancellationRequested)
        {
          _listeningOnTcpAndUdp.Add(_tcpListener);
          //RadioStore.Add(new Radio("T6S3", _tcpListener.SourceIpAddress.Address.ToString(), "TCP"));
          _tcpListener = new TcpSyslogReceiver();
        }
        // Removes all listeners that have finished listening.
        _listeningOnTcpAndUdp.RemoveAll(_listener => !_listener.TokenToStopListening.Token.IsCancellationRequested);
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
