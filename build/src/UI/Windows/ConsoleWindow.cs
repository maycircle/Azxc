using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.UI.Controls;
using Azxc.UI.Events;

namespace Azxc.UI
{
    class ConsoleWindow : Controls.Window
    {
        private Controls.Window killWindow;
        public Expander<FancyBitmapFont> killCmd;

        public ConsoleWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) : base(position, sizeMode)
        {
            killWindow = new Controls.Window(position, SizeModes.Flexible);

            killCmd = new Expander<FancyBitmapFont>(killWindow, "Kill", "Kill any player.",
                Azxc.core.uiManager.font);
            killCmd.onExpanded += KillCmd_Expanded;

            Prepare();
        }

        public void Prepare()
        {
            AddItem(killCmd);
        }

        private void KillCmd_Expanded(object sender, ControlEventArgs e)
        {
            killWindow.Clear();
            foreach (Profile profile in Profiles.all)
            {
                if (profile != null)
                {
                    Button<FancyBitmapFont> player = new Button<FancyBitmapFont>(profile.name,
                        Azxc.core.uiManager.font);
                    player.onClicked += KillPlayer_Clicked;
                    killWindow.AddItem(player);
                }
            }
        }

        private void KillPlayer_Clicked(object sender, ControlEventArgs e)
        {
            Button<FancyBitmapFont> button = e.item as Button<FancyBitmapFont>;
            foreach (Profile profile in Profiles.all)
            {
                if (profile.name == button.text && profile.duck != null)
                    profile.duck.Kill(new DTIncinerate(null));
            }
        }
    }
}
