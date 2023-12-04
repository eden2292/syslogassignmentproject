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
    public string ListeningIP { get; set; }
    public int ListeningPort { get; set; }
    public CancellationTokenSource TokenToStopSource { get; set; }

    private CancellationToken _stopListening;
    private TcpListener _listener;
    private readonly GlobalInjection _injectedGlobals;
    private readonly RadioListServicer _radioList;
    private readonly ListServicer _liveFeedMessages;
    /// <summary>
    /// Constantly listens for TCP syslog messages on a specified port on all accessible ip addresses.
    /// </summary>
    /// <param name="injectedGlobals">Singleton that is used to obtain which port and ip address the object listens on.</param>
    /// <param name="radioList">List of all unique radios that data has been received from.</param>
    /// <param name="liveFeedMessages">List of all processed syslog messages.</param>
    public TcpSyslogReceiver(GlobalInjection injectedGlobals, RadioListServicer radioList, ListServicer liveFeedMessages)
    {
      ListeningIP = injectedGlobals.ReceivingIpAddress;
      ListeningPort = injectedGlobals.ReceivingPortNumber;
      _injectedGlobals = injectedGlobals;
      _radioList = radioList;
      _liveFeedMessages = liveFeedMessages;
    }
    /// <summary>
    /// Listener updates with new values before listening for new TCP connections.
    /// </summary>
    private void RefreshListener()
    {
      try
      {
        _listener = new TcpListener(IPAddress.Parse(_injectedGlobals.ReceivingIpAddress), _injectedGlobals.ReceivingPortNumber);
      }
      catch
      {
        // If entered port number is bad, reset to default port number.
        _injectedGlobals.ReceivingPortNumber = GlobalInjection.DEFAULT_PORT_NUM;
        ListeningPort = GlobalInjection.DEFAULT_PORT_NUM;
        _injectedGlobals.InvokeBadPortChange();
        RefreshListener();
      }
      _injectedGlobals.InvokeGoodPortChange();
      TokenToStopSource = new CancellationTokenSource();
      _stopListening = TokenToStopSource.Token;
    }
    /// <summary>
    /// Asynchronously listens for incoming connections and accepts them.
    /// </summary>
    /// <returns>Fire and forget.</returns>
    public async Task StartListening()
    {
      RefreshListener();
      _listener.Start();
      while (!_stopListening.IsCancellationRequested)
      {
        try
        {
          TcpClient _client = await _listener.AcceptTcpClientAsync(_stopListening);
          Task.Run(() => HandleStream(_client));
        }
        catch (Exception ex)
        {
          break;
        }
      }
      _listener.Stop();
      StartListening();
    }
    /// <summary>
    /// Asynchronously decodes a TCP connection and parses its information into the radio page and syslog message list.
    /// </summary>
    /// <param name="sourceOfTcpMessage">Tcp client that is sending syslog messages.</param>
    /// <returns>Fire and forget.</returns>
    private async Task HandleStream(TcpClient sourceOfTcpMessage)
    {
      using(sourceOfTcpMessage)
      {
        byte[] _buffer = new byte[250];
        int _bytesRead = 0;
        SyslogMessage _formattedMessage;
        NetworkStream _syslogMessageStream = sourceOfTcpMessage.GetStream();

        SourceIpAddress = sourceOfTcpMessage.Client.RemoteEndPoint as IPEndPoint;

        Radio _currentRadio = new Radio("T6S3", SourceIpAddress.Address.ToString(), SourceIpAddress.Port, "TCP");
        try
        {
          while((_bytesRead = await _syslogMessageStream.ReadAsync(_buffer, 0, _buffer.Length)) > -1 && !_stopListening.IsCancellationRequested)
          {
            if(_bytesRead == 0)
            {
              throw new SocketException();
            }
            _radioList.UpdateList(_currentRadio);
            // If we are listening for a TCP connection and we are listening on the ip address that connection has come through on, it should be accepted.
            if((_injectedGlobals.ListeningOptions.Equals("Both") || _injectedGlobals.ListeningOptions.Equals("TCP")))
            {
              _formattedMessage = new SyslogMessage(_injectedGlobals, _injectedGlobals.ReceivingIpAddress, _injectedGlobals.ReceivingPortNumber,
                SourceIpAddress.Address.ToString(), SourceIpAddress.Port, DateTime.Now, Encoding.ASCII.GetString(_buffer, 0, _bytesRead), "TCP");

              if(((_formattedMessage.ParseMessage() & SyslogMessage.ParseFailure.Priority) != SyslogMessage.ParseFailure.Priority)
              && !_stopListening.IsCancellationRequested)
              {
                _liveFeedMessages.SyslogMessageList.Insert(0, _formattedMessage);
                _liveFeedMessages.RefreshList();
              }
            }
          }
          Console.WriteLine("break");
        }
        // The below excpetions suggest that the radio has cut out and must be marked as red.
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
}
