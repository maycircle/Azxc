
using Azxc.Patches.Misc;
using Azxc.UI.Controls;
using Azxc.UI.Events;
using DuckGame;

namespace Azxc.UI
{
    class FunWindow : Controls.Window
    {
        private CheckBox _assaultAura, _spinMeRound;

        public FunWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _assaultAura = new CheckBox("Assault Aura");
            _assaultAura.onChecked += WeaponAura_Checked;
            AddItem(_assaultAura);

            _spinMeRound = new CheckBox("Spin Me Round");
            _spinMeRound.onChecked += SpinMeRound_Checked;
            AddItem(_spinMeRound);
        }

        private void WeaponAura_Checked(object sender, ControlEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            AssaultAura.Toggle(checkBox.isChecked);
        }

        private void SpinMeRound_Checked(object sender, ControlEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            SpinMeRound.Toggle(checkBox.isChecked);
        }
    }
}
