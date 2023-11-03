using Microsoft.AspNetCore.Components;

public static class Globals
{
  public const int DEFAULT_PORT_NUM = 514;
  public const string DEFAULT_IP4_ADDRESS = "127.0.0.1";
  public const string DEFAULT_DEBUG_COLOUR = "#00008b";
  public const string DEFAULT_INFO_COLOUR = "#000000";
  public const string DEFAULT_WARNING_COLOUR = "#ffff00";
  public const string DEFAULT_ERROR_COLOUR = "#ff0000";
  public static List<SyslogMessage> S_liveFeedMessages = new List<SyslogMessage>();

  public static string S_receivingIpAddress = DEFAULT_IP4_ADDRESS;
  public static int S_receivingPortNumber = DEFAULT_PORT_NUM;
}
