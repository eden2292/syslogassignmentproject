using SyslogAssignmentProject.Interfaces;
using System.Net.Sockets;
using System.Net;
using System.Text;
using static Globals;
using System.Net.Http;
using Syncfusion.Blazor;

namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// Class responsible for listening for TCP connections and updating the livefeed
  /// with new syslog messages.
  /// </summary>
  public class TcpSyslogReceiver
  {
    public IPEndPoint SourceIpAddress { get; private set; }
    private TcpListener _listener;
    private TcpClient _client;
    private CancellationTokenSource _tokenToStopSource;
    private CancellationToken _stopListening;

    private async Task RefreshListener()
    {
      _listener = new TcpListener(IPAddress.Parse(S_ReceivingIpAddress), S_ReceivingPortNumber);
      _tokenToStopSource = new CancellationTokenSource();
      _stopListening = _tokenToStopSource.Token;
      return;
    }
    public bool CheckListener(int portNumber)
    {
      bool _valid = true;
      try
      {
        TcpListener _listener = new TcpListener(IPAddress.Parse(S_ReceivingIpAddress), portNumber);
        _listener.Start();
        _listener.Stop();
      }
      catch
      {
        _valid = false;
      }
      return _valid;
    }
    public async void StartListening()
    {
      await RefreshListener();
      _listener.Start();
      while (!_stopListening.IsCancellationRequested)
      {
        try
        {
          _client = await _listener.AcceptTcpClientAsync(_stopListening);
          Task.Run(() => HandleStream(_client));
        }
        catch(Exception ex)
        {
          Console.WriteLine($"STOPPED TCP {ex.Message}");
          _listener.Stop();
          _listener = null;
          break;
        }
      }
      
    }

    private async Task HandleStream(TcpClient sourceOfTcpMessage)
    {
      if (S_ListeningOptions.Equals("Both") || S_ListeningOptions.Equals("TCP"))
      {
        byte[] _buffer = new byte[250];
        int _bytesRead = 0;
        SyslogMessage _formattedMessage;
        NetworkStream _syslogMessageStream = sourceOfTcpMessage.GetStream();
        SourceIpAddress = sourceOfTcpMessage.Client.RemoteEndPoint as IPEndPoint;
        Radio _currentRadio = new Radio("T6S3", SourceIpAddress.Address.ToString(), "TCP");
        S_RadioList.UpdateList(_currentRadio);
        try
        {
          while ((_bytesRead = await _syslogMessageStream.ReadAsync(_buffer, 0, _buffer.Length)) != 0)
          {
            _formattedMessage = new SyslogMessage(SourceIpAddress.Address.ToString(), DateTime.Now,
              Encoding.ASCII.GetString(_buffer, 0, _bytesRead), "TCP");
            if (((_formattedMessage.ParseMessage() & SyslogMessage.ParseFailure.Priority) != SyslogMessage.ParseFailure.Priority)
            && !_stopListening.IsCancellationRequested)
            {
              S_LiveFeedMessages.UpdateList(_formattedMessage);
            }
          }
        }
        catch (SocketException)
        {
          S_RadioList.ConnectionInterrupted(_currentRadio, "#FF0000");
        }
      }
      return;
    }

    public async Task StopListening()
    {
      _tokenToStopSource.Cancel();
      while (_listener is not null)
      {
        continue;
      }
      return;
    }
  }
}
