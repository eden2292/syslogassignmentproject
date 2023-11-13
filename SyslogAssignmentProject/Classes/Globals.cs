using SyslogAssignmentProject.Classes;
/// <summary>
/// The Globals class includes global constants and variables for the entire project.
/// These allow it to be accessible from any file within the project.
/// </summary>
public static class Globals
{
  public const int DEFAULT_PORT_NUM = 514;
  public const string DEFAULT_IP4_ADDRESS = "127.0.0.1";
  public static string DEFAULT_DEBUG_COLOUR = "#00008B";
  public static string DEFAULT_INFO_COLOUR = "#000000";
  public static string DEFAULT_WARNING_COLOUR = "#FFFF00";
  public static string DEFAULT_ERROR_COLOUR = "#FF0000";

  public static ListServicer S_LiveFeedMessages = new ListServicer();
  public static RadioListServicer S_RadioList = new RadioListServicer();

  public static string S_ReceivingIpAddress = DEFAULT_IP4_ADDRESS;
  public static int S_ReceivingPortNumber = DEFAULT_PORT_NUM;

  public static string S_CurrentDebugColour = DEFAULT_DEBUG_COLOUR;
  public static string S_CurrentInfoColour = DEFAULT_INFO_COLOUR;
  public static string S_CurrentWarningColour = DEFAULT_WARNING_COLOUR;
  public static string S_CurrentErrorColour = DEFAULT_ERROR_COLOUR;

  public static string S_ChangingDebugColour = "color: " + DEFAULT_DEBUG_COLOUR;
  public static string S_ChangingInfoColour = "color: " + DEFAULT_INFO_COLOUR;
  public static string S_ChangingWarningColour = "color: " + DEFAULT_WARNING_COLOUR;
  public static string S_ChangingErrorColour = "color: " + DEFAULT_ERROR_COLOUR;

  public static string S_ListeningOptions = "Both";

  public static bool S_HideHiddenRadios = true;

  public static string APP_DIRECTORY
  {
    get
    {
      return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    }
  }
}