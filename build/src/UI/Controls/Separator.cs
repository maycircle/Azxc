using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

namespace Azxc.UI.Controls
{
    public class Separator : Control, IHasIndent
    {
        public Vec2 indent { get; set; }

        public Separator()
        {
            indent = new Vec2(1.5f, 1.0f);
            height = 0.5f;
        }

        public override void Draw()
        {
            Graphics.DrawRect(position, position + size, Color.DarkSlateGray, 0.5f);
        }
    }
}
