using Azxc.UI.Controls;
using Azxc.UI.Controls.Misc;
using Azxc.UI.Events;
using DuckGame;
using System.Linq;

namespace Azxc.UI
{
    class ConsoleWindow : Controls.Window
    {
        private Button _skipCmd;

        public ConsoleWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            CustomProfileSelector killCommand = new CustomProfileSelector("Kill", "Kill player.");
            killCommand.onExpanded += KillCommand_Expanded;
            killCommand.onCommandRun += KillCommand_CommandRun;
            AddItem(killCommand);

            AddItem(new ProfileSelector<CallCommand>("Call", "Call method on player."));
            AddItem(new ProfileSelector<GiveCommand>("Give", "Give item to player."));

            _skipCmd = new Button("Skip", "Skip current level |RED|(Host only).");
            _skipCmd.onClicked += Skip_Clicked; AddItem(_skipCmd);
        }

        private void KillCommand_Expanded(object sender, ControlEventArgs e)
        {
            CustomProfileSelector item = sender as CustomProfileSelector;

            Controls.Window ducksWindow = new Controls.Window();
            Button all = new Button("All", "Kill all ducks.");
            all.onClicked += DucksAll_Clicked;
            ducksWindow.AddItem(all);

            Button allButLocal = new Button("All but Local",
                "Kill all ducks but local |RED|(Only works in Network or Challenge Arcades).");
            allButLocal.onClicked += DucksAllButLocal_Clicked;
            ducksWindow.AddItem(allButLocal);

            Expander ducksExpander = new Expander(ducksWindow, "Ducks");
            item.AddItem(ducksExpander);
        }

        private void KillCommand_CommandRun(object sender, ControlEventArgs e)
        {
            Button button = sender as Button;
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
            if ((DuckNetwork.active && DuckNetwork.localDuckIndex == DuckNetwork.hostDuckIndex) ||
                !DuckNetwork.active)
            {
                Level.current = new GameLevel(Deathmatch.RandomLevelString(GameMode.previousLevel),
                    0, false, false);
            }
        }
    }
}
