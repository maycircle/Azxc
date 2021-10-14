using Azxc.UI.Events;
using DuckGame;
using System;

namespace Azxc.UI.Controls
{
    public class Button : Label, IClickable, IHasTooltip, ISelectable
    {
        public bool showToolTip { get; set; }
        public string toolTipText { get; set; }

        public bool isSelected { get; set; }

        protected Button()
        {
            toolTipText = "";
            showToolTip = false;
            indent = Vec2.One * 1.5f;
        }

        public Button(string text) : base(text)
        {
            toolTipText = "";
            showToolTip = false;
            indent = Vec2.One * 1.5f;
        }

        public Button(string text, string toolTipText) : base(text)
        {
            this.toolTipText = toolTipText;
            showToolTip = true;
            indent = Vec2.One * 1.5f;
        }

        public Button(string text, string toolTipText, Vec2 position) :
            base(text, position)
        {
            this.toolTipText = toolTipText;
            showToolTip = true;
            indent = Vec2.One * 1.5f;
        }

        public override void Update()
        {
            width = font.GetWidth(text) + indent.x * 2;
            height = font.characterHeight * font.scale.y + indent.y * 2;
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
            font.Draw(toolTipText, start + indent, Color.White, new Depth(1.0f), true);
        }

        public override void Draw()
        {
            if (isSelected && showToolTip &&
                Azxc.GetCore().GetUI().GetInteractionManager().activeWindow == parent)
            {
                DrawTooltip();
            }

            // Draw button background
            Graphics.DrawRect(position, position + size,
                isSelected ? Color.DarkSlateGray : new Color(17, 39, 39), 0.5f);
            // Draw button text
            font.Draw(text, position + indent, Color.White, new Depth(1.0f), true);
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
