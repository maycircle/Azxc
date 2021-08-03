using Azxc.UI.Events;
using DuckGame;
using System;

namespace Azxc.UI.Controls
{
    public class Expander<T> : Button<T>
    {
        protected Window window;

        private Vec2 arrowFix = new Vec2(4f, 1.6f);

        protected Expander(T font) : base(font)
        {
            window = new Window();
        }

        public Expander(Window window, string text, T font) : base(text, font)
        {
            this.window = window;
        }

        public Expander(Window window, string text, string toolTipText, T font) :
            base(text, toolTipText, font)
        {
            this.window = window;
        }

        public Expander(Window window, string text, string toolTipText, T font, Vec2 position) :
            base(text, toolTipText, font, position)
        {
            this.window = window;
        }

        public override void Update()
        {
            width = GetWidth() + indent.x * 2 + 5f;
            height = characterHeight * GetScale().y + indent.y * 2;
        }

        public override void Draw()
        {
            base.Draw();

            Sprite arrow = new Sprite("contextArrowRight");
            arrow.scale = new Vec2(0.5f);
            Vec2 arrowPosition = new Vec2(x + width - indent.x, y + height / 2) - arrowFix;
            Graphics.Draw(arrow, arrowPosition.x, arrowPosition.y, 1f);
        }

        public override void Click()
        {
            base.Click();

            Expand();
        }

        public virtual void Expand()
        {
            float windowIndent = 2f;
            window.position = new Vec2(x + width + indent.x + windowIndent, y - 0.5f * 3);
            window.Show();
            OnExpanded(new ControlEventArgs(this));
        }

        public event EventHandler<ControlEventArgs> onExpanded;
        protected void OnExpanded(ControlEventArgs e)
        {
            onExpanded?.Invoke(this, e);
        }
    }
}
