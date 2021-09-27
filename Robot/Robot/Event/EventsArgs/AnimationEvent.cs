using Robot.Action;
using System;
using System.Collections.Generic;
using System.Text;

namespace Robot.Event.Args
{
    public class SheetStartedEvent : EventArgs
    {
        public Sheet Sheet { get; }
        public Dictionary<string, object> Variable { get; }
        public SheetStartedEvent(Sheet Sheet, Dictionary<string, object> Variable)
        {
            this.Sheet = Sheet;
            this.Variable = Variable;
        }
    }
    public class SheetFinishEvent : EventArgs
    {
        public Sheet Sheet { get; }
        public SheetFinishEvent(Sheet Sheet)
        {
            this.Sheet = Sheet;
        }
    }
    public class ActionStartedEvent : EventArgs
    {
        public Sheet Sheet { get; }
        public Liaison Liaison { get; }
        public AbstractAction Action { get; }
        public ActionStartedEvent(Sheet Sheet, Liaison Liaison, AbstractAction Action)
        {
            this.Sheet = Sheet;
            this.Liaison = Liaison;
            this.Action = Action;
        }
    }
    public class ActionFinishEvent : EventArgs
    {
        public Sheet Sheet { get; }
        public AbstractAction Action { get; }
        public ActionFinishEvent(Sheet Sheet, AbstractAction Action)
        {
            this.Sheet = Sheet;
            this.Action = Action;
        }
    }
}
