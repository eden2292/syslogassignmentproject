using Microsoft.AspNetCore.Components;
using SyslogAssignmentProject.Classes;
using SyslogAssignmentProject.Interfaces;
using System.Net.Sockets;

namespace SyslogAssignmentProject.Services
{
  public class BackgroundRunner
  {
    private CancellationTokenSource _cancellationTokenForListener;
    private Task _backgroundListener;

    public BackgroundRunner()
    {
      _cancellationTokenForListener = new CancellationTokenSource();
      _backgroundListener = Task.Run(BackgroundListener, _cancellationTokenForListener.Token);
    }

    private async Task BackgroundListener()
    {
      // Listens to active radios on tcp and udp protocols, if disconnected, it is removed.
      List<IListener> _listeningOnTcpAndUdp = new List<IListener>();
      while (!_cancellationTokenForListener.Token.IsCancellationRequested)
      {
        // listen for tcp and listen for udp
        // if a connection is established we want to create a new listener.

        TcpSyslogReceiver _tcpListener = new TcpSyslogReceiver();
        UdpSyslogReceiver _udpListener = new UdpSyslogReceiver();

        _udpListener.StartListening();
        _tcpListener.StartListening();

        if (_udpListener.EarsFull)
        {
          _listeningOnTcpAndUdp.Add(_udpListener);
          _udpListener = new UdpSyslogReceiver();
        }
        if (_tcpListener.EarsFull)
        {
          _listeningOnTcpAndUdp.Add(_tcpListener);
          _tcpListener = new TcpSyslogReceiver();
        }
      }
    }
    public void Stop()
    {
      _cancellationTokenForListener.Cancel();
    }
  }

}
