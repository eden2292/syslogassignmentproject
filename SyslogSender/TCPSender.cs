using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SyslogSender
{
  internal class TCPSender : ISender
  {
    public IPEndPoint EndPoint { get; set; }
    public Socket ThisSocket { get; set; }
    private List<Task> tasks = new List<Task>();

    public TCPSender(IPEndPoint _endPoint, Socket _socket)
    {
      EndPoint = _endPoint;
      ThisSocket = _socket;
    }

    public async Task StartSendingPackets()
    {
      await ThisSocket.ConnectAsync(EndPoint);
      for(uint packetNumber = 0; packetNumber < uint.MaxValue; packetNumber++)
      {
        Console.WriteLine($"[PACKET #{packetNumber}]Sending message to {EndPoint.Address}:{EndPoint.Port}...");
        Task hello = SendPacketToAddress(packetNumber);
        //tasks.Add(SendPacketToAddress());
        //await hello;
        Console.WriteLine($"[PACKET #{packetNumber}]2 second delay period...");
        await Task.Delay(2000);
        Console.WriteLine($"[PACKET #{packetNumber}]CONTINUE");
      }
    }

    private async Task SendPacketToAddress(uint packetNumber)
    {
      string text = "<1>1 2023-11-03T15:00:00.000Z - - - - ABCDEFG123456";
      byte[] sendBuffer = Encoding.ASCII.GetBytes(text);

      _ = await ThisSocket.SendAsync(sendBuffer, SocketFlags.None);

      Console.WriteLine($"[PACKET #{packetNumber}]Sent message \"{text}!\" to {EndPoint.Address}:{EndPoint.Port}");

      byte[] acknowledgeBuffer = new byte[1024];
      _ = await ThisSocket.ReceiveAsync(acknowledgeBuffer, SocketFlags.None);

      string acknowledgeText = Encoding.UTF8.GetString(acknowledgeBuffer);
      Console.WriteLine($"[PACKET #{packetNumber}]Received acknowledgement \"{acknowledgeText}!\" from {EndPoint.Address}:{EndPoint.Port}!");

      return;
    }
  }
}
