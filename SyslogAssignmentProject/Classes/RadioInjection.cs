namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// Used to inject a radio into the radio page.
  /// </summary>
  public class RadioInjection
  {
    public Radio Radio { get; set; } = new();

    public string IpAddress { get; set; } = null;
  }
}
