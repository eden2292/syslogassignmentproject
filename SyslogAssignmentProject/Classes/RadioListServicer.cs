namespace SyslogAssignmentProject.Classes
{
  public class RadioListServicer
  {
    public List<Radio> RadioStore { get; private set; }

    public event Action ListChanged;
    
    private Dictionary<string, Timer> _udpRadioTimer { get; set; }

    public RadioListServicer() 
    {
      RadioStore = new List<Radio>();
      _udpRadioTimer = new Dictionary<string, Timer>();
    }
    public void UpdateList(Radio radioToAdd)
    {
      RadioStore.Add(radioToAdd);
      if (radioToAdd.TransportProtocol.Equals("UDP"))
      {
        if (_udpRadioTimer.ContainsKey(radioToAdd.IPAddress))
        {
          _udpRadioTimer[radioToAdd.IPAddress].Dispose();
          _udpRadioTimer[radioToAdd.IPAddress] = new Timer(UdpInterrupted, radioToAdd, 5 * 60 * 1000, 0);
        }
        else
        {
          _udpRadioTimer.Add(radioToAdd.IPAddress, new Timer(UdpInterrupted, radioToAdd, 5 * 60 * 1000, 0));
          ConnectionInterrupted(radioToAdd, "#FFFFFF");

        }
      }
      RemoveDuplicates();
      ListChanged?.Invoke();
    }

    private void UdpInterrupted(object state)
    {
      _udpRadioTimer[(state as Radio).IPAddress].Dispose();
      ConnectionInterrupted(state as Radio, "#FF0000");
    }

    private void RemoveDuplicates()
    {
      List<Radio> _newList = RadioStore
      .GroupBy(_radio => new { _radio.IPAddress, _radio.TransportProtocol })
      .Select(_group => _group.First())
      .ToList();

      RadioStore = _newList;
    }

    public Radio GetRadio(string ipAddress, string transportProtocol)
    {
      Radio _toReturn = null;
      foreach (Radio _radio in RadioStore)
      {
        if (_radio.IPAddress.Equals(ipAddress) && _radio.TransportProtocol.Equals(transportProtocol))
        {
          _toReturn = _radio;
          break;
        }
      }
      return _toReturn;
    }

    public void ConnectionInterrupted(Radio makeRed, string hexColour)
    {
      int _indexOfRadio = RadioStore.FindIndex(_radio => _radio.IPAddress.Equals(makeRed.IPAddress) && 
      _radio.TransportProtocol.Equals(makeRed.TransportProtocol));
      makeRed.HexColour = hexColour;
      RadioStore[_indexOfRadio] = makeRed;
      ListChanged?.Invoke();
    }

    public List<string> UniqueIpAddresses()
    {
      List<string> _listOfIps = new List<string>();
      _listOfIps = RadioStore.GroupBy(_radio => _radio.IPAddress)
      .Select(_uniqueIp => _uniqueIp.First().IPAddress).ToList();
      return _listOfIps;
    }
  }
}
