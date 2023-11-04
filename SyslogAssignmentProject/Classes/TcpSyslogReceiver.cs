using SyslogAssignmentProject.Interfaces;
using System.Net.Sockets;
using System.Net;
using System.Text;
using static Globals;
using System.Net.Http;

namespace SyslogAssignmentProject.Classes
{
  public class TcpSyslogReceiver : IListener
  {
    private CancellationTokenSource _cancellationTokenSource;

    public bool EarsFull { get; private set; }
    public TcpSyslogReceiver() 
    {
      _cancellationTokenSource = new CancellationTokenSource();
      EarsFull = false;
    }
    public async Task StartListening()
    {
      TcpListener _listener = new TcpListener(IPAddress.Parse(S_receivingIpAddress), S_receivingPortNumber);

      _listener.Start();
      Console.WriteLine("Started listening");

      _ = Task.Run(async () =>
      {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
          TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
          Console.WriteLine("Accepted connection");
          EarsFull = true;
          HandleTcpClient(tcpClient);
        }
      });

    }

    private void HandleTcpClient(TcpClient client)
    {
      Console.WriteLine("Handling connection");
      using NetworkStream receivedConnection = client.GetStream();
      SyslogMessage _formattedMessage;
      IPEndPoint _sourceIpAddress = client.Client.RemoteEndPoint as IPEndPoint;

      while (true)
      {
        Console.WriteLine("Reading connection");
        byte[] _buffer = new byte[500];
        int _bytesRead;
        _bytesRead = receivedConnection.Read(_buffer, 0, _buffer.Length);
        _formattedMessage = new SyslogMessage(_sourceIpAddress.Address.ToString(), DateTime.Now, Encoding.ASCII.GetString(_buffer, 0, _bytesRead), "TCP");
        if (_formattedMessage.ParseMessage() < 4)
        {
          S_liveFeedMessages.UpdateList(_formattedMessage);
        }
      }
    }
    public async void StopListening()
    {
      _cancellationTokenSource.Cancel();
    }
  }
}
