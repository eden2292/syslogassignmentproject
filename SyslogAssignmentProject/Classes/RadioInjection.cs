using static MudBlazor.CategoryTypes;

namespace SyslogAssignmentProject.Classes
{
  /// <summary>
  /// Used to inject a radio into the radio page.
  /// </summary>
  public class RadioInjection
  {
    public Radio Radio { get; set; } = new Radio();

    public string IpAddress { get; set; } = null;

    private readonly RadioListServicer _radioListServicer;

    /// <summary>
    /// Allows access to list of current radios for operations.
    /// </summary>
    /// <param name="radioListServicer">Singleton of list of current radios.</param>
    public RadioInjection(RadioListServicer radioListServicer)
    {
      _radioListServicer = radioListServicer;
    }

    public void UpdateRadioHiddenValue()
    {
      _radioListServicer.ChangeRadio(Radio);
    }
  }
}
