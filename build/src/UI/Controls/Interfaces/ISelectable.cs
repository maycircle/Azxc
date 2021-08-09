using Azxc.UI.Events;
using System;

namespace Azxc.UI.Controls
{
    public interface ISelectable
    {
        bool isSelected { get; set; }

        event EventHandler<ControlEventArgs> onSelected;

        void Select();
    }
}
