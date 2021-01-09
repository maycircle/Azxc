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
        private CheckBox<FancyBitmapFont> infinityAmmo, norecoil, noreload, bulletHit;

        public WeaponsWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) : base(position, sizeMode)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            infinityAmmo = new CheckBox<FancyBitmapFont>("Infinite Ammo",
                "Endless ammo for most weapons.", Azxc.core.uiManager.font);
            infinityAmmo.onChecked += InfiniteAmmo_Checked; AddItem(infinityAmmo);

            norecoil = new CheckBox<FancyBitmapFont>("No Recoil",
                "Disable kickback on shot.", Azxc.core.uiManager.font);
            norecoil.onChecked += NoRecoil_Checked; AddItem(norecoil);

            noreload = new CheckBox<FancyBitmapFont>("No Reload",
                "No delay between shots for most weapons.", Azxc.core.uiManager.font);
            noreload.onChecked += NoReload_Checked; AddItem(noreload);

            bulletHit = new CheckBox<FancyBitmapFont>("Bullet Hit",
                "Bullets fly through walls.", Azxc.core.uiManager.font);
            bulletHit.onChecked += BulletHit_Checked; AddItem(bulletHit);
        }

        private void InfiniteAmmo_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            InfiniteAmmo.HookAndToggle(checkBox.isChecked);
        }

        private void NoRecoil_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            NoRecoil.HookAndToggle(checkBox.isChecked);
        }

        private void NoReload_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            NoReload.HookAndToggle(checkBox.isChecked);
        }

        private void BulletHit_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            BulletHit.HookAndToggle(checkBox.isChecked);

            if (checkBox.isChecked)
            {
                Controls.Window properties = new Controls.Window(new Vec2(checkBox.x + checkBox.width + checkBox.indent.x * 3,
                    checkBox.y - 0.5f * 3), SizeModes.Flexible);
                RadioBox<FancyBitmapFont> physicsObjects = new RadioBox<FancyBitmapFont>("Physics objects",
                    "Bullets hit all physical objects.", Azxc.core.uiManager.font);
                physicsObjects.onChecked += PhysicsObjects_Checked;
                RadioBox<FancyBitmapFont> onlyDucks = new RadioBox<FancyBitmapFont>("Ducks",
                    "Bullets hit only ducks.", Azxc.core.uiManager.font);
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
