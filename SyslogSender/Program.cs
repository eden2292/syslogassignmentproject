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
      string ipAddressString;
      int portNumber;
      ProtocolType chosenProtocolType = ProtocolType.Udp;

      Console.WriteLine("Input IP address: ");
      ipAddressString = Console.ReadLine();
      Console.WriteLine("Input port number: ");
      portNumber = Convert.ToInt32(Console.ReadLine());

      string yesForTCP;
      Console.WriteLine("Type y/Y to use TCP instead of UDP: ");
      yesForTCP = Console.ReadLine();
      if(yesForTCP.ToLower() == "y")
        chosenProtocolType = ProtocolType.Tcp;

      IPAddress serverAddr = IPAddress.Parse(ipAddressString);

      EndPoint = new IPEndPoint(serverAddr, portNumber);



      if(chosenProtocolType == ProtocolType.Udp)
      {
        ThisSocket = new Socket(EndPoint.AddressFamily, SocketType.Dgram, chosenProtocolType);
        UDPSender newUDPSender = new UDPSender(EndPoint, ThisSocket);
        Task.Run(async () => { await newUDPSender.StartSendingPackets(); });
      }
      else
      {
        ThisSocket = new Socket(EndPoint.AddressFamily, SocketType.Stream, chosenProtocolType);
        TCPSender newTCPSender = new TCPSender(EndPoint, ThisSocket);
        Task.Run(async () => { await newTCPSender.StartSendingPackets(); });
      }

      Console.ReadKey();
    }
  }
}