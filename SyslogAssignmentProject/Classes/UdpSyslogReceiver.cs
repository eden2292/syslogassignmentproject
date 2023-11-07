using System.Net.Sockets;
using System.Net;
using System.Text;
using SyslogAssignmentProject.Interfaces;
using static Globals;

namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// Class responsible for listening for UDP connections and updating the
  /// livefeed with new syslog messages.
  /// </summary>
  public class UdpSyslogReceiver : IListener
  {
    public UdpClient LocalClient { get; set; }
    public IPAddress LocalHostIpAddress { get; set; }
    public bool EarsFull { get; private set; }

    public CancellationTokenSource TokenToStopListening { get; private set; }
    /// <summary>
    /// Creates a new instance of a UDP listener that starts listening for an incoming UDP connection.
    /// </summary>
    public UdpSyslogReceiver()
    {
      LocalClient = new UdpClient(S_ReceivingPortNumber);
      LocalHostIpAddress = IPAddress.Parse(S_ReceivingIpAddress);
      TokenToStopListening = new CancellationTokenSource();
      EarsFull = false;
      StartListening();

    }
    /// <summary>
    /// Starts listening for UDP connections, once a connection is established,
    /// it can be read much easily than a TCP connection so is converted to ASCII
    /// and added to the livefeed.
    /// </summary>
    /// <returns>Fire and forget operation</returns>
    public async Task StartListening()
    {
      while(!TokenToStopListening.IsCancellationRequested)
      {
        UdpReceiveResult _waitingToReceiveMessage = await LocalClient.ReceiveAsync();
        EarsFull = true;
        byte[] _receivedMessage = _waitingToReceiveMessage.Buffer;
        SyslogMessage _formattedMessage;
        IPEndPoint _sourceInformation = _waitingToReceiveMessage.RemoteEndPoint;
        _formattedMessage = new SyslogMessage(_sourceInformation.Address.ToString(), DateTime.Now, Encoding.ASCII.GetString(_receivedMessage), "UDP");

        if(_formattedMessage.ParseMessage() < 4 && !TokenToStopListening.IsCancellationRequested)
        {
          S_LiveFeedMessages.UpdateList(_formattedMessage);
        }
      }
    }
    /// <summary>
    /// Stops listening for UDP connections.
    /// </summary>
    public async void StopListening()
    {
      TokenToStopListening.Cancel();
      LocalClient.Close();
      LocalClient.Dispose();
    }
  }
}
