namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// Used to store global variables within a class that is a singleton injected into pages.
  /// </summary>
  public class GlobalInjection
  {


    public const int DEFAULT_PORT_NUM = 514;
    public const string DEFAULT_IP4_ADDRESS = "127.0.0.1";
    public const string DEFAULT_DEBUG_COLOUR = "#00008B";
    public const string DEFAULT_INFO_COLOUR = "#000000";
    public const string DEFAULT_WARNING_COLOUR = "#FFFF00";
    public const string DEFAULT_ERROR_COLOUR = "#FF0000";
    public string ListeningOptions { get; set; } = "Both";
    public event Action BadChangePortNumber;
    public event Action GoodChangePortNumber;
    public string AppDirectory
    {
      get
      {
        return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
      }
    }

    public string ReceivingIpAddress { get; set; }
    public int ReceivingPortNumber { get; set; }
    public string CurrentDebugColour { get; set; }
    public string CurrentInfoColour { get; set; }
    public string CurrentWarningColour { get; set; }
    public string CurrentErrorColour { get; set; }
    /// <summary>
    /// Sets initial values as constants that can be manipulated given that it will be used as a singleton.
    /// </summary>
    public GlobalInjection()
    {
      ReceivingIpAddress = DEFAULT_IP4_ADDRESS;
      ReceivingPortNumber = DEFAULT_PORT_NUM;
      CurrentDebugColour = DEFAULT_DEBUG_COLOUR;
      CurrentInfoColour = DEFAULT_INFO_COLOUR;
      CurrentWarningColour = DEFAULT_WARNING_COLOUR;
      CurrentErrorColour = DEFAULT_ERROR_COLOUR;
    }
    /// <summary>
    /// If a port is changed and a listener cannot use the port, action is triggered to appropriately inform user.
    /// </summary>
    public void InvokeBadPortChange()
    {
      BadChangePortNumber?.Invoke();
    }
    /// <summary>
    /// If a port is changed and a listener can use the port, action is triggered to appropriately inform user.
    /// </summary>
    public void InvokeGoodPortChange()
    {
      GoodChangePortNumber?.Invoke();
    }

  }
}
