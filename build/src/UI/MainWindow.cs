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
        public CheckBox<FancyBitmapFont> checkBox1;

        public MainWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) : base(position, sizeMode)
        {
            CreateControls();
            Prepare();
        }

        private void CreateControls()
        {
            label1 = new Label<FancyBitmapFont>("Label #1", Azxc.core.uiManager.font);
            label2 = new Label<FancyBitmapFont>("Testing new label #2 UwU", Azxc.core.uiManager.font);
            button1 = new Button<FancyBitmapFont>("Button #1", Azxc.core.uiManager.font);
            button2 = new Button<FancyBitmapFont>("Testing button #2", "with tooltip.", Azxc.core.uiManager.font);
            button3 = new Button<FancyBitmapFont>("Testing new button number 3 UwU", "with much longer tootlip.", Azxc.core.uiManager.font);
            checkBox1 = new CheckBox<FancyBitmapFont>("CheckBox #1", Azxc.core.uiManager.font);
        }

        public void Prepare()
        {
           AddItem(label1);
           AddItem(button1);
           AddItem(label2);
           AddItem(button2);
           AddItem(button3);
           AddItem(checkBox1);
        }
    }
}