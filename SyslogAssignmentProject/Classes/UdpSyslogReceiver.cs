using System.Net.Sockets;
using System.Net;
using System.Text;
using MudBlazor;

namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// Class responsible for listening for UDP connections and updating the
  /// livefeed with new syslog messages.
  /// </summary>
  public class UdpSyslogReceiver
  {
    public IPEndPoint SourceIpAddress { get; private set; }
    public int ListeningPort { get; private set; }
    public CancellationTokenSource TokenToStopSource { get; set; }

    private UdpClient _udpListener;
    private CancellationToken _stopListening;

    private readonly GlobalInjection _injectedGlobals;
    private readonly RadioListServicer _injectedRadioServicer;
    private readonly ListServicer _injectedListServicer;

    /// <summary>
    /// Constantly listens for UDP syslog messages on a specified port on all accessible ip addresses.
    /// </summary>
    /// <param name="globalInjection">Singleton that is used to obtain which port and ip address the object listens on.</param>
    /// <param name="injectedRadioServicer">List of all unique radios that data has been received from.</param>
    /// <param name="injectedListServicer">List of all processed syslog messages.</param>
    public UdpSyslogReceiver(GlobalInjection globalInjection, RadioListServicer injectedRadioServicer, ListServicer injectedListServicer)
    {
      _injectedGlobals = globalInjection;
      _injectedRadioServicer = injectedRadioServicer;
      _injectedListServicer = injectedListServicer;
      ListeningPort = _injectedGlobals.ReceivingPortNumber;
    }
    /// <summary>
    /// Listener updates with new values before listening for new UDP connections.
    /// </summary>
    private void RefreshListener()
    {
      try
      {
        if(IPAddress.Parse(_injectedGlobals.ReceivingIpAddress).AddressFamily == AddressFamily.InterNetwork)
          _udpListener = new UdpClient(_injectedGlobals.ReceivingPortNumber, AddressFamily.InterNetwork);
        else
          _udpListener = new UdpClient(_injectedGlobals.ReceivingPortNumber, AddressFamily.InterNetworkV6);
      }
      catch (SocketException)
      {
        // If entered port number is bad, reset to default port number.
        _injectedGlobals.ReceivingPortNumber = GlobalInjection.DEFAULT_PORT_NUM;
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
      UdpReceiveResult _result = new UdpReceiveResult();
      while (!_stopListening.IsCancellationRequested)
      {
        try
        {
          _result = await _udpListener.ReceiveAsync(_stopListening);
          Task.Run(() => HandleStream(_result.RemoteEndPoint, _result.Buffer));
        }
        catch (Exception ex)
        {
          break;
        }
      }
      _udpListener.Close();
      StartListening();
    }
    /// <summary>
    /// Asynchronously decodes a UDP connection and parses its information into the radio page and syslog message list.
    /// </summary>
    /// <param name="clientEndpoint">IP endpoint from where packet was received from.</param>
    /// <param name="message">The received UDP message in bytes.</param>
    /// <returns>Fire and forget.</returns>
    private async Task HandleStream(IPEndPoint clientEndpoint, byte[] message)
    {
      if (_injectedGlobals.ListeningOptions.Equals("Both") || _injectedGlobals.ListeningOptions.Equals("UDP"))
      {
        try
        {
          SourceIpAddress = clientEndpoint;
          SyslogMessage _formattedMessage;

          _injectedRadioServicer.UpdateList(new Radio("T6S3", SourceIpAddress.Address.ToString(), SourceIpAddress.Port, "UDP"));

          _formattedMessage = new SyslogMessage(_injectedGlobals, _injectedGlobals.ReceivingIpAddress, _injectedGlobals.ReceivingPortNumber,
            SourceIpAddress.Address.ToString(), SourceIpAddress.Port, DateTime.Now,
            Encoding.ASCII.GetString(message), "UDP");

          if (((_formattedMessage.ParseMessage() & SyslogMessage.ParseFailure.Priority) != SyslogMessage.ParseFailure.Priority) &&
            !_stopListening.IsCancellationRequested)
          {
            _injectedListServicer.SyslogMessageList.Add(_formattedMessage);
            _injectedListServicer.RefreshList();

          }
        }
        catch
        { }
      }
      return;
    }
  }
}
