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
    
    /// <summary>
    /// Starts asynchronously listening for all incoming connections.
    /// </summary>
    public BackgroundRunner()
    {
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
      Console.WriteLine("RUN");
      _stop = false;
      // Contains UDP and TCP listeners that are actively receiving information.
      List<IListener> _listeningOnTcpAndUdp = new List<IListener>();
      string _listeningIpAddress = S_ReceivingIpAddress;
      int _listeningPortNumber = S_ReceivingPortNumber;
      string _listeningOptions = S_ListeningOptions;
      UdpSyslogReceiver _udpListener = null;
      TcpSyslogReceiver _tcpListener = null;
      if (_listeningOptions.Equals("Both"))
      {
        Console.WriteLine("FIRST");
        _udpListener = new UdpSyslogReceiver();
        _tcpListener = new TcpSyslogReceiver();
      }
      else if (_listeningOptions.Equals("UDP"))
      {
        Console.WriteLine("SECOND");
        _udpListener = new UdpSyslogReceiver();
      }
      else
      {
        _tcpListener = new TcpSyslogReceiver();
      }
      int i = 0;
      while (!_stop)
      {
        Console.WriteLine(i);
        i++;
        if (!_listeningOptions.Equals(S_ListeningOptions))
        {
          Console.WriteLine("CHANGE IN LISTENING");
          if (_udpListener is not null)
          {
            _udpListener.StopListening();
          }
          if (_tcpListener is not null)
          {
            _tcpListener.StopListening();
          }
          _listeningOnTcpAndUdp.ForEach(_listener => _listener.StopListening());
          S_LiveFeedMessages.UpdateIpAndPort();
          _stop = true;
          continue;
        }
        if (!_listeningIpAddress.Equals(S_ReceivingIpAddress) || _listeningPortNumber != S_ReceivingPortNumber)
        {
          Console.WriteLine("CHANGE IN IP / PORT");
          _listeningOnTcpAndUdp.ForEach(_listener => _listener.StopListening());
          if (_udpListener is not null)
          {
            _udpListener.StopListening();
          }
          if (_tcpListener is not null)
          {
            _tcpListener.StopListening();
          }

          ValidIpAddressAndPort(_listeningIpAddress, _listeningPortNumber);
          _stop = true;
          continue;
        }
        if (_udpListener is not null)
        {
          if (_udpListener.EarsFull || _udpListener.TokenToStopListening.Token.IsCancellationRequested)
          {
            _listeningOnTcpAndUdp.Add(_udpListener);
            _udpListener = new UdpSyslogReceiver();
          }
        }
        if (_tcpListener is not null)
        {
          if (_tcpListener.EarsFull || _tcpListener.TokenToStopListening.Token.IsCancellationRequested)
          {
            _listeningOnTcpAndUdp.Add(_tcpListener);
            _tcpListener = new TcpSyslogReceiver();
          }
        }
        // Removes all listeners that have finished listening.
        _listeningOnTcpAndUdp.RemoveAll(_listener => !_listener.TokenToStopListening.Token.IsCancellationRequested);
      }
      BackgroundListener();
    }
    private void ValidIpAddressAndPort(string oldIpAddress, int oldPortNumber)
    {
      try
      {
        UdpClient _tempUdpListener = new UdpClient(S_ReceivingPortNumber);
        _tempUdpListener.Close();
        TcpListener _tempTcpListener = new TcpListener(IPAddress.Parse(S_ReceivingIpAddress), S_ReceivingPortNumber);
        _tempTcpListener.Start();
        _tempTcpListener.Stop();
      }
      catch
      {
        S_ReceivingPortNumber = oldPortNumber;
        S_ReceivingIpAddress = oldIpAddress;
      }
      finally
      {
        S_LiveFeedMessages.UpdateIpAndPort();
      }
    }
  }

}
