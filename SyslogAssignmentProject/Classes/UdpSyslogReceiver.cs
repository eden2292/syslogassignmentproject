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
    public IPEndPoint SourceIpAddress { get; private set; }
    private UdpClient _udpListener;
    public CancellationTokenSource TokenToStopSource;
    private CancellationToken _stopListening;
    private RadioListServicer S_RadioList = new RadioListServicer();
    private ListServicer S_LiveFeedMessages = new ListServicer();
    private int S_ReceivingPortNumber;
    private int S_SendingPortNumber;
    private string S_ListeningOptions;

    public UdpSyslogReceiver(IPEndPoint sourceIpAddress, UdpClient listener, TcpClient client, CancellationTokenSource tokenStop, RadioListServicer radioList, ListServicer liveFeedMessages,
    CancellationToken stopListening, int receivingPortNumber, int sendingPortNumber, string listeningOptions)
    {
        SourceIpAddress = sourceIpAddress;
        _udpListener = listener;
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
      _udpListener = new UdpClient(S_ReceivingPortNumber);
      TokenToStopSource = new CancellationTokenSource();
      _stopListening = TokenToStopSource.Token;
    }

    public async Task StartListening(CancellationToken stopListening)
    {
      RefreshListener();
      UdpReceiveResult _result = new UdpReceiveResult();
      while (!stopListening.IsCancellationRequested)
      {
        try
        {
          Console.WriteLine(stopListening.CanBeCanceled.ToString());
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
      if (S_ListeningOptions.Equals("Both") || S_ListeningOptions.Equals("UDP"))
      {
        try
        {
          SourceIpAddress = clientEndpoint;
          SyslogMessage _formattedMessage;
          S_RadioList.UpdateList(new Radio("T6S3", SourceIpAddress.Address.ToString(), SourceIpAddress.Port, "UDP"));
          _formattedMessage = new SyslogMessage(SourceIpAddress.Address.ToString(), SourceIpAddress.Port, DateTime.Now,
            Encoding.ASCII.GetString(message), "UDP");
          if (((_formattedMessage.ParseMessage() & SyslogMessage.ParseFailure.Priority) != SyslogMessage.ParseFailure.Priority) &&
            !_stopListening.IsCancellationRequested)
          {
            S_LiveFeedMessages.SyslogMessageList.Insert(0, _formattedMessage) ;
            S_LiveFeedMessages.invoke();

          }
        }
        catch
        {//Can we please put something here?
         }
      }
      return;
    }
  }
}
