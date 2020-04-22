using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.UI.Events;

namespace Azxc.UI.Controls
{
    public class Button<T> : Label<T>, IAutoUpdate, IClickable, ITooltip, ISelect
    {
        public bool showToolTip { get; set; }
        public string toolTipText { get; set; }

        public bool selected { get; set; }

        public Button(string text, T font) : base(text, font)
        {
            toolTipText = "";
            showToolTip = false;
            indent = Vec2.One;
        }

        public Button(string text, string toolTipText, T font) : base(text, font)
        {
            this.toolTipText = toolTipText;
            showToolTip = true;
            indent = Vec2.One;
        }

        public Button(string text, string toolTipText, T font, Vec2 position) : base(text, font, position)
        {
            this.toolTipText = toolTipText;
            showToolTip = true;
            indent = Vec2.One;
        }

        public override void Update()
        {
            width = GetWidth() + indent.x * 2;
            height = characterHeight * GetScale().y + indent.y * 2;
        }

        public virtual void DrawTooltip()
        {
            float toolTipIndent = 4f;
            Vec2 start = new Vec2(x + width + toolTipIndent, y);
            MethodInfo getWidth = AccessTools.Method(typeof(T), "GetWidth");
            float toolTipWidth = (float)getWidth.Invoke(font, new object[] { toolTipText, false });
            Vec2 end = new Vec2(toolTipWidth + indent.x * 2, characterHeight * GetScale().y + indent.y * 2);

            Graphics.DrawRect(start, start + end, Color.Black * 0.5f, 1f);

            MethodInfo draw = AccessTools.Method(typeof(T), "Draw",
                new Type[] { typeof(string), typeof(Vec2), typeof(Color), typeof(Depth), typeof(bool) });
            draw.Invoke(font, new object[] { toolTipText, start + indent, Color.White, new Depth(1f), true });
        }

        public override void Draw()
        {
            if (selected && showToolTip && Azxc.core.uiManager.interact.activeWindow == parent)
                DrawTooltip();

            Graphics.DrawRect(position, position + size,
                selected ? Color.DarkSlateGray : new Color(17, 39, 39), 0.5f);

            // Draw text itself
            MethodInfo draw = AccessTools.Method(typeof(T), "Draw",
                new Type[] { typeof(string), typeof(Vec2), typeof(Color), typeof(Depth), typeof(bool) });
            draw.Invoke(font, new object[] { text, position + indent, Color.White, new Depth(1f), true });
        }

        public virtual void Click()
        {
            OnClicked(new ControlEventArgs(this));
        }

        public event EventHandler<ControlEventArgs> onClicked;
        protected void OnClicked(ControlEventArgs e)
        {
            onClicked?.Invoke(this, e);
        }

        public void Select()
        {
            OnSelected(new ControlEventArgs(this));
        }

        public event EventHandler<ControlEventArgs> onSelected;
        protected void OnSelected(ControlEventArgs e)
        {
            onSelected?.Invoke(this, e);
        }
    }
}
