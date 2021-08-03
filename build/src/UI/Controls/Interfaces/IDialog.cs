using Azxc.UI.Events;
using System;

namespace Azxc.UI.Controls
{
    public interface IDialog
    {
        DialogResult dialogResult { get; set; }

        event EventHandler<ControlEventArgs> onResult;

        void ThrowResult();
    }
}
