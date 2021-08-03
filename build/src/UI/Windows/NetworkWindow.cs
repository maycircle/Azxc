
using Azxc.Patches;
using Azxc.UI.Controls;
using Azxc.UI.Events;
using DuckGame;

namespace Azxc.UI
{
    class NetworkWindow : Controls.Window
    {
        private CheckBox<FancyBitmapFont> _enableCustomNickname, _hatStealer, _hatConverter;
        private TextBox<FancyBitmapFont> _customNickname;

        public NetworkWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _enableCustomNickname = new CheckBox<FancyBitmapFont>("Enable custom nickname",
                "Change your displaying name |RED|(Switching generally doesn't work in online game).",
                Azxc.GetCore().GetUI().font);
            _enableCustomNickname.onChecked += EnableCustomNickname_Checked;
            AddItem(_enableCustomNickname);

            _customNickname = new TextBox<FancyBitmapFont>(Azxc.GetCore().GetConfig().TryGetSingle("CustomNickname", ""),
                "Custom nickname...", "|YELLOW|(CustomNickname).", Azxc.GetCore().GetUI().font);
            _customNickname.inputDialogTitle = "Custom nickname:";
            _customNickname.onTextChanged += CustomNickname_TextChanged;
            AddItem(_customNickname);

            AddItem(new Separator());

            _hatStealer = new CheckBox<FancyBitmapFont>("Hat Stealer",
                "Steal custom hats of other players |YELLOW|(HatStealerSavePath).", Azxc.GetCore().GetUI().font);
            _hatStealer.onChecked += HatStealer_Checked;
            AddItem(_hatStealer);

            bool.TryParse(Azxc.GetCore().GetConfig().TryGetSingle("EnableHatConverter", "False"),
                out HatStealer.autoConvert);
            _hatConverter = new CheckBox<FancyBitmapFont>("Hat Converter",
                "Automatically convert PNG images to Duck Game's HAT files |YELLOW|(EnableHatConverter).",
                Azxc.GetCore().GetUI().font, HatStealer.autoConvert);
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
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
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
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            HatStealer.HookAndToggle(checkBox.isChecked);
        }

        private void HatConverter_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            HatStealer.autoConvert = checkBox.isChecked;
            Azxc.GetCore().GetConfig().TrySet("EnableHatConverter", checkBox.isChecked.ToString());
        }
    }
}
