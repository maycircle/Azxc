using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.UI.Controls;
using Azxc.UI.Events;
using Azxc.Hacks;

namespace Azxc.UI
{
    class WeaponsWindow : Controls.Window
    {
        public CheckBox<FancyBitmapFont> infinityAmmo, norecoil;

        public WeaponsWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) : base(position, sizeMode)
        {
            infinityAmmo = new CheckBox<FancyBitmapFont>("Infinite Ammo",
                "Most weapons ammo's are now endless.", Azxc.core.uiManager.font);
            infinityAmmo.onChecked += InfiniteAmmo_Checked;

            norecoil = new CheckBox<FancyBitmapFont>("No Recoil",
                "Disable kickback after shot.", Azxc.core.uiManager.font);
            norecoil.onChecked += NoRecoil_Checked;

            Prepare();
        }

        public void Prepare()
        {
            AddItem(infinityAmmo);
            AddItem(norecoil);
        }

        private void InfiniteAmmo_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            InfiniteAmmo.enabled = checkBox.isChecked;

            MethodInfo original = typeof(Gun).GetMethod("Reload");

            // Patch if it has no patches
            if (Azxc.core.harmony.GetPatchInfo(original) == null)
            {
                Azxc.core.harmony.Patch(typeof(Gun).GetMethod("Reload"),
                    transpiler: new HarmonyMethod(typeof(Hacks.InfiniteAmmo), "Transpiler"));
            }
        }

        private void NoRecoil_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            NoRecoil.enabled = checkBox.isChecked;

            MethodInfo original = typeof(Gun).GetMethod("ApplyKick");

            if (Azxc.core.harmony.GetPatchInfo(original) == null)
            {
                Azxc.core.harmony.Patch(typeof(Gun).GetMethod("ApplyKick"),
                    prefix: new HarmonyMethod(typeof(Hacks.NoRecoil), "Prefix"));
            }
        }
    }
}
