using Azxc.UI.Events;
using System;

namespace Azxc.UI.Controls
{
    public interface IClickable
    {
        event EventHandler<ControlEventArgs> onClicked;

        void Click();
    }
}
