namespace SyslogAssignmentProject.Classes
{
    public class GlobalInjection
    {

        public int DEFAULT_PORT_NUM{get; set;} = 514;
        public string DEFAULT_IP4_ADDRESS { get; set; } = "127.0.0.1";
        public string DEFAULT_DEBUG_COLOUR { get; set; } = "#00008B";
        public string DEFAULT_INFO_COLOUR { get; set; } = "#000000";
        public string DEFAULT_WARNING_COLOUR { get; set; }  = "#FFFF00";
        public string DEFAULT_ERROR_COLOUR { get; set; } = "#FF0000";
        public string S_ListeningOptions { get; set; } = "Both";
        public bool S_HideHiddenRadios = true;
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
        public string S_ChangingDebugColour { get; set; }
        public string S_ChangingInfoColour { get; set; }
        public string S_ChangingWarningColour { get; set; }
        public string S_ChangingErrorColour { get; set; }
        public GlobalInjection()
        {
            S_ReceivingIpAddress = DEFAULT_IP4_ADDRESS;
            S_ReceivingPortNumber = DEFAULT_PORT_NUM;
            S_CurrentDebugColour = DEFAULT_DEBUG_COLOUR;
            S_CurrentInfoColour = DEFAULT_INFO_COLOUR;
            S_CurrentWarningColour = DEFAULT_WARNING_COLOUR;
            S_CurrentErrorColour = DEFAULT_ERROR_COLOUR;
            S_ChangingDebugColour = "color: " + DEFAULT_DEBUG_COLOUR;
            S_ChangingInfoColour = "color: " + DEFAULT_INFO_COLOUR;
            S_ChangingWarningColour = "color: " + DEFAULT_WARNING_COLOUR;
            S_ChangingErrorColour = "color: " + DEFAULT_ERROR_COLOUR;
        }

    }
}
