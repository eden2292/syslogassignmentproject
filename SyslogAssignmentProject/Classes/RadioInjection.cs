namespace SyslogAssignmentProject.Classes
{
  public class RadioInjection
  {
    private readonly RadioListServicer _radioListServicer;

    public RadioInjection(RadioListServicer radioListServicer)
    {
      _radioListServicer = radioListServicer;
    }
    public Radio radio { get; set; }

    public void UpdateRadioHiddenValue()
    {
      _radioListServicer.RadioStore.Single(x => x.Id == radio.Id).Hidden = radio.Hidden;
    }
  }
}
