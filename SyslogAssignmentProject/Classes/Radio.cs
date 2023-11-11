

using System.Runtime.CompilerServices;
using System.Text;

public class Radio
{
  public string Name { get; set; }
  public string IPAddress { get; set; }
  public string TransportProtocol { get; set; }
  public string PathOfImage { get; set; }

  public string HexColour { get; set; }
  public Radio(string name, string ipAddress, string transportProtocol)
  {
    Name = name;
    IPAddress = ipAddress;
    TransportProtocol = transportProtocol;
    PathOfImage = "T6S3.jpg";
    HexColour = "#FFFFFF";
  }

  public string InternetProtocol()
  {
    string protocol = string.Empty;
    if (IPAddress.Contains('.'))
    {
      protocol = "IPv4";
    }
    else
    {
      protocol = "IPv6";
    }
    return protocol;
  }
}