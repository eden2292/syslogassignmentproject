using System.Net.Sockets;
using System.Net;
using System.Text;
using SyslogAssignmentProject.Interfaces;
using static Globals;

namespace SyslogAssignmentProject.Classes
{
  public class UdpSyslogReceiver : IListener
  {
    public UdpClient LocalClient { get; set; }
    public IPAddress LocalHostIpAddress { get; set; }
    public bool EarsFull { get; private set; }

    private bool _continue;

    public UdpSyslogReceiver()
    {
      LocalClient = new UdpClient(S_receivingPortNumber);

      LocalHostIpAddress = IPAddress.Parse(S_receivingIpAddress);

      EarsFull = false;

    }

    public async Task StartListening()
    {
      _continue = true;
      while (_continue)
      {
        UdpReceiveResult _waitingToReceiveMessage = await LocalClient.ReceiveAsync();
        EarsFull = true;
        byte[] _receivedMessage = _waitingToReceiveMessage.Buffer;
        SyslogMessage _formattedMessage;
        IPEndPoint _sourceInformation = _waitingToReceiveMessage.RemoteEndPoint;
        if (_continue)
        {
          _formattedMessage = new SyslogMessage(_sourceInformation.Address.ToString(), DateTime.Now, Encoding.ASCII.GetString(_receivedMessage), "UDP");

          if (_formattedMessage.ParseMessage() < 4)
          {
            Console.WriteLine("Testing");
            S_liveFeedMessages.UpdateList(_formattedMessage);
          }
        }
      }
    }
    public async void StopListening()
    {
      await Task.Run(() => _continue = false);
    }
  }
}
