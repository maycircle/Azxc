using System;

namespace Azxc.UI
{
    [Flags]
    public enum UserInterfaceState
    {
        /// <summary>
        /// UI is visually visible.
        /// </summary>
        Open = 1,

        /// <summary>
        /// UI is fully updating all controls.
        /// </summary>
        Enabled = 2,

        /// <summary>
        /// UI is only updating ForceUpdate controls.
        /// </summary>
        Freeze = 4
    }
}
