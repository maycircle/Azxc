using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Harmony;
using DuckGame;

using Azxc.UI.Events;
using Azxc.UI.Controls;
using Azxc.Hacks;

namespace Azxc.UI
{
    class NetworkWindow : Controls.Window
    {
        private CheckBox<FancyBitmapFont> _enableCustomNickname;
        private TextBox<FancyBitmapFont> _customNickname;

        public NetworkWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) :
            base(position, sizeMode)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _enableCustomNickname = new CheckBox<FancyBitmapFont>("Enable custom nickname",
                "Change your displaying name |RED|(Switching doesn't work in online game).",
                Azxc.core.uiManager.font);
            _enableCustomNickname.onChecked += EnableCustomNickname_Checked;

            _customNickname = new TextBox<FancyBitmapFont>(Azxc.core.config.TryGetSingle("CustomNickname", ""),
                "Custom nickname...", "(Value loads from config).", Azxc.core.uiManager.font);
            _customNickname.inputDialogTitle = "Custom nickname:";
            _customNickname.onTextChanged += CustomNickname_TextChanged;

            onLoad += NetworkWindow_Load;
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

        private void CheckRequirements()
        {
            Clear();

            AddItem(_enableCustomNickname);
            AddItem(_customNickname);

            if (items.Count() == 0)
            {
                AddItem(new Label<FancyBitmapFont>("Game conditions don't match with any",
                                    Azxc.core.uiManager.font));
                AddItem(new Label<FancyBitmapFont>("feature requirements",
                    Azxc.core.uiManager.font));
            }
        }

        public override void Appear()
        {
            CheckRequirements();
        }

        private void NetworkWindow_Load(object sender, ControlEventArgs e)
        {
            CheckRequirements();
        }
    }
}
