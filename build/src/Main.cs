using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.Bindings;
using Azxc.UI;

namespace Azxc
{
    public class Azxc : Mod
    {
        public static AzxcCore core;

        public Azxc()
        {
            core = new AzxcCore();
            // Probably, in future i will create a special AutoPatcher for this
            core.harmony.PatchAll();
            core.Prepare();
        }

        protected override void OnPostInitialize()
        {
            // Call OnTick on every tick. Some sort of Update, but actually no
            core.harmony.Patch(typeof(RockWeather).GetMethod("TickWeather"),
                postfix: new HarmonyMethod(typeof(Azxc), "OnTick"));
        }

        public static void OnTick()
        {
            core.bindingManager.Update();
            core.uiManager.Update();
        }
    }
}
