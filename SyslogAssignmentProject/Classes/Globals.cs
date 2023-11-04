using Microsoft.AspNetCore.Components;
using SyslogAssignmentProject.Classes;

public static class Globals
{
  public const int DEFAULT_PORT_NUM = 514;
  public const string DEFAULT_IP4_ADDRESS = "127.0.0.1";
  public const string DEFAULT_DEBUG_COLOUR = "#00008b";
  public const string DEFAULT_INFO_COLOUR = "#000000";
  public const string DEFAULT_WARNING_COLOUR = "#ffff00";
  public const string DEFAULT_ERROR_COLOUR = "#ff0000";
  public static ListServicer S_liveFeedMessages = new ListServicer();

  public static string S_receivingIpAddress = DEFAULT_IP4_ADDRESS;
  public static int S_receivingPortNumber = DEFAULT_PORT_NUM;

  public static string S_currentDebugColour = DEFAULT_DEBUG_COLOUR;
  public static string S_currentInfoColour = DEFAULT_INFO_COLOUR;
  public static string S_currentWarningColour = DEFAULT_DEBUG_COLOUR;
  public static string S_currentErrorColour = DEFAULT_INFO_COLOUR;
}
