using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// Class responsible for listening for TCP connections and updating the livefeed
  /// with new syslog messages.
  /// </summary>
  public class TcpSyslogReceiver
  {
    public IPEndPoint SourceIpAddress { get; set; }
    private TcpListener _listener;
    private TcpClient _client;
    public CancellationTokenSource TokenToStopSource = new CancellationTokenSource();
    private readonly GlobalInjection _injectedGlobals;
    private readonly RadioListServicer _radioList;
    private readonly ListServicer _liveFeedMessages;
    private CancellationToken _stopListening;

    public TcpSyslogReceiver(GlobalInjection InjectedGlobals, RadioListServicer radioList, ListServicer liveFeedMessages)
    {
      _injectedGlobals = InjectedGlobals;
      _radioList = radioList;
      _liveFeedMessages = liveFeedMessages;
    }
    private void RefreshListener()
    {
      // Change IPAddress.Any to _injectedGlobals.S_ReceivingIpAddress if Sam says we need to.
      _listener = new TcpListener(IPAddress.Any, _injectedGlobals.S_ReceivingPortNumber);
      TokenToStopSource = new CancellationTokenSource();
      _stopListening = TokenToStopSource.Token;
    }
    public async Task StartListening()
    {
      RefreshListener();
      _listener.Start();
      while(!_stopListening.IsCancellationRequested)
      {
        try
        {
          _client = await _listener.AcceptTcpClientAsync(_stopListening);
          Task.Run(() => HandleStream(_client));
        }
        catch(Exception ex)
        {
          break;
        }
      }
      _listener.Stop();
      StartListening();
    }

    private async Task HandleStream(TcpClient sourceOfTcpMessage)
    {
      byte[] _buffer = new byte[250];
      int _bytesRead = 0;
      SyslogMessage _formattedMessage;
      NetworkStream _syslogMessageStream = sourceOfTcpMessage.GetStream();
      SourceIpAddress = sourceOfTcpMessage.Client.RemoteEndPoint as IPEndPoint;
      Radio _currentRadio = new Radio("T6S3", SourceIpAddress.Address.ToString(), SourceIpAddress.Port, "TCP");
      _radioList.UpdateList(_currentRadio);
      try
      {
        while((_bytesRead = await _syslogMessageStream.ReadAsync(_buffer, 0, _buffer.Length)) != 0)
        {
          if(_injectedGlobals.S_ListeningOptions.Equals("Both") || _injectedGlobals.S_ListeningOptions.Equals("TCP"))
          {
            _formattedMessage = new SyslogMessage(_injectedGlobals.S_ReceivingIpAddress, _injectedGlobals.S_ReceivingPortNumber,
              SourceIpAddress.Address.ToString(), SourceIpAddress.Port, DateTime.Now, Encoding.ASCII.GetString(_buffer, 0, _bytesRead), "TCP");
            if(((_formattedMessage.ParseMessage() & SyslogMessage.ParseFailure.Priority) != SyslogMessage.ParseFailure.Priority)
            && !_stopListening.IsCancellationRequested)
            {
              _liveFeedMessages.SyslogMessageList.Insert(0, _formattedMessage);
              _liveFeedMessages.invoke();
            }
          }
        }

      }
      catch(SocketException)
      {
        _radioList.ConnectionInterrupted(_currentRadio, "#FF0000");
      }
      catch(IOException)
      {
        _radioList.ConnectionInterrupted(_currentRadio, "#FF0000");
      }
      return;
    }
  }
}
