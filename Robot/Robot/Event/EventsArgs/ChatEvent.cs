﻿using System;
namespace Robot.Event.Args
{
    public class SpeakingStartEvent : EventArgs
    {
        public string Phrase { get; }

        public SpeakingStartEvent(string Phrase)
        {
            this.Phrase = Phrase;
        }
    }

    public class SpeakingStopEvent : EventArgs
    {
        public string File { get; }

        public SpeakingStopEvent(string file)
        {
            this.File = file;
        }
    }
    public class UserChatEvent : EventArgs
    {
        public string Phrase { get; }

        public UserChatEvent(string Phrase)
        {
            this.Phrase = Phrase;
        }
    }
}
