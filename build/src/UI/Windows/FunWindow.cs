using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using DuckGame;

using Azxc.UI.Controls;
using Azxc.UI.Events;
using Azxc.Hacks.Misc;

namespace Azxc.UI
{
    class FunWindow : Controls.Window
    {
        private CheckBox<FancyBitmapFont> _assaultAura;

        public FunWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) :
            base(position, sizeMode)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _assaultAura = new CheckBox<FancyBitmapFont>("Assault Aura", Azxc.core.uiManager.font);
            _assaultAura.onChecked += WeaponAura_Checked;
            AddItem(_assaultAura);
        }

        private void WeaponAura_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            AssaultAura.Toggle(checkBox.isChecked);
        }
    }
}
