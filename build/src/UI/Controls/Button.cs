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
    public class Button<T> : Label<T>, IAutoUpdate, IClickable, IHasTooltip, ISelectable
    {
        public bool showToolTip { get; set; }
        public string toolTipText { get; set; }

        public bool selected { get; set; }

        protected Button(T font) : base(font)
        {
            toolTipText = "";
            showToolTip = false;
            indent = Vec2.One * 1.5f;
        }

        public Button(string text, T font) : base(text, font)
        {
            toolTipText = "";
            showToolTip = false;
            indent = Vec2.One * 1.5f;
        }

        public Button(string text, string toolTipText, T font) : base(text, font)
        {
            this.toolTipText = toolTipText;
            showToolTip = true;
            indent = Vec2.One * 1.5f;
        }

        public Button(string text, string toolTipText, T font, Vec2 position) :
            base(text, font, position)
        {
            this.toolTipText = toolTipText;
            showToolTip = true;
            indent = Vec2.One * 1.5f;
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
            Vec2 toolTipSize = GetLinesSize(toolTipText);
            Vec2 end = new Vec2(toolTipSize.x + indent.x * 2,
                toolTipSize.y + indent.y * 2);

            // Draw tooltip blending background
            Graphics.DrawRect(start, start + end, Color.Black * 0.5f, 1f);
            // Draw tooltip text
            DrawText(toolTipText, start + indent, Color.White, new Depth(1.0f), true);
        }

        public override void Draw()
        {
            if (selected && showToolTip && Azxc.GetCore().GetUI().interact.activeWindow == parent)
                DrawTooltip();

            // Draw button background
            Graphics.DrawRect(position, position + size,
                selected ? Color.DarkSlateGray : new Color(17, 39, 39), 0.5f);
            // Draw button text
            DrawText(text, position + indent, Color.White, new Depth(1.0f), true);
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
