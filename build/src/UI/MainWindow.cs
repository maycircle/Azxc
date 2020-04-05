using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.UI.Controls;

namespace Azxc.UI
{
    class MainWindow : Controls.Window
    {
        public Label<FancyBitmapFont> label1, label2;
        public Button<FancyBitmapFont> button1, button2, button3;

        public MainWindow(Vec2 position, Vec2 size) : base(position, size)
        {
            CreateControls();
            Prepare();
        }

        private void CreateControls()
        {
            label1 = new Label<FancyBitmapFont>("Label #1", Azxc.core.uiManager.font);
            label2 = new Label<FancyBitmapFont>("Testing new label #2 UwU", Azxc.core.uiManager.font);
            button1 = new Button<FancyBitmapFont>("Button #1", Azxc.core.uiManager.font);
            button2 = new Button<FancyBitmapFont>("Testing button #2", Azxc.core.uiManager.font);
            button3 = new Button<FancyBitmapFont>("Testing button number 3", Azxc.core.uiManager.font);
        }

        public void Prepare()
        {
           AddItem(label1);
           AddItem(button1);
           AddItem(label2);
           AddItem(button2);
           AddItem(button3);
        }
    }
}
