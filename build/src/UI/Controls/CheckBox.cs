using Azxc.UI.Events;
using DuckGame;
using System;

namespace Azxc.UI.Controls
{
    public class CheckBox : Button
    {
        private readonly Vec2 CheckSize = new Vec2(3f);

        public bool isChecked { get; set; }

        public CheckBox(string text, bool isChecked = false) : base(text)
        {
            this.isChecked = isChecked;
        }

        public CheckBox(string text, string toolTipText, bool isChecked = false) :
            base(text, toolTipText)
        {
            this.isChecked = isChecked;
        }

        public CheckBox(string text, string toolTipText, Vec2 position,
            bool isChecked = false) : base(text, toolTipText, position)
        {
            this.isChecked = isChecked;
        }

        public override void Update()
        {
            width = font.GetWidth(text) + indent.x * 2 + CheckSize.x + 2f;
            height = font.characterHeight * font.scale.y + indent.y * 2;
        }

        public override void Draw()
        {
            base.Draw();

            Vec2 start = new Vec2(x + width - CheckSize.x - indent.x,
                y + height / 2 - CheckSize.y / 2);

            float border = 0.5f;

            Graphics.DrawRect(start, start + CheckSize, Color.White, 1f, false, border);
            if (isChecked)
            {
                Graphics.DrawRect(start + new Vec2(border * 2),
                    start + CheckSize - new Vec2(border * 2), Color.White, 1f);
            }
        }

        public override void Click()
        {
            base.Click();

            Check();
        }

        public virtual void Check()
        {
            isChecked = !isChecked;
            OnChecked(new ControlEventArgs(this));
        }

        public event EventHandler<ControlEventArgs> onChecked;
        protected void OnChecked(ControlEventArgs e)
        {
            onChecked?.Invoke(this, e);
        }
    }
}
