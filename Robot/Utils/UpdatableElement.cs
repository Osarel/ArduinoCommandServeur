using System;
using System.Collections.Generic;
using System.Text;

namespace Robot
{
    public interface UpdatableElement
    {

        public bool Stop();
        public bool Save();
        public bool AddToList();
        public UpdatableElement GetLastInstance();
    }

    public enum UpdatableType {
        Arduino,
        Element,
        Sheet
    }
}
