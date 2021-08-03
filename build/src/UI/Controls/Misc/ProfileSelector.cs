using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DuckGame;

using Azxc.UI.Events;

namespace Azxc.UI.Controls.Misc
{
    class ProfileSelector<T, U> : Expander<T>, ISelector where U : Command
    {
        private AbstractExpander<T, U> _selected;

        public ProfileSelector(string text, T font) : base(font)
        {
            this.text = text;
        }

        public ProfileSelector(string text, string toolTipText, T font) :
            base(font)
        {
            this.text = text;
            this.toolTipText = toolTipText;
            showToolTip = true;
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
                    AbstractExpander<T, U> player = new AbstractExpander<T, U>(profile.name, font);
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
                if (profile.name == _selected.text && profile.duck != null)
                    return profile;
            }

            throw new KeyNotFoundException("Couldn't find the requested profile");
        }

        private void Player_Expanded(object sender, ControlEventArgs e)
        {
            AbstractExpander<T, U> item = (AbstractExpander<T, U>)e.item;
            ((Command)item.window).AddSelector(this);
            _selected = item;
        }
    }
}
