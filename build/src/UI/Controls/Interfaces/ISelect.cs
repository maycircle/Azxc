using System;

using DuckGame;

using Azxc.UI.Events;

namespace Azxc.UI.Controls
{
    public interface ISelect
    {
        bool selected { get; set; }

        event EventHandler<ControlEventArgs> onSelected;

        void Select();
    }
}
