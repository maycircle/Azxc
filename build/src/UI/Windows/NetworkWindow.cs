using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DuckGame;

using Azxc.UI.Events;
using Azxc.UI.Controls;
using Azxc.Patches;

namespace Azxc.UI
{
    class NetworkWindow : Controls.Window
    {
        private CheckBox<FancyBitmapFont> _enableCustomNickname, _hatStealer, _hatConverter;
        private TextBox<FancyBitmapFont> _customNickname;

        public NetworkWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) :
            base(position, sizeMode)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _enableCustomNickname = new CheckBox<FancyBitmapFont>("Enable custom nickname",
                "Change your displaying name |RED|(Switching SOMETIMES works in online game).",
                Azxc.core.uiManager.font);
            _enableCustomNickname.onChecked += EnableCustomNickname_Checked;
            AddItem(_enableCustomNickname);

            _customNickname = new TextBox<FancyBitmapFont>(Azxc.core.config.TryGetSingle("CustomNickname", ""),
                "Custom nickname...", "|YELLOW|(CustomNickname).", Azxc.core.uiManager.font);
            _customNickname.inputDialogTitle = "Custom nickname:";
            _customNickname.onTextChanged += CustomNickname_TextChanged;
            AddItem(_customNickname);

            AddItem(new Separator());

            _hatStealer = new CheckBox<FancyBitmapFont>("Hat Stealer",
                "Steal hats of other players |YELLOW|(HatStealerSavePath).", Azxc.core.uiManager.font);
            _hatStealer.onChecked += HatStealer_Checked;
            AddItem(_hatStealer);

            bool.TryParse(Azxc.core.config.TryGetSingle("EnableHatConverter", "False"),
                out HatStealer.autoConvert);
            _hatConverter = new CheckBox<FancyBitmapFont>("Hat Converter",
                "Automatically convert PNG images to Duck Game's HAT files |YELLOW|(EnableHatConverter).",
                Azxc.core.uiManager.font, HatStealer.autoConvert);
            _hatConverter.onChecked += HatConverter_Checked;
            AddItem(_hatConverter);
        }

        private void EnableCustomNickname()
        {
            CustomNickname.HookAndToggle(true, _customNickname.inputText);
            Azxc.core.config.TrySet("CustomNickname", _customNickname.inputText);
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
            HatStealer.CheckSaveFolder();
            HatStealer.HookAndToggle(checkBox.isChecked);
        }

        private void HatConverter_Checked(object sender, ControlEventArgs e)
        {
            CheckBox<FancyBitmapFont> checkBox = e.item as CheckBox<FancyBitmapFont>;
            HatStealer.autoConvert = checkBox.isChecked;
            Azxc.core.config.TrySet("EnableHatConverter", checkBox.isChecked.ToString());
        }
    }
}
