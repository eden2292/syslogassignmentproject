
using System.Net;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

/// <summary>
/// The radio and its networking information.
/// </summary>
public class Radio
{
  public Guid Id { get; set; }
  public string Name { get; set; }
  public string IpAddress { get; set; }
  public string IpFamily
  {
    get
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
  public bool Hidden { get; set; } = false;
  /// <summary>
  /// Creates a new radio which is a source where syslog messages are transmitted from.
  /// </summary>
  /// <param name="name">The name/model of the radio.</param>
  /// <param name="ipAddress">The IP address that the radio is sending information from.</param>
  /// <param name="portNumber">The port number that the radio is sending information from.</param>
  /// <param name="transportProtocol">The transport protocol being used to send information.</param>
  public Radio(string name, string ipAddress, int portNumber, string transportProtocol)
  {
    Id = Guid.NewGuid();
    Name = name;
    IpAddress = ipAddress;
    TransportProtocol = transportProtocol;
    PathOfImage = "T6S3.png";
    HexColour = "#8a9496";
    PortNumber = portNumber;
  }
  
  /// <summary>
  /// Creates new radio without settings any properties.
  /// </summary>
  public Radio()
  {
    //  This is needed please do not remove. 
  }
}