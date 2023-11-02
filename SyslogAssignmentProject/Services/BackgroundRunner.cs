using SyslogAssignmentProject.Classes;
using SyslogAssignmentProject.Interfaces;
using System.Net.Sockets;

namespace SyslogAssignmentProject.Services
{
  public class BackgroundRunner
  {
    private CancellationTokenSource _cancellationTokenSource;
    private Task _backgroundTask;

    public BackgroundRunner()
    {
      _cancellationTokenSource = new CancellationTokenSource();
      _backgroundTask = Task.Run(BackgroundTask, _cancellationTokenSource.Token);
    }

    private async Task BackgroundTask()
    {
      // Listens to active radios on tcp and udp protocols, if disconnected, it is removed.
      List<IListener> _listeningOnTcpAndUdp = new List<IListener>();
      while (!_cancellationTokenSource.Token.IsCancellationRequested)
      {
        // listen for tcp and listen for udp
        // if a connection is established we want to create a new listener.

        TcpSyslogReceiver _tcpListener = new TcpSyslogReceiver();
        UdpSyslogReceiver _udpListener = new UdpSyslogReceiver();

        _udpListener.StartListening();

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
      _cancellationTokenSource.Cancel();
    }
  }

}
