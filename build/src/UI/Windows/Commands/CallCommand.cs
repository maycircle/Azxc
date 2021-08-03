using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.UI.Controls;
using Azxc.UI.Events;

namespace Azxc.UI
{
    class CallCommand : Controls.Misc.Command
    {
        public CallCommand()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            foreach (MethodInfo method in typeof(Duck).GetMethods())
            {
                if (method.GetParameters().Count() == 0 && method.ReturnType == typeof(void))
                {
                    Button<FancyBitmapFont> executor = new Button<FancyBitmapFont>(method.Name,
                        Azxc.GetCore().GetUI().font);
                    executor.onClicked += CallMethod_Clicked;
                    AddItem(executor);
                }
            }
            Sort(delegate (Control x, Control y)
            {
                Button<FancyBitmapFont> bx = x as Button<FancyBitmapFont>;
                Button<FancyBitmapFont> by = y as Button<FancyBitmapFont>;
                if (bx.text == null && by.text == null) return 0;
                else if (bx.text == null) return -1;
                else if (by.text == null) return 1;
                else return bx.text.CompareTo(by.text);
            });
        }

        private void CallMethod_Clicked(object sender, ControlEventArgs e)
        {
            Profile selectedProfile = (Profile)_selectors[0].GetValue();
            if (selectedProfile == null)
                return;

            Button<FancyBitmapFont> button = e.item as Button<FancyBitmapFont>;
            foreach (MethodInfo method in typeof(Duck).GetMethods())
            {
                if (method.Name == button.text)
                {
                    try
                    {
                        method.Invoke(selectedProfile.duck, null);
                    }
                    catch { }
                }
            }
        }
    }
}
