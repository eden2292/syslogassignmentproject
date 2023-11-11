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
    public bool EarsFull { get; private set; }
    public IPEndPoint SourceIpAddress { get; private set; }

    public CancellationTokenSource TokenToStopListening { get; private set; }
    /// <summary>
    /// Creates a new instance of a UDP listener that starts listening for an incoming UDP connection.
    /// </summary>
    public UdpSyslogReceiver()
    {
      LocalClient = new UdpClient(S_ReceivingPortNumber);
      TokenToStopListening = new CancellationTokenSource();
      EarsFull = false;
      StartListening();

    }
    public bool CheckListener(int portNumber)
    {
      bool _valid = true;
      try
      {
        UdpClient _listener = new UdpClient(portNumber);
        _listener.Close();
      }
      catch
      {
        _valid = false;
      }
      return _valid;
    }
    public void StartListening()
    {
      Task _run = Task.Run(StartTaskListening, TokenToStopListening.Token);

    }
    /// <summary>
    /// Starts listening for UDP connections, once a connection is established,
    /// it can be read much easily than a TCP connection so is converted to ASCII
    /// and added to the livefeed.
    /// </summary>
    /// <returns>Fire and forget operation</returns>
    private async Task StartTaskListening()
    {
      while(true)
      {
        UdpReceiveResult _waitingToReceiveMessage = new UdpReceiveResult();
        try
        {
          _waitingToReceiveMessage = await LocalClient.ReceiveAsync();
        }
        catch (SocketException ex)
        {
          return;
        }
        EarsFull = true;
        byte[] _receivedMessage = _waitingToReceiveMessage.Buffer;
        SyslogMessage _formattedMessage;
        SourceIpAddress = _waitingToReceiveMessage.RemoteEndPoint;
        S_RadioList.UpdateList(new Radio("T6S3", SourceIpAddress.Address.ToString(), "UDP"));
        _formattedMessage = new SyslogMessage(SourceIpAddress.Address.ToString(), DateTime.Now, Encoding.ASCII.GetString(_receivedMessage), "UDP");

        if(((_formattedMessage.ParseMessage() & SyslogMessage.ParseFailure.Priority) != SyslogMessage.ParseFailure.Priority) &&
        !TokenToStopListening.IsCancellationRequested)
        {
          S_LiveFeedMessages.UpdateList(_formattedMessage);
        }
      }
    }

    /// <summary>
    /// Stops listening for UDP connections.
    /// </summary>
    public async Task StopListening()
    {
      TokenToStopListening.Cancel();
      LocalClient.Close();
    }
  }
}
