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
    public class CheckBox<T> : Button<T>
    {
        public bool isChecked { get; set; }

        private Vec2 checkSize = new Vec2(3f);

        public CheckBox(string text, T font, bool isChecked = false) : base(text, font)
        {
            this.isChecked = isChecked;
        }

        public CheckBox(string text, string toolTipText, T font, bool isChecked = false) : 
            base(text, toolTipText, font)
        {
            this.isChecked = isChecked;
        }

        public CheckBox(string text, string toolTipText, T font, Vec2 position, bool isChecked = false):
            base(text, toolTipText, font, position)
        {
            this.isChecked = isChecked;
        }

        public override void Update()
        {
            width = GetWidth() + indent.x * 2 + checkSize.x;
            height = characterHeight * GetScale().y + indent.y * 2;
        }

        public override void Draw()
        {
            base.Draw();

            Vec2 start = new Vec2(position.x + size.x - checkSize.x - indent.x,
                position.y + size.y / 2 - checkSize.y / 2);

            float border = 0.5f;

            Graphics.DrawRect(start, start + checkSize, Color.White, 1f, false, border);
            if (isChecked)
            {
                Graphics.DrawRect(start + new Vec2(border * 2),
                    start + checkSize - new Vec2(border * 2), Color.White, 1f);
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