using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

namespace Azxc.UI.Controls
{
    public class Separator : Control, IAutoUpdate, IIndent
    {
        public Vec2 indent { get; set; }

        public Separator()
        {
            indent = new Vec2(0.5f, 1f);
            height = 0.5f;
        }

        public virtual void Update()
        {
            Controls.Window activeWindow = Azxc.core.uiManager.interact.activeWindow;
            if (activeWindow != null)
                width = activeWindow.width - activeWindow.indent.x * 4;
        }

        public override void Draw()
        {
            Graphics.DrawRect(position, position + size, Color.DarkSlateGray, 0.5f);
        }
    }
}
