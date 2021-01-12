using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Harmony;
using DuckGame;

using Azxc.UI.Events;
using Azxc.UI.Controls;

namespace Azxc.UI
{
    class NetworkWindow : Controls.Window
    {
        private CheckBox<FancyBitmapFont> _enableCustomNickname;
        private TextBox<FancyBitmapFont> _customNickname;

        public NetworkWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) : base(position, sizeMode)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _enableCustomNickname = new CheckBox<FancyBitmapFont>("Enable custom nickname",
                "Change your displaying name.", Azxc.core.uiManager.font); AddItem(_enableCustomNickname);

            _customNickname = new TextBox<FancyBitmapFont>("", "Custom nickname...",
                Azxc.core.uiManager.font);
            _customNickname.inputDialogTitle = "Custom nickname:";
            AddItem(_customNickname);
        }
    }
}
