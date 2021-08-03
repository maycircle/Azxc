using System;

using DuckGame;

using Azxc.UI.Controls;

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
