
using System.Net;
/// <summary>
/// The radio and its networking information.
/// </summary>
public class Radio
{
  public string Name { get; set; }
  public string IpAddress { get; set; }
  public string IpFamily 
  { get
    {
      string _addressFamily;
      if (IPAddress.Parse(IpAddress).AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
      {
        _addressFamily = "IPv4";
      }
      else if (IPAddress.Parse(IpAddress).AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
      {
        _addressFamily = "IPv6";
      }
      else
      {
        _addressFamily = "Unknown";
      }
      return _addressFamily;
    }
  }
  public string TransportProtocol { get; set; }
  public string PathOfImage { get; set; }

  public string HexColour { get; set; }
  public int PortNumber { get; set; }
  public bool Hidden { get; set; }
  public Radio(string name, string ipAddress, int portNumber, string transportProtocol)
  {
    Name = name;
    IpAddress = ipAddress;
    TransportProtocol = transportProtocol;
    PathOfImage = "T6S3.jpg";
    HexColour = "#FFFFFF";
    PortNumber = portNumber;
    Hidden = false;
  }
  public Radio()
  {
    //  This is needed please do not remove. 
  }
}