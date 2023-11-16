namespace SyslogAssignmentProject.Classes
{
  public class GlobalInjection
  {

    public int DEFAULT_PORT_NUM { get; set; } = 514;
    public const string DEFAULT_IP4_ADDRESS = "127.0.0.1";
    public const string DEFAULT_DEBUG_COLOUR = "#00008B";
    public const string DEFAULT_INFO_COLOUR = "#000000";
    public const string DEFAULT_WARNING_COLOUR = "#FFFF00";
    public const string DEFAULT_ERROR_COLOUR = "#FF0000";
    public string S_ListeningOptions { get; set; } = "Both";
    public bool S_HideHiddenRadios { get; set; } = true;
    public event Action BadChangePortNumber;
    public event Action GoodChangePortNumber;
    public ListServicer S_LiveFeedMessages = new ListServicer();
    public RadioListServicer S_RadioList = new RadioListServicer();
    public string S_AppDirectory
    {
      get
      {
        return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
      }
    }

    public string S_ReceivingIpAddress { get; set; }
    public int S_ReceivingPortNumber { get; set; }
    public string S_CurrentDebugColour { get; set; }
    public string S_CurrentInfoColour { get; set; }
    public string S_CurrentWarningColour { get; set; }
    public string S_CurrentErrorColour { get; set; }
    public GlobalInjection()
    {
      S_ReceivingIpAddress = DEFAULT_IP4_ADDRESS;
      S_ReceivingPortNumber = DEFAULT_PORT_NUM;
      S_CurrentDebugColour = DEFAULT_DEBUG_COLOUR;
      S_CurrentInfoColour = DEFAULT_INFO_COLOUR;
      S_CurrentWarningColour = DEFAULT_WARNING_COLOUR;
      S_CurrentErrorColour = DEFAULT_ERROR_COLOUR;
    }
    public void InvokeBadPortChange()
    {
      BadChangePortNumber?.Invoke();
    }
    public void InvokeGoodPortChange()
    {
      GoodChangePortNumber?.Invoke();
    }

  }
}
