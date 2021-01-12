using System;

using Azxc.UI.Events;

namespace Azxc.UI.Controls
{
    public interface IDialog
    {
        DialogResult dialogResult { get; set; }

        event EventHandler<ControlEventArgs> onResult;

        void ThrowResult();
    }
}
