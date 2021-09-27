using System;
namespace Robot.Event.Args
{
    class SpeakingStartEvent : EventArgs
    {
        public string Phrase { get; } 

        public SpeakingStartEvent(string Phrase)
        {
            this.Phrase = Phrase;
        }
    }

    class SpeakingStopEvent : EventArgs
    {
        public string Phrase { get; }

        public SpeakingStopEvent(string Phrase)
        {
            this.Phrase = Phrase;
        }
    }
    class UserChatEvent : EventArgs
    {
        public string Phrase { get; }

        public UserChatEvent(string Phrase)
        {
            this.Phrase = Phrase;
        }
    }
}
