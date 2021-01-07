using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.UI;
using Azxc.UI.Controls;

namespace Azxc
{
    public class Azxc : ClientMod
    {
        public static AzxcCore core;

        public Azxc()
        {
            core = new AzxcCore();
            // Probably, in future I will create a special AutoPatcher for this
            core.harmony.PatchAll();
            core.Prepare();
        }

        protected override void OnPostInitialize()
        {
            // OnTick gets called for the entire time game running, so I will use it as some sort
            // of "update"
            core.harmony.Patch(typeof(RockWeather).GetMethod("TickWeather"),
                postfix: new HarmonyMethod(typeof(Azxc), "OnTick"));

            MainWindow mainWindow = new MainWindow(new Vec2(5f), SizeModes.Flexible);
            mainWindow.Show();
        }

        public static void OnTick()
        {
            core.bindingManager.Update();
            core.uiManager.Update();
        }
    }
}
