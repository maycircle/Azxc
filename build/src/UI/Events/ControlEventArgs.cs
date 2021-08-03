using Azxc.UI.Controls;
using System;

namespace Azxc.UI.Events
{
    public class ControlEventArgs : EventArgs
    {
        public Control item;

        public ControlEventArgs(Control item)
        {
            this.item = item;
        }
    }
}
