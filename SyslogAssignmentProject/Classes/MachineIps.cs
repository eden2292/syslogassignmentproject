using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// Retrieves the IP addresses stored on the current machine.
  /// </summary>
  public class MachineIps
  {
    public List<string> AllIpAddresses { get; set; }

    public MachineIps()
    {
      AllIpAddresses = new List<string>();
      NetworkInterface[] _networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

      foreach (NetworkInterface _networkInterface in _networkInterfaces)
      {
        if (_networkInterface.OperationalStatus == OperationalStatus.Up &&
            (_networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
             _networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
        {
          IPInterfaceProperties _ipProperties = _networkInterface.GetIPProperties();
          foreach (UnicastIPAddressInformation _unicastAddress in _ipProperties.UnicastAddresses)
          {
            if (_unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork ||
              _unicastAddress.Address.AddressFamily == AddressFamily.InterNetworkV6)
            {
              AllIpAddresses.Add(_unicastAddress.Address.ToString());
            }
          }
        }
      }
    }
  }
}
