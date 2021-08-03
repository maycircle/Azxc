using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.UI.Events;
using Azxc.UI.Controls;

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
            AddItem(new Expander<FancyBitmapFont>(new WeaponsWindow(), "Weapons",
                "Weapon(s) hacks.", Azxc.GetCore().GetUI().font));
            AddItem(new Expander<FancyBitmapFont>(new ConsoleWindow(), "Commands",
                "GUI for console commands.", Azxc.GetCore().GetUI().font));
            AddItem(new Expander<FancyBitmapFont>(new ArcadeWindow(), "Arcade",
                "Challenges-related stuff.", Azxc.GetCore().GetUI().font));
            AddItem(new Expander<FancyBitmapFont>(new NetworkWindow(), "Network",
                "(Duck)Networking-related stuff.", Azxc.GetCore().GetUI().font));
            AddItem(new Expander<FancyBitmapFont>(new FunWindow(), "Fun",
                "Stuff to show off with. Lots of glitches, have fun :)", 
                Azxc.GetCore().GetUI().font));
            AddItem(new Expander<FancyBitmapFont>(new MiscWindow(), "Misc",
                "Random stuff.", Azxc.GetCore().GetUI().font));
        }
    }
}
