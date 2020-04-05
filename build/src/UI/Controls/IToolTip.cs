using System;

using DuckGame;

namespace Azxc.UI.Controls
{
    public interface ITooltip
    {
        bool showToolTip { get; set; }
        string toolTipText { get; set; }

        void DrawTooltip();
    }
}
