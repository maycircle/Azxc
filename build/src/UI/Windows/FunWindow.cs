
using Azxc.Patches.Misc;
using Azxc.UI.Controls;
using Azxc.UI.Events;
using DuckGame;

namespace Azxc.UI
{
    class FunWindow : Controls.Window
    {
        private CheckBox<FancyBitmapFont> _assaultAura, _spinMeRound;

        public FunWindow()
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
