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
        public CheckBox<FancyBitmapFont> infinityAmmo, norecoil, noreload, bulletHit;

        public WeaponsWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) : base(position, sizeMode)
        {
            infinityAmmo = new CheckBox<FancyBitmapFont>("Infinite Ammo",
                "Most weapons ammo's are now endless.", Azxc.core.uiManager.font);
            infinityAmmo.onChecked += InfiniteAmmo_Checked;

            norecoil = new CheckBox<FancyBitmapFont>("No Recoil",
                "Disable kickback after shot.", Azxc.core.uiManager.font);
            norecoil.onChecked += NoRecoil_Checked;

            noreload = new CheckBox<FancyBitmapFont>("No Reload",
                "No delay between shots for most weapons.", Azxc.core.uiManager.font);
            noreload.onChecked += NoReload_Checked;

            bulletHit = new CheckBox<FancyBitmapFont>("Bullet Hit",
                "Bullets shoot through walls.", Azxc.core.uiManager.font);
            bulletHit.onChecked += BulletHit_Checked;

            Prepare();
        }

        public void Prepare()
        {
            AddItem(infinityAmmo);
            AddItem(norecoil);
            AddItem(noreload);
            AddItem(bulletHit);
        }

        private void InfiniteAmmo_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            InfiniteAmmo.enabled = checkBox.isChecked;

            MethodInfo original = typeof(Gun).GetMethod("Reload");

            // Patch if it has no patches
            if (Azxc.core.harmony.GetPatchInfo(original) == null)
            {
                Azxc.core.harmony.Patch(original,
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
                Azxc.core.harmony.Patch(original,
                    prefix: new HarmonyMethod(typeof(Hacks.NoRecoil), "Prefix"));
            }
        }

        private void NoReload_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            NoReload.enabled = checkBox.isChecked;

            MethodInfo original = typeof(Gun).GetMethod("Fire");

            if (Azxc.core.harmony.GetPatchInfo(original) == null)
            {
                Azxc.core.harmony.Patch(original,
                    postfix: new HarmonyMethod(typeof(Hacks.NoReload), "Postfix"));
                Azxc.core.harmony.Patch(typeof(Gun).GetMethod("OnHoldAction"),
                    transpiler: new HarmonyMethod(typeof(Hacks.NoReload), "Transpiler"));
            }
        }

        private void BulletHit_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            BulletHit.enabled = checkBox.isChecked;

            MethodInfo original = AccessTools.Method(typeof(Bullet), "RaycastBullet");

            if (Azxc.core.harmony.GetPatchInfo(original) == null)
            {
                Azxc.core.harmony.Patch(original,
                    transpiler: new HarmonyMethod(typeof(Hacks.BulletHit), "Transpiler"));
            }

            if (checkBox.isChecked)
            {
                Controls.Window properties = new Controls.Window(new Vec2(checkBox.x + checkBox.width + checkBox.indent.x * 3,
                    checkBox.y - 0.5f * 3), SizeModes.Flexible);
                RadioBox<FancyBitmapFont> physicsObjects = new RadioBox<FancyBitmapFont>("Physics objects",
                    "Bullets concern only physical objects.", Azxc.core.uiManager.font);
                physicsObjects.onChecked += PhysicsObjects_Checked;
                RadioBox<FancyBitmapFont> onlyDucks = new RadioBox<FancyBitmapFont>("Ducks",
                    "Bullets concern only ducks.", Azxc.core.uiManager.font);
                onlyDucks.onChecked += OnlyDucks_Checked;

                properties.AddItem(physicsObjects);
                properties.AddItem(onlyDucks);

                if (BulletHit.trigger == typeof(PhysicsObject))
                    physicsObjects.isChecked = true;
                else
                    onlyDucks.isChecked = true;

                properties.Show();
            }
        }

        private void PhysicsObjects_Checked(object sender, ControlEventArgs e)
        {
            BulletHit.trigger = typeof(PhysicsObject);
        }

        private void OnlyDucks_Checked(object sender, ControlEventArgs e)
        {
            BulletHit.trigger = typeof(Duck);
        }
    }
}
