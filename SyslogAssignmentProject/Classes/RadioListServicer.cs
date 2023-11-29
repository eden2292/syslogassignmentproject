using System.Linq;
//using MainLayout;

namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// A list of radios and methods to interact with the list.
  /// </summary>
  public class RadioListServicer
  {
    private readonly GlobalInjection _injectedGlobals;
    private Dictionary<string, Timer> _udpRadioTimer { get; set; }
    public bool Hidden { get; set; }

    public List<Radio> RadioStore { get; set; }
    public event Action ListChanged;
    /// <summary>
    /// Creates a new list of radios and a new dictionary of timers to time how long it has been since a UDP message on a radio.
    /// </summary>
    public RadioListServicer(GlobalInjection injectedGlobals)
    {
      RadioStore = new List<Radio>();
      _udpRadioTimer = new Dictionary<string, Timer>();
      _injectedGlobals = injectedGlobals;
    }

    /// <summary>
    /// Adds a radio to the list and ensures a UDP radio's last message timer is reset.
    /// </summary>
    /// <param name="radioToAdd">The radio to add to the list.</param>
    public void UpdateList(Radio radioToAdd)
    {
      if(RadioStore.FindIndex(_radio => _radio.IpAddress.Equals(radioToAdd.IpAddress) &&
        _radio.TransportProtocol.Equals(radioToAdd.TransportProtocol)) == -1)
      {
        RadioStore.Add(radioToAdd);
        ListChanged?.Invoke();
      }
      if(radioToAdd.TransportProtocol.Equals("UDP"))
      {
        if(_udpRadioTimer.ContainsKey(radioToAdd.IpAddress))
        {
          _udpRadioTimer[radioToAdd.IpAddress].Dispose();
          _udpRadioTimer[radioToAdd.IpAddress] = new Timer(UdpInterrupted, radioToAdd, 5 * 60 * 1000, 0);
          ConnectionInterrupted(radioToAdd, "#8a9496");
          ListChanged?.Invoke();
        }
        else
        {
          _udpRadioTimer.Add(radioToAdd.IpAddress, new Timer(UdpInterrupted, radioToAdd, 5 * 60 * 1000, 0));
        }
      }
    }
    /// <summary>
    /// Gets list of radios that need to be displayed based on whether they are set to hidden or visible.
    /// </summary>
    /// <returns>Returns radio list based on whether the user wants hidden or visible radios.</returns>
    public List<Radio> VisibleRadios()
    {
      List<Radio> _radiosForNavbar = new List<Radio>();
      if(_injectedGlobals.HideHiddenRadios)
      {
        _radiosForNavbar = RadioStore.FindAll(_radio => !_radio.Hidden);
      }
      else
      {
        _radiosForNavbar = RadioStore;
      }
      return _radiosForNavbar;
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
      RadioStore[_indexOfRadio].HexColour = hexColour;
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

    public void HideRadio(Radio toChange)
    {
      int _index = RadioStore.FindIndex(_radio => _radio.IpAddress == toChange.IpAddress && _radio.TransportProtocol == toChange.TransportProtocol);
      if(_index != -1)
      {
        RadioStore[_index].Hidden = toChange.Hidden;
        ListChanged?.Invoke();
      }
    }

    public void Delete(Radio rad)
    {
      RadioStore.Remove(rad);
      ListChanged?.Invoke();
    }

    /// <summary>
    /// Timer triggers which means that UDP radio needs to be marked as red as 5 minutes has passed since last message.
    /// </summary>
    /// <param name="state">Radio that triggers the timer.</param>
    private void UdpInterrupted(object state)
    {
      _udpRadioTimer[(state as Radio).IpAddress].Dispose();
      ConnectionInterrupted(state as Radio, "#FF0000");
    }
  }
}
