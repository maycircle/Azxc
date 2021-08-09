using Azxc.UI.Events;
using DuckGame;
using System.Collections.Generic;
using System.Linq;

namespace Azxc.UI.Controls.Misc
{
    class ProfileSelector<T> : Expander, ISelector where T : Command
    {
        private AbstractExpander<T> _selectedItem;

        public ProfileSelector(string text)
        {
            this.text = text;
        }

        public ProfileSelector(string text, string toolTipText)
        {
            this.text = text;
            this.toolTipText = toolTipText;
            showToolTip = true;
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
                    AbstractExpander<T> player = new AbstractExpander<T>(profile.name);
                    player.onExpanded += Player_Expanded;
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

        private void Player_Expanded(object sender, ControlEventArgs e)
        {
            AbstractExpander<T> item = (AbstractExpander<T>)e.item;
            ((Command)item.window).AddSelector(this);
            _selectedItem = item;
        }
    }
}
