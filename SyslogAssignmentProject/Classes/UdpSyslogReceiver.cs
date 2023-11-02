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

    public async void StartListening()
    {
      _continue = true;
      while (_continue)
      {
        UdpReceiveResult _waitingToReceiveMessage = await LocalClient.ReceiveAsync();
        EarsFull = true;
        byte[] _receivedMessage = _waitingToReceiveMessage.Buffer;
        IPEndPoint _sourceInformation = _waitingToReceiveMessage.RemoteEndPoint;
        if (_continue)
        {
          Console.WriteLine(Encoding.ASCII.GetString(_receivedMessage)); //call Message 
        }
      }
    }
    public async void StopListening()
    {
      await Task.Run(() => _continue = false);
    }
  }
}
