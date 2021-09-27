using System;
using System.Collections.Generic;
using System.Text;

namespace Robot.Event.Args
{
    public class ElementActualValueChanged : EventArgs
    {
        public float NewValue { get; }
        public ElementActualValueChanged(float value)
        {
            NewValue = value;
        }
    }
}
