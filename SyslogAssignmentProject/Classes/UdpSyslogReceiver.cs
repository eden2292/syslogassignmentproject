using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// Class responsible for listening for UDP connections and updating the
  /// livefeed with new syslog messages.
  /// </summary>
  public class UdpSyslogReceiver
  {
    private readonly GlobalInjection _injectedGlobals;
    private readonly RadioListServicer _injectedRadioServicer;
    private readonly ListServicer _injectedListServicer;
    public IPEndPoint SourceIpAddress { get; private set; }
    private UdpClient _udpListener;
    public int ListeningPort { get; private set; }
    public CancellationTokenSource TokenToStopSource;
    private CancellationToken _stopListening;

    public UdpSyslogReceiver(GlobalInjection globalInjection, RadioListServicer injectedRadioServicer, ListServicer injectedListServicer)
    {
      _injectedGlobals = globalInjection;
      _injectedRadioServicer = injectedRadioServicer;
      _injectedListServicer = injectedListServicer;
      ListeningPort = _injectedGlobals.S_ReceivingPortNumber;
    }

    private void RefreshListener()
    {
      _udpListener = new UdpClient(ListeningPort);
      TokenToStopSource = new CancellationTokenSource();
      _stopListening = TokenToStopSource.Token;
    }

    public async Task StartListening()
    {
      RefreshListener();
      UdpReceiveResult _result = new UdpReceiveResult();
      while(!_stopListening.IsCancellationRequested)
      {
        try
        {
          Console.WriteLine(_stopListening.CanBeCanceled.ToString());
          _result = await _udpListener.ReceiveAsync(_stopListening);
          _ = Task.Run(() => HandleMessageAsync(_result.RemoteEndPoint, _result.Buffer));
        }
        catch(Exception ex)
        {
          break;
        }
      }
      _udpListener.Close();
      StartListening();
    }

    private async Task HandleMessageAsync(IPEndPoint clientEndpoint, byte[] message)
    {
      if(_injectedGlobals.S_ListeningOptions.Equals("Both") || _injectedGlobals.S_ListeningOptions.Equals("UDP"))
      {
        try
        {
          SourceIpAddress = clientEndpoint;
          SyslogMessage _formattedMessage;
          _injectedRadioServicer.UpdateList(new Radio("T6S3", SourceIpAddress.Address.ToString(), SourceIpAddress.Port, "UDP"));
          _formattedMessage = new SyslogMessage(_injectedGlobals.S_ReceivingIpAddress, _injectedGlobals.S_ReceivingPortNumber,
            SourceIpAddress.Address.ToString(), SourceIpAddress.Port, DateTime.Now,
            Encoding.ASCII.GetString(message), "UDP");
          if(((_formattedMessage.ParseMessage() & SyslogMessage.ParseFailure.Priority) != SyslogMessage.ParseFailure.Priority) &&
            !_stopListening.IsCancellationRequested)
          {
            _injectedListServicer.SyslogMessageList.Add(_formattedMessage);
            _injectedListServicer.invoke();

          }
        }
        catch(Exception e)
        {
          Console.WriteLine(e);
        }
      }
      return; // Unreachable. Plz fix. 
    }
  }
}
