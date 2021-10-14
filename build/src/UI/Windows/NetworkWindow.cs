using Azxc.Patches;
using Azxc.UI.Controls;
using Azxc.UI.Events;
using DuckGame;

namespace Azxc.UI
{
    class NetworkWindow : Controls.Window
    {
        private CheckBox _enableCustomNickname, _hatStealer, _hatConverter;
        private TextBox _customNickname;

        public NetworkWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _enableCustomNickname = new CheckBox("Enable custom nickname",
                "Change your displaying name |RED|(Switching generally doesn't work in online game).");
            _enableCustomNickname.onChecked += EnableCustomNickname_Checked;
            AddItem(_enableCustomNickname);

            _customNickname = new TextBox(Azxc.GetCore().GetConfig().TryGetSingle("CustomNickname"),
                "Custom nickname...", "|YELLOW|(CustomNickname).");
            _customNickname.inputDialogTitle = "Custom nickname:";
            _customNickname.onTextChanged += CustomNickname_TextChanged;
            AddItem(_customNickname);

            AddItem(new Separator());

            _hatStealer = new CheckBox("Hat Stealer",
                "Steal custom hats of other players |YELLOW|(HatStealerSavePath).");
            _hatStealer.onChecked += HatStealer_Checked;
            AddItem(_hatStealer);

            bool.TryParse(Azxc.GetCore().GetConfig().TryGetSingle("EnableHatConverter", "False"),
                out HatStealer.autoConvert);
            _hatConverter = new CheckBox("Hat Converter",
                "Automatically convert PNG images to Duck Game's HAT files |YELLOW|(EnableHatConverter).",
                HatStealer.autoConvert);
            _hatConverter.onChecked += HatConverter_Checked;
            AddItem(_hatConverter);
        }

        private void EnableCustomNickname()
        {
            CustomNickname.HookAndToggle(true, _customNickname.inputText);
            Azxc.GetCore().GetConfig().TrySet("CustomNickname", _customNickname.inputText);
        }

        private void EnableCustomNickname_Checked(object sender, ControlEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox.isChecked && !string.IsNullOrEmpty(_customNickname.inputText))
                EnableCustomNickname();
            else if (!checkBox.isChecked)
                CustomNickname.HookAndToggle(false, string.Empty);
        }

        private void CustomNickname_TextChanged(object sender, ControlEventArgs e)
        {
            if (_enableCustomNickname.isChecked && !string.IsNullOrEmpty(_customNickname.inputText))
                EnableCustomNickname();
        }

        private void HatStealer_Checked(object sender, ControlEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            HatStealer.HookAndToggle(checkBox.isChecked);
        }

        private void HatConverter_Checked(object sender, ControlEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            HatStealer.autoConvert = checkBox.isChecked;
            Azxc.GetCore().GetConfig().TrySet("EnableHatConverter", checkBox.isChecked.ToString());
        }
    }
}
