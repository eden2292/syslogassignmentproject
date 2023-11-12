using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SyslogSender
{
  internal class UDPSender : ISender
  {
    private IPEndPoint _endPoint;
    public IPEndPoint EndPoint
    {
      get { return _endPoint; }
      set
      {
        _endPoint = value;
        _thisClient = new UdpClient(EndPoint.AddressFamily);
      }
    }
    private UdpClient _thisClient;
    private Random _rng;
    private List<Task> _tasks = new List<Task>();

    public UDPSender(IPEndPoint endPoint)
    {
      EndPoint = endPoint;
      _rng = new Random();
    }

    public async Task StartSendingPackets()
    {
      for (uint packetNumber = 0; packetNumber < uint.MaxValue; packetNumber++)
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

      _ = await _thisClient.Client.SendToAsync(sendBuffer, SocketFlags.None, EndPoint);

      Console.WriteLine($"[PACKET #{packetNumber}]Sent message \"{text}\" to {EndPoint.Address}:{EndPoint.Port}");

      return;
    }
  }
}
