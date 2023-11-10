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
    public IPEndPoint EndPoint { get; set; }
    private UdpClient ThisClient;
    private Random rng;
    private List<Task> tasks = new List<Task>();

    public UDPSender(IPEndPoint _endPoint)
    {
      EndPoint = _endPoint;
      ThisClient = new UdpClient();
      rng = new Random();
    }

    public async Task StartSendingPackets()
    {
      for (uint packetNumber = 0; packetNumber < uint.MaxValue; packetNumber++)
      {
        Console.WriteLine($"[PACKET #{packetNumber}]Sending message to {EndPoint.Address}:{EndPoint.Port}...");
        Task sendPacketTask = SendPacketToAddress(packetNumber);
        Console.WriteLine($"[PACKET #{packetNumber}]2 second delay period...");
        await Task.Delay(2000);
        Console.WriteLine($"[PACKET #{packetNumber}]CONTINUE");
      }
    }

    private async Task SendPacketToAddress(uint packetNumber)
    {
      string text = $"<{rng.Next(0, 24)}>1 {DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)} - - - - ABCDEFG123456";
      byte[] sendBuffer = Encoding.ASCII.GetBytes(text);

      _ = await ThisClient.Client.SendToAsync(sendBuffer, SocketFlags.None, EndPoint);

      Console.WriteLine($"[PACKET #{packetNumber}]Sent message \"{text}\" to {EndPoint.Address}:{EndPoint.Port}");

      return;
    }
  }
}
