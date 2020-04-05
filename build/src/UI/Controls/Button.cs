using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

namespace Azxc.UI.Controls
{
    public class Button<T> : Label<T>, IAutoUpdate, ISelect
    {
        public bool selected { get; set; }

        public Button(string text, T font) : base(text, font)
        {
            indent = Vec2.One;
        }

        public Button(string text, T font, Vec2 position) : base(text, font, position)
        {
            indent = Vec2.One;
        }

        public override void Update()
        {
            width = GetWidth() + indent.x * 2;
            height = characterHeight * GetScale().y + indent.y * 2;
        }

        public override void Draw()
        {
            Graphics.DrawRect(position, position + size, selected ? Color.DarkSlateGray : new Color(17, 39, 39), 1f);
            // Draw text itself
            MethodInfo draw = AccessTools.Method(typeof(T), "Draw", new Type[] { typeof(string), typeof(Vec2), typeof(Color), typeof(Depth), typeof(bool) });
            draw.Invoke(font, new object[] { text, position + indent, Color.White, new Depth(1f), true });
        }
    }
}
