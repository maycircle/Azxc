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
            killWindow.AddItem(new Label<FancyBitmapFont>("Profiles:", Azxc.core.uiManager.font));
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
            killWindow.AddItem(new Separator());
            Controls.Window ducksWindow = new Controls.Window(Vec2.Zero, SizeModes.Flexible);

            Button<FancyBitmapFont> all = new Button<FancyBitmapFont>("All",
                "Kill all ducks.", Azxc.core.uiManager.font);
            all.onClicked += DucksAll_Clicked;
            ducksWindow.AddItem(all);

            Button<FancyBitmapFont> allButYou = new Button<FancyBitmapFont>("All but Local",
                "Basically kills all ducks but local, so only works in Network or Challenge Arcades.", Azxc.core.uiManager.font);
            allButYou.onClicked += DucksAllButYou_Clicked;
            ducksWindow.AddItem(allButYou);

            Expander<FancyBitmapFont> ducks = new Expander<FancyBitmapFont>(ducksWindow, "Ducks",
                Azxc.core.uiManager.font);
            killWindow.AddItem(ducks);
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

        private void DucksAll_Clicked(object sender, ControlEventArgs e)
        {
            if (Level.current == null)
                return;
            foreach (Duck duck in Level.current.things.OfType<Duck>())
            {
                duck.Kill(new DTIncinerate(null));
            }
        }

        private void DucksAllButYou_Clicked(object sender, ControlEventArgs e)
        {
            if (Level.current == null)
                return;
            foreach (Duck duck in Level.current.things.OfType<Duck>())
            {
                if (!duck.isLocal || duck is TargetDuck)
                    duck.Kill(new DTIncinerate(null));
            }
        }
    }
}
