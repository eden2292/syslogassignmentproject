using SyslogAssignmentProject.Interfaces;
using System.Net.Sockets;
using System.Net;
using System.Text;
using static Globals;
using System.Net.Http;

namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// Class responsible for listening for TCP connections and updating the livefeed
  /// with new syslog messages.
  /// </summary>
  public class TcpSyslogReceiver : IListener
  {
    private CancellationTokenSource _tokenToStopListening;

    // EarsFull is used to check if this object cannot currently listen to incoming connections (as it is currently receiving a message).
    public bool EarsFull { get; private set; }
    /// <summary>
    /// Creates a new instance of a TCP listener that starts listening for an incoming TCP connection.
    /// </summary>
    public TcpSyslogReceiver() 
    {
      _tokenToStopListening = new CancellationTokenSource();
      EarsFull = false;
      StartListening();
    }
    /// <summary>
    /// Starts listening for TCP connections, once a connection is established,
    /// the client is parsed to be handled.
    /// </summary>
    /// <returns>Fire and forget operation</returns>
    public async Task StartListening()
    {
      TcpListener _listener = new TcpListener(IPAddress.Parse(S_ReceivingIpAddress), S_ReceivingPortNumber);

      _listener.Start();

      _ = Task.Run(async () =>
      {
        while(!_tokenToStopListening.IsCancellationRequested)
        {
          TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
          EarsFull = true;
          HandleTcpClient(tcpClient);
        }
      });

    }
    /// <summary>
    /// Uses a NetworkStream to read the Syslog message before adding it to the live feed.
    /// </summary>
    /// <param name="client">TCP client object to convert to ASCII syslog message</param>
    private async void HandleTcpClient(TcpClient client)
    {
      NetworkStream receivedConnection = client.GetStream();
      SyslogMessage _formattedMessage;
      IPEndPoint _sourceIpAddress = client.Client.RemoteEndPoint as IPEndPoint;
      while(!_tokenToStopListening.IsCancellationRequested)
      {
        byte[] _buffer = new byte[BYTE_BUFFER];
        int _bytesRead;
        try
        {
          _bytesRead = await receivedConnection.ReadAsync(_buffer, 0, _buffer.Length);
          _formattedMessage = new SyslogMessage(_sourceIpAddress.Address.ToString(), DateTime.Now, Encoding.ASCII.GetString(_buffer, 0, _bytesRead), "TCP");
          if (((_formattedMessage.ParseMessage() & SyslogMessage.ParseFailure.Priority) != SyslogMessage.ParseFailure.Priority) && !_tokenToStopListening.IsCancellationRequested)
          {
            S_LiveFeedMessages.UpdateList(_formattedMessage);
          }
        }
        catch(Exception ex) 
        {
          StopListening();
          Console.WriteLine(ex.Message);
        }
      }
    }
    /// <summary>
    /// Stops listening for TCP connections.
    /// </summary>
    public async void StopListening()
    {
      _tokenToStopListening.Cancel();
      EarsFull = false;
    }
  }
}
