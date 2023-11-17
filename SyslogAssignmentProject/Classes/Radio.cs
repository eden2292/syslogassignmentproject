
/// <summary>
/// The radio and its networking information.
/// </summary>
public class Radio
{
  public Guid Id { get; set; }
  public string Name { get; set; }
  public string IPAddress { get; set; }
  public string TransportProtocol { get; set; }
  public string PathOfImage { get; set; }
  public string HexColour { get; set; }
  public int PortNumber { get; set; }
  public bool Hidden { get; set; } = false;

  public Radio(string name, string ipAddress, int portNumber, string transportProtocol)
  {
    Id = Guid.NewGuid();
    Name = name;
    IPAddress = ipAddress;
    TransportProtocol = transportProtocol;
    PathOfImage = "T6S3.jpg";
    HexColour = "#FFFFFF";
    PortNumber = portNumber;
  }
  public Radio()
  {
    //This is needed please do not remove. 
  }
}