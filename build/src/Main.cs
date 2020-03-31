using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HarmonyLib;
using DuckGame;

namespace Azxc
{
    public class Azxc : Mod
    {
        public Harmony harmony;

        public Azxc()
        {
            harmony = new Harmony("harmony_ultra_unique_id");
            // Probably, in future i will create a special AutoPatcher for this
            harmony.PatchAll();
        }

        protected override void OnPostInitialize()
        {
            // Call OnTick on every tick. Some sort of Update, but actually no
            harmony.Patch(typeof(RockWeather).GetMethod("TickWeather"),
                postfix: new HarmonyMethod(typeof(Azxc), "OnTick"));
        }

        public static void OnTick()
        {

        }
    }
}
