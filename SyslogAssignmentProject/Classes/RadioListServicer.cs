namespace SyslogAssignmentProject.Classes
{
  public class RadioListServicer
  {
    public List<Radio> RadioStore { get; private set; }

    public event Action ListChanged;

    public RadioListServicer() 
    {
      RadioStore = new List<Radio>();
    }
    public void UpdateList(Radio radioToAdd)
    {
      RadioStore.Add(radioToAdd);
      RemoveDuplicates();
      ListChanged?.Invoke();
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
  }
}
