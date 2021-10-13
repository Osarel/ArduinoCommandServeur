namespace Robot
{
    public class RobotOption : UpdatableElement
    {
        public string name = "[Saisir ici le nom]";
        public string langue = "FR-fr";
        public string voiceService = "no";
        public string voice = "fr-FR-Standard-B";
        public string voiceSubscriptionKey = "microsoftSubcriptionKey";
        public string voiceSubscriptionRegion = "francecentral";
        public string genre = "Male";

        public string startingAnimator = null;

        public bool autoStart = false;
        public bool autoUpdate = true;
        public bool debug = false;
        public bool AddToList()
        {
            ArduinoCommand.robot.Option = this;
            return true;
        }

        public UpdatableElement GetLastInstance()
        {
            return ArduinoCommand.robot.Option;
        }

        public bool Save()
        {
            ArduinoCommand.robot.SaveConfig();
            return true;
        }

        public bool Stop()
        {
            return true;
        }
    }
}