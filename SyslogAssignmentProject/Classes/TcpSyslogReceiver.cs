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
    private RadioListServicer S_RadioList = new RadioListServicer();
    private ListServicer S_LiveFeedMessages = new ListServicer();
    private CancellationToken _stopListening;
    private int S_ReceivingPortNumber;
    private int S_SendingPortNumber;
    private string S_ListeningOptions;

        public TcpSyslogReceiver(IPEndPoint sourceIpAddress, TcpListener listener, TcpClient client, CancellationTokenSource tokenStop, RadioListServicer radioList, ListServicer liveFeedMessages,
            CancellationToken stopListening, int receivingPortNumber, int sendingPortNumber, string listeningOptions)
        {
            SourceIpAddress = sourceIpAddress;
            _listener = listener;
            _client = client;
            TokenToStopSource = tokenStop;
            S_RadioList = radioList;
            S_LiveFeedMessages = liveFeedMessages;
            _stopListening = stopListening;
            S_ReceivingPortNumber = receivingPortNumber;
            S_SendingPortNumber = sendingPortNumber;
            S_ListeningOptions = listeningOptions;
        }
        private void RefreshListener()
    {
      // Change IPAddress.Any to S_ReceivingIpAddress if Sam says we need to.
      _listener = new TcpListener(IPAddress.Any, S_ReceivingPortNumber);
      TokenToStopSource = new CancellationTokenSource();
      _stopListening = TokenToStopSource.Token;
    }
    public async Task StartListening(TcpListener listener)
    {
      RefreshListener();
      listener.Start();
      while (!_stopListening.IsCancellationRequested)
      {
        try
        {
          _client = await listener.AcceptTcpClientAsync(_stopListening);
          Task.Run(() => HandleStream(_client));
        }
        catch(Exception ex)
        {
          break;
        }
      }
      listener.Stop();
      StartListening(listener);

    }
   // receivedDateTime, string fullMessage, string protocolType, string receivingIPAddress, int receivingPort
    private async Task HandleStream(TcpClient sourceOfTcpMessage, string listeningOptions, IPEndPoint sourceIpAddress)
    {
        byte[] _buffer = new byte[250];
        int _bytesRead = 0;
        SyslogMessage _formattedMessage;
        NetworkStream _syslogMessageStream = sourceOfTcpMessage.GetStream();
        sourceIpAddress = sourceOfTcpMessage.Client.RemoteEndPoint as IPEndPoint;
        Radio _currentRadio = new Radio("T6S3", sourceIpAddress.Address.ToString(), sourceIpAddress.Port, "TCP");
        S_RadioList.UpdateList(_currentRadio);
        try
        {
          while ((_bytesRead = await _syslogMessageStream.ReadAsync(_buffer, 0, _buffer.Length)) != 0)
          {
            if (listeningOptions.Equals("Both") || listeningOptions.Equals("TCP"))
          {
              _formattedMessage = new SyslogMessage(SourceIpAddress.Address.ToString(), SourceIpAddress.Port, DateTime.Now,
                Encoding.ASCII.GetString(_buffer, 0, _bytesRead), "TCP");
              if (((_formattedMessage.ParseMessage() & SyslogMessage.ParseFailure.Priority) != SyslogMessage.ParseFailure.Priority)
              && !_stopListening.IsCancellationRequested)
              {
                S_LiveFeedMessages.SyslogMessageList.Insert(0, _formattedMessage);
                S_LiveFeedMessages.invoke();
              }
            }
          }

        }
        catch (SocketException)
        {
          S_RadioList.ConnectionInterrupted(_currentRadio, "#FF0000");
        }
        catch (IOException)
        {
          S_RadioList.ConnectionInterrupted(_currentRadio, "#FF0000");
        }
      return;
    }
  }
}
