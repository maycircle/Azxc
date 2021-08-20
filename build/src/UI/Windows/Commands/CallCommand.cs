using Azxc.UI.Controls;
using Azxc.UI.Events;
using DuckGame;
using System.Linq;
using System.Reflection;

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
                    Button executor = new Button(method.Name);
                    executor.onClicked += CallMethod_Clicked;
                    AddItem(executor);
                }
            }
            Sort(delegate (Control x, Control y)
            {
                Button bx = x as Button;
                Button by = y as Button;
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

            Button button = sender as Button;
            foreach (MethodInfo method in typeof(Duck).GetMethods())
            {
                if (method.Name == button.text)
                {
                    try
                    {
                        method.Invoke(selectedProfile.duck, null);
                    }
                    catch
                    { }
                }
            }
        }
    }
}
