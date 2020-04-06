using System;

using DuckGame;

using Azxc.UI.Events;

namespace Azxc.UI.Controls
{
    public interface IClickable
    {
        event EventHandler<ControlEventArgs> onClicked;

        void Click();
    }
}
