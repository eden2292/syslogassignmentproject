namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// A list of radios and methods to interact with the list.
  /// </summary>
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

    /// <summary>
    /// Adds a radio to the list.
    /// </summary>
    /// <param name="radioToAdd">The radio to add to the list.</param>
    public void UpdateList(Radio radioToAdd)
    {
      RadioStore.Add(radioToAdd);
      if(radioToAdd.TransportProtocol.Equals("UDP"))
      {
        if(_udpRadioTimer.ContainsKey(radioToAdd.IpAddress))
        {
          _udpRadioTimer[radioToAdd.IpAddress].Dispose();
          _udpRadioTimer[radioToAdd.IpAddress] = new Timer(UdpInterrupted, radioToAdd, 5 * 60 * 1000, 0);
        }
        else
        {
          _udpRadioTimer.Add(radioToAdd.IpAddress, new Timer(UdpInterrupted, radioToAdd, 5 * 60 * 1000, 0));
          ConnectionInterrupted(radioToAdd, "#FFFFFF");

        }
      }
      List<Radio> _newList = RadioStore.GroupBy(_radio => new { _radio.IpAddress, _radio.TransportProtocol }).Select(_group => _group.First()).ToList();
      RadioStore = _newList;
      ListChanged?.Invoke();
    }

    private void UdpInterrupted(object state)
    {
      _udpRadioTimer[(state as Radio).IpAddress].Dispose();
      ConnectionInterrupted(state as Radio, "#FF0000");
    }

    /// <summary>
    /// Sets a radio's colour in the tree view to a different colour.
    /// </summary>
    /// <param name="makeRed">The radio whose colour you are changing.</param>
    /// <param name="hexColour">The radio's new colour as a hex code.</param>
    public void ConnectionInterrupted(Radio makeRed, string hexColour)
    {
      int _indexOfRadio = RadioStore.FindIndex(_radio => _radio.IpAddress.Equals(makeRed.IpAddress) &&
      _radio.TransportProtocol.Equals(makeRed.TransportProtocol));
      makeRed.HexColour = hexColour;
      RadioStore[_indexOfRadio] = makeRed;
      ListChanged?.Invoke();
    }

    /// <summary>
    /// Gets all unique IPs from the radio list.
    /// </summary>
    /// <returns>A list of unique IP addresses.</returns>
    public List<string> UniqueIpAddresses()
    {
      List<string> _listOfIps = new List<string>();
      _listOfIps = RadioStore.GroupBy(_radio => _radio.IpAddress)
      .Select(_uniqueIp => _uniqueIp.First().IpAddress).ToList();
      return _listOfIps;
    }
  }
}
