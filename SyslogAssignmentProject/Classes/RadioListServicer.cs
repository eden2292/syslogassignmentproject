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

    public void HideReveal(Radio toChange)
    {
      int _indexOfRadioToChange = RadioStore.IndexOf(toChange);
      if(RadioStore[_indexOfRadioToChange].Hidden)
      {
        RadioStore[_indexOfRadioToChange].Hidden = false;
      }
      else
      {
        RadioStore[_indexOfRadioToChange].Hidden = true;
      }
    }

    /// <summary>
    /// Adds a radio to the list.
    /// </summary>
    /// <param name="radioToAdd">The radio to add to the list.</param>
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

    /// <summary>
    /// Removes any duplicate radios (i.e. radios that use the same IP and
    /// transport protocol as another) from the list.
    /// </summary>
    private void RemoveDuplicates()
    {
      List<Radio> _newList = RadioStore
      .GroupBy(_radio => new { _radio.IPAddress, _radio.TransportProtocol })
      .Select(_group => _group.First())
      .ToList();

      RadioStore = _newList;
    }

    /// <summary>
    /// Gets a radio from the list.
    /// </summary>
    /// <param name="ipAddress">The IP address of the radio you want.</param>
    /// <param name="transportProtocol">The transport protocol of the radio you want.</param>
    /// <returns>The radio you requested (null if it cannot be found).</returns>
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

    /// <summary>
    /// Sets a radio's colour in the tree view to a different colour.
    /// </summary>
    /// <param name="makeRed">The radio whose colour you are changing.</param>
    /// <param name="hexColour">The radio's new colour as a hex code.</param>
    public void ConnectionInterrupted(Radio makeRed, string hexColour)
    {
      int _indexOfRadio = RadioStore.FindIndex(_radio => _radio.IPAddress.Equals(makeRed.IPAddress) && 
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
      _listOfIps = RadioStore.GroupBy(_radio => _radio.IPAddress)
      .Select(_uniqueIp => _uniqueIp.First().IPAddress).ToList();
      return _listOfIps;
    }
  }
}
