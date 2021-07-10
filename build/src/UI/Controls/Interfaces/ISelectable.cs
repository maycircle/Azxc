using System;

using Azxc.UI.Events;

namespace Azxc.UI.Controls
{
    public interface ISelectable
    {
        bool selected { get; set; }

        event EventHandler<ControlEventArgs> onSelected;

        void Select();
    }
}
