using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SyslogSender
{
  public class Program
  {
    public static void Main()
    {
      Socket ThisSocket;
      IPEndPoint EndPoint;
      IPAddress serverAddress;
      int portNumber;
      ProtocolType chosenProtocolType = ProtocolType.Udp;

      Console.WriteLine("Input IP address (leave blank to default to 127.0.0.1): ");
      if(!IPAddress.TryParse(Console.ReadLine(), out serverAddress))
        serverAddress = IPAddress.Parse("127.0.0.1");
      Console.WriteLine($"Set server IP address to {serverAddress}");

      Console.WriteLine("Input port number (leave blank to default to 514): ");
      if(!int.TryParse(Console.ReadLine(), out portNumber))
        portNumber = 514;
      Console.WriteLine($"Set server port number to {portNumber}");

      string yesForTCP;
      Console.WriteLine("Type y/Y to use TCP instead of UDP: ");
      yesForTCP = Console.ReadLine();
      if(yesForTCP.ToLower() == "y")
        chosenProtocolType = ProtocolType.Tcp;

      EndPoint = new IPEndPoint(serverAddress, portNumber);



      if(chosenProtocolType == ProtocolType.Udp)
      {
        UDPSender newUDPSender = new UDPSender(EndPoint);
        Task.Run(async () => { await newUDPSender.StartSendingPackets(); });
      }
      else
      {
        TCPSender newTCPSender = new TCPSender(EndPoint);
        Task.Run(async () => { await newTCPSender.StartSendingPackets(); });
      }

      Console.ReadKey();
    }
  }
}