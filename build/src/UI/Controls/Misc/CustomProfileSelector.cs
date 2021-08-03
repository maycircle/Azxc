using Azxc.UI.Events;
using DuckGame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azxc.UI.Controls.Misc
{
    class CustomProfileSelector<T> : Expander<T>, ISelector
    {
        private Button<T> _selected;

        public CustomProfileSelector(string text, string toolTipText, T font) :
            base(font)
        {
            this.text = text;
            this.toolTipText = toolTipText;
            showToolTip = true;
        }

        public void AddItem(Control item)
        {
            window.AddItem(item);
        }

        public override void Click()
        {
            window = new Window();

            window.AddItem(new Label<T>("Profiles:", font));
            if (Profiles.activeNonSpectators.Count == 0)
            {
                window.AddItem(new Label<T>("No available profiles!", font));
                return;
            }
            foreach (Profile profile in Profiles.activeNonSpectators.Where(x => x.duck != null))
            {
                if (profile != null && profile.duck != null)
                {
                    Button<T> player = new Button<T>(profile.name, font);
                    player.onClicked += Player_Clicked;
                    window.AddItem(player);
                }
            }

            base.Click();
        }

        public object GetValue()
        {
            foreach (Profile profile in Profiles.all)
            {
                if (profile.name == _selected.text && profile.duck != null)
                    return profile;
            }

            throw new KeyNotFoundException("Couldn't find the requested profile");
        }

        public event EventHandler<ControlEventArgs> onCommandRun;
        protected void OnCommandRun(ControlEventArgs e)
        {
            onCommandRun?.Invoke(this, e);
        }

        private void Player_Clicked(object sender, ControlEventArgs e)
        {
            _selected = (Button<T>)e.item;
            OnCommandRun(e);
        }
    }
}
