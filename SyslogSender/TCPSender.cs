using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace SyslogSender
{
  internal class TCPSender : ISender
  {
    private IPEndPoint _endPoint;
    public IPEndPoint EndPoint
    {
      get { return _endPoint; }
      set
      {
        _endPoint = value;
        _thisClient = new TcpClient(EndPoint.AddressFamily);
      }
    }
    private TcpClient _thisClient;
    private Random _rng;
    private List<Task> _tasks = new List<Task>();

    public TCPSender(IPEndPoint _endPoint)
    {
      EndPoint = _endPoint;
      _rng = new Random();
    }

    public async Task StartSendingPackets()
    {
      await _thisClient.ConnectAsync(EndPoint);
      for(uint packetNumber = 0; packetNumber < uint.MaxValue; packetNumber++)
      {
        Console.WriteLine($"[PACKET #{packetNumber}]Sending message to {EndPoint.ToString()}...");
        Task sendPacketTask = SendPacketToAddress(packetNumber);
        Console.WriteLine($"[PACKET #{packetNumber}]2 second delay period...");
        await Task.Delay(2000);
        Console.WriteLine($"[PACKET #{packetNumber}]CONTINUE");
      }
    }

    private async Task SendPacketToAddress(uint packetNumber)
    {
      string text = $"<{_rng.Next(0, 24)}>1 {DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)} - - - - ABCDEFG123456";
      byte[] sendBuffer = Encoding.ASCII.GetBytes(text);

      _ = await _thisClient.Client.SendAsync(sendBuffer, SocketFlags.None);

      Console.WriteLine($"[PACKET #{packetNumber}]Sent message \"{text}\" to {EndPoint.Address}:{EndPoint.Port}");

      return;
    }
  }
}
