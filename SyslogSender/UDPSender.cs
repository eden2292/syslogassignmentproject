using System;
using System.Collections.Generic;
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
    public Socket ThisSocket { get; set; }
    private List<Task> tasks = new List<Task>();

    public UDPSender(IPEndPoint _endPoint, Socket _socket)
    {
      EndPoint = _endPoint;
      ThisSocket = _socket;
    }

    public async Task StartSendingPackets()
    {
      for (uint packetNumber = 0; packetNumber < uint.MaxValue; packetNumber++)
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
      byte[] send_buffer = Encoding.ASCII.GetBytes(text);

      ThisSocket.SendTo(send_buffer, EndPoint);

      Console.WriteLine($"[PACKET #{packetNumber}]Sent message \"{text}\" to {EndPoint.Address}:{EndPoint.Port}");

      return;
    }
  }
}
