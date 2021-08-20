using Azxc.Patches;
using Azxc.UI.Controls;
using Azxc.UI.Events;
using DuckGame;

namespace Azxc.UI
{
    class WeaponsWindow : Controls.Window
    {
        private CheckBox _infinityAmmo, _norecoil, _noreload, _bulletHit,
            _triggerBot, _rangeHack;

        public WeaponsWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _infinityAmmo = new CheckBox("Infinite Ammo", "Endless ammo for most weapons.");
            _infinityAmmo.onChecked += InfiniteAmmo_Checked; AddItem(_infinityAmmo);

            _norecoil = new CheckBox("No Recoil", "Disable kickback on shot.");
            _norecoil.onChecked += NoRecoil_Checked; AddItem(_norecoil);

            _noreload = new CheckBox("No Reload", "No delay between shots for most weapons.");
            _noreload.onChecked += NoReload_Checked; AddItem(_noreload);

            _bulletHit = new CheckBox("Bullet Hit", "Bullets fly through walls.");
            _bulletHit.onChecked += BulletHit_Checked; AddItem(_bulletHit);

            _rangeHack = new CheckBox("Range Hack", "Weapons maximum range increased.");
            _rangeHack.onChecked += RangeHack_Checked; AddItem(_rangeHack);

            _triggerBot = new CheckBox("TriggerBot", "Automatically shoot at ducks.");
            _triggerBot.onChecked += TriggerBot_Checked; AddItem(_triggerBot);
        }

        private void InfiniteAmmo_Checked(object sender, ControlEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            InfiniteAmmo.HookAndToggle(checkBox.isChecked);
        }

        private void NoRecoil_Checked(object sender, ControlEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            NoRecoil.HookAndToggle(checkBox.isChecked);
        }

        private void NoReload_Checked(object sender, ControlEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            NoReload.HookAndToggle(checkBox.isChecked);
        }

        private void BulletHit_Checked(object sender, ControlEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            BulletHit.HookAndToggle(checkBox.isChecked);

            if (checkBox.isChecked)
            {
                Vec2 propertiesPos = new Vec2(checkBox.x + checkBox.width + checkBox.indent.x * 3,
                    checkBox.y - 0.5f * 3);
                Controls.Window properties = new Controls.Window(propertiesPos,
                    SizeModes.Flexible);

                RadioBox physicsObjects = new RadioBox("Physics objects", "Bullets hit all physical objects.");
                physicsObjects.onChecked += PhysicsObjects_Checked;

                RadioBox onlyDucks = new RadioBox("Ducks", "Bullets hit only ducks.");
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

        private void RangeHack_Checked(object sender, ControlEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            RangeHack.HookAndToggle(checkBox.isChecked);
        }

        private void PhysicsObjects_Checked(object sender, ControlEventArgs e)
        {
            BulletHit.trigger = typeof(PhysicsObject);
        }

        private void OnlyDucks_Checked(object sender, ControlEventArgs e)
        {
            BulletHit.trigger = typeof(Duck);
        }

        private void TriggerBot_Checked(object sender, ControlEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            TriggerBot.HookAndToggle(checkBox.isChecked);
        }
    }
}
