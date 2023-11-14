using SyslogAssignmentProject.Classes;

namespace SyslogAssignmentProject.Services
{
  /// <summary>
  /// Starts running at the start of the application, is what
  /// listens to UDP and TCP connections. Ensures there is always
  /// an available listener for each protocol when they are currently
  /// receiving data.
  /// </summary>
  public class BackgroundRunner
  {
    private readonly GlobalInjection _injectedGlobals;
    private readonly ListServicer _injectedListServicer;
    private readonly RadioListServicer _injectedRadioServicer;

    /// <summary>
    /// Starts asynchronously listening for all incoming connections.
    /// </summary>
    public BackgroundRunner(GlobalInjection injectedGlobals, ListServicer injectedListServicer, RadioListServicer injectedRadioServicer)
    {
      _injectedGlobals = injectedGlobals;
      _injectedListServicer = injectedListServicer;
      _injectedRadioServicer = injectedRadioServicer;
      Task.Run(BackgroundListener);
    }

    private async Task BackgroundListener()
    {
      TcpSyslogReceiver _tcpSyslogReceiver = new TcpSyslogReceiver(_injectedGlobals, _injectedRadioServicer, _injectedListServicer);
      UdpSyslogReceiver _udpSyslogReceiver = new UdpSyslogReceiver(_injectedGlobals, _injectedRadioServicer, _injectedListServicer);
      _tcpSyslogReceiver.StartListening();
      _udpSyslogReceiver.StartListening();
      string _listeningIpAddress = _injectedGlobals.S_ReceivingIpAddress;
      int _listeningPortNumber = _injectedGlobals.S_ReceivingPortNumber;
      string _listeningOptions = _injectedGlobals.S_ListeningOptions;
      while (true)
      {
        if (!_listeningIpAddress.Equals(_injectedGlobals.S_ReceivingIpAddress) || _listeningPortNumber != _injectedGlobals.S_ReceivingPortNumber || !_listeningOptions.Equals(_injectedGlobals.S_ListeningOptions))
        {
          _tcpSyslogReceiver.TokenToStopSource.Cancel();
          _udpSyslogReceiver.TokenToStopSource.Cancel();
          _listeningIpAddress = _injectedGlobals.S_ReceivingIpAddress;
          _listeningPortNumber =_injectedGlobals.S_ReceivingPortNumber;
          _listeningOptions = _injectedGlobals.S_ListeningOptions;
        }
      }
    }
  }
}
