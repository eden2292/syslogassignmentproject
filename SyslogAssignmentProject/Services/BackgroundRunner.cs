using Microsoft.AspNetCore.Components;
using SyslogAssignmentProject.Classes;
using SyslogAssignmentProject.Interfaces;
using System.Net;
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
    private bool _stop;
    private Task _listensForAllIncomingConnections;
    private List<IListener> _listeningOnTcpAndUdp = new List<IListener>();
    private int _indexOfActiveUdpListener = 0;
    private int _indexOfActiveTcpListener = 1;

    /// <summary>
    /// Starts asynchronously listening for all incoming connections.
    /// </summary>
    public BackgroundRunner()
    {
      _listensForAllIncomingConnections = Task.Run(BackgroundListener);
    }

    private async Task BackgroundListener()
    {
      TcpSyslogReceiver _tcpSyslogReceiver = new TcpSyslogReceiver();
      UdpSyslogReceiver _udpSyslogReceiver = new UdpSyslogReceiver();
      _tcpSyslogReceiver.StartListening();
      _udpSyslogReceiver.StartListening();
      string _listeningIpAddress = S_ReceivingIpAddress;
      int _listeningPortNumber = S_ReceivingPortNumber;
      string _listeningOptions = S_ListeningOptions;
      while (true)
      {
        if (!_listeningIpAddress.Equals(S_ReceivingIpAddress) || _listeningPortNumber != S_ReceivingPortNumber || !_listeningOptions.Equals(S_ListeningOptions))
        {
          _tcpSyslogReceiver.TokenToStopSource.Cancel();
          _udpSyslogReceiver.TokenToStopSource.Cancel();
          _listeningIpAddress = S_ReceivingIpAddress;
          _listeningPortNumber = S_ReceivingPortNumber;
          _listeningOptions = S_ListeningOptions;
        }
      }
    }
  }
}
