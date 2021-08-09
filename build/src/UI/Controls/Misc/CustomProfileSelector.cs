using Azxc.UI.Events;
using DuckGame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azxc.UI.Controls.Misc
{
    class CustomProfileSelector : Expander, ISelector
    {
        private Button _selectedItem;

        public CustomProfileSelector(string text, string toolTipText)
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

            window.AddItem(new Label("Profiles:"));
            if (Profiles.activeNonSpectators.Count == 0)
            {
                window.AddItem(new Label("No available profiles!"));
                return;
            }
            foreach (Profile profile in Profiles.activeNonSpectators.Where(x => x.duck != null))
            {
                if (profile != null && profile.duck != null)
                {
                    Button player = new Button(profile.name);
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
                if (profile.name == _selectedItem.text && profile.duck != null)
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
            _selectedItem = (Button)e.item;
            OnCommandRun(e);
        }
    }
}
