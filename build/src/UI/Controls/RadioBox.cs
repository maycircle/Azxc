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
    public class RadioBox<T> : CheckBox<T>
    {
        private Vec2 checkSize = new Vec2(3f);

        public RadioBox(string text, T font, bool isChecked = false) : base(text, font)
        {
            this.isChecked = isChecked;
        }

        public RadioBox(string text, string toolTipText, T font, bool isChecked = false) : 
            base(text, toolTipText, font)
        {
            this.isChecked = isChecked;
        }

        public RadioBox(string text, string toolTipText, T font, Vec2 position,
            bool isChecked = false) : base(text, toolTipText, font, position)
        {
            this.isChecked = isChecked;
        }

        public override void Check()
        {
            if (isChecked)
                return;
            isChecked = true;
            OnChecked(new ControlEventArgs(this));
            foreach (RadioBox<T> radioBox in
                Azxc.core.uiManager.interact.activeWindow.OfType<RadioBox<T>>())
            {
                if (radioBox != this)
                    radioBox.isChecked = false;
            }
        }
    }
}
