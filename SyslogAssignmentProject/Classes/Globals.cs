using SyslogAssignmentProject.Classes;
/// <summary>
/// The Globals class includes global constants and variables for the entire project.
/// These allow it to be accessible from any file within the project.
/// </summary>
public static class Globals
{
  public const int DEFAULT_PORT_NUM = 514;
  public const string DEFAULT_IP4_ADDRESS = "127.0.0.1";
  public const string DEFAULT_DEBUG_COLOUR = "#00008b";
  public const string DEFAULT_INFO_COLOUR = "#000000";
  public const string DEFAULT_WARNING_COLOUR = "#ffff00";
  public const string DEFAULT_ERROR_COLOUR = "#ff0000";
  public const int BYTE_BUFFER = 500;

  public static ListServicer S_LiveFeedMessages = new ListServicer();

  public static string S_ReceivingIpAddress = DEFAULT_IP4_ADDRESS;
  public static int S_ReceivingPortNumber = DEFAULT_PORT_NUM;

  public static string S_CurrentDebugColour = DEFAULT_DEBUG_COLOUR;
  public static string S_CurrentInfoColour = DEFAULT_INFO_COLOUR;
  public static string S_CurrentWarningColour = DEFAULT_WARNING_COLOUR;
  public static string S_CurrentErrorColour = DEFAULT_ERROR_COLOUR;
}