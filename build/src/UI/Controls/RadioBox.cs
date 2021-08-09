using Azxc.UI.Events;
using DuckGame;
using System.Linq;

namespace Azxc.UI.Controls
{
    public class RadioBox : CheckBox
    {
        public RadioBox(string text, bool isChecked = false) : base(text)
        {
            this.isChecked = isChecked;
        }

        public RadioBox(string text, string toolTipText, bool isChecked = false) :
            base(text, toolTipText)
        {
            this.isChecked = isChecked;
        }

        public RadioBox(string text, string toolTipText, Vec2 position,
            bool isChecked = false) : base(text, toolTipText, position)
        {
            this.isChecked = isChecked;
        }

        public override void Check()
        {
            if (isChecked)
                return;

            isChecked = true;
            OnChecked(new ControlEventArgs(this));

            // Uncheck every other radiobox if this is checked
            foreach (RadioBox radioBox in
                Azxc.GetCore().GetUI().GetInteractionManager().activeWindow.OfType<RadioBox>())
            {
                if (radioBox != this)
                    radioBox.isChecked = false;
            }
        }
    }
}
