using Azxc.Patches;
using Azxc.UI.Controls;
using Azxc.UI.Events;
using DuckGame;

namespace Azxc.UI
{
    class WeaponsWindow : Controls.Window
    {
        private CheckBox<FancyBitmapFont> _infinityAmmo, _norecoil, _noreload, _bulletHit,
            _triggerBot, _rangeHack;

        public WeaponsWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _infinityAmmo = new CheckBox<FancyBitmapFont>("Infinite Ammo",
                "Endless ammo for most weapons.", Azxc.GetCore().GetUI().font);
            _infinityAmmo.onChecked += InfiniteAmmo_Checked; AddItem(_infinityAmmo);

            _norecoil = new CheckBox<FancyBitmapFont>("No Recoil",
                "Disable kickback on shot.", Azxc.GetCore().GetUI().font);
            _norecoil.onChecked += NoRecoil_Checked; AddItem(_norecoil);

            _noreload = new CheckBox<FancyBitmapFont>("No Reload",
                "No delay between shots for most weapons.", Azxc.GetCore().GetUI().font);
            _noreload.onChecked += NoReload_Checked; AddItem(_noreload);

            _bulletHit = new CheckBox<FancyBitmapFont>("Bullet Hit",
                "Bullets fly through walls.", Azxc.GetCore().GetUI().font);
            _bulletHit.onChecked += BulletHit_Checked; AddItem(_bulletHit);

            _rangeHack = new CheckBox<FancyBitmapFont>("Range Hack",
                "Weapons maximum range increased.", Azxc.GetCore().GetUI().font);
            _rangeHack.onChecked += RangeHack_Checked; AddItem(_rangeHack);

            _triggerBot = new CheckBox<FancyBitmapFont>("TriggerBot",
                "Automatically shoot at ducks.", Azxc.GetCore().GetUI().font);
            _triggerBot.onChecked += TriggerBot_Checked; AddItem(_triggerBot);
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
                    "Bullets hit all physical objects.", Azxc.GetCore().GetUI().font);
                physicsObjects.onChecked += PhysicsObjects_Checked;
                RadioBox<FancyBitmapFont> onlyDucks = new RadioBox<FancyBitmapFont>("Ducks",
                    "Bullets hit only ducks.", Azxc.GetCore().GetUI().font);
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
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
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
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            TriggerBot.HookAndToggle(checkBox.isChecked);
        }
    }
}
