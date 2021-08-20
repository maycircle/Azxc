using Azxc.UI.Controls;
using DuckGame;

namespace Azxc.UI
{
    class MainWindow : Controls.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AddItem(new Expander(new WeaponsWindow(), "Weapons",
                "Weapon(s) hacks."));
            AddItem(new Expander(new ConsoleWindow(), "Commands",
                "GUI for console commands."));
            AddItem(new Expander(new ArcadeWindow(), "Arcade",
                "Challenges-related stuff."));
            AddItem(new Expander(new NetworkWindow(), "Network",
                "(Duck)Networking-related stuff."));
            AddItem(new Expander(new FunWindow(), "Fun",
                "Stuff to show off with. Lots of glitches, have fun :)"));
            AddItem(new Expander(new MiscWindow(), "Misc",
                "Random stuff."));
        }
    }
}
