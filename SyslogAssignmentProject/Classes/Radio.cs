

using System.Text;

public class Radio
{
  public string Name { get; set; }
  public string IPAddress { get; set; }
  public string TransportProtocol { get; set; }
  private string PathOfImage
  {
    get
    {
      return $"wwwroot/{Name}.jpg";
    }
  }

  public Radio(string _name, string _ipAddress, string _transportProtocol)
  {
    Name = _name;
    IPAddress = _ipAddress;
    TransportProtocol = _transportProtocol;
  }
}