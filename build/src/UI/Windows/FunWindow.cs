using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using DuckGame;

using Azxc.UI.Controls;
using Azxc.UI.Events;
using Azxc.Patches.Misc;

namespace Azxc.UI
{
    class FunWindow : Controls.Window
    {
        private CheckBox<FancyBitmapFont> _assaultAura, _spinMeRound;

        public FunWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) :
            base(position, sizeMode)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _assaultAura = new CheckBox<FancyBitmapFont>("Assault Aura", Azxc.GetCore().GetUI().font);
            _assaultAura.onChecked += WeaponAura_Checked;
            AddItem(_assaultAura);

            _spinMeRound = new CheckBox<FancyBitmapFont>("Spin Me Round", Azxc.GetCore().GetUI().font);
            _spinMeRound.onChecked += SpinMeRound_Checked;
            AddItem(_spinMeRound);
        }

        private void WeaponAura_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            AssaultAura.Toggle(checkBox.isChecked);
        }

        private void SpinMeRound_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            SpinMeRound.Toggle(checkBox.isChecked);
        }
    }
}
