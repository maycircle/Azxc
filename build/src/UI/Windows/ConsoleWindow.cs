using Azxc.UI.Controls;
using Azxc.UI.Controls.Misc;
using Azxc.UI.Events;
using DuckGame;
using System.Linq;

namespace Azxc.UI
{
    class ConsoleWindow : Controls.Window
    {
        private Button<FancyBitmapFont> _skipCmd;

        public ConsoleWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            CustomProfileSelector<FancyBitmapFont> killCommand = new CustomProfileSelector<FancyBitmapFont>(
                "Kill", "Kill player.", Azxc.GetCore().GetUI().font);
            killCommand.onExpanded += KillCommand_Expanded;
            killCommand.onCommandRun += KillCommand_CommandRun;
            AddItem(killCommand);

            AddItem(new ProfileSelector<FancyBitmapFont, CallCommand>("Call",
                "Call method on player.", Azxc.GetCore().GetUI().font));
            AddItem(new ProfileSelector<FancyBitmapFont, GiveCommand>("Give",
                "Give item to player.", Azxc.GetCore().GetUI().font));

            _skipCmd = new Button<FancyBitmapFont>("Skip", "Skip current level |RED|(Host only).",
                Azxc.GetCore().GetUI().font);
            _skipCmd.onClicked += Skip_Clicked; AddItem(_skipCmd);
        }

        private void KillCommand_Expanded(object sender, ControlEventArgs e)
        {
            CustomProfileSelector<FancyBitmapFont> item = (CustomProfileSelector<FancyBitmapFont>)e.item;

            Controls.Window ducksWindow = new Controls.Window();
            Button<FancyBitmapFont> all = new Button<FancyBitmapFont>("All",
                "Kill all ducks.", Azxc.GetCore().GetUI().font);
            all.onClicked += DucksAll_Clicked;
            ducksWindow.AddItem(all);

            Button<FancyBitmapFont> allButLocal = new Button<FancyBitmapFont>("All but Local",
                "Kill all ducks but local |RED|(Only works in Network or Challenge Arcades).",
                Azxc.GetCore().GetUI().font);
            allButLocal.onClicked += DucksAllButLocal_Clicked;
            ducksWindow.AddItem(allButLocal);

            Expander<FancyBitmapFont> ducksExpander = new Expander<FancyBitmapFont>(
                ducksWindow, "Ducks", Azxc.GetCore().GetUI().font);
            item.AddItem(ducksExpander);
        }

        private void KillCommand_CommandRun(object sender, ControlEventArgs e)
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

        private void DucksAllButLocal_Clicked(object sender, ControlEventArgs e)
        {
            if (Level.current == null)
                return;
            foreach (Duck duck in Level.current.things.OfType<Duck>())
            {
                if (!duck.isLocal || duck is TargetDuck)
                    duck.Kill(new DTIncinerate(null));
            }
        }

        private void Skip_Clicked(object sender, ControlEventArgs e)
        {
            GameMode.Skip();
        }
    }
}
