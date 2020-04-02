using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.Bindings;

namespace Azxc.UI
{
    internal class UserInterfaceManager_DoDraw
    {
        static void Postfix()
        {
            Azxc.core.uiManager.Draw();
        }
    }

    public class UserInterfaceManager : IAutoUpdate, IBinding
    {
        private UserInterfaceState _state;
        private Queue<Control> _controls;

        public UserInterfaceState state
        {
            get { return _state; }
        }

        public UserInterfaceManager(UserInterfaceState state)
        {
            _state = state;
            _controls = new Queue<Control>();

            // So our GUI will draw everywhere
            Azxc.core.harmony.Patch(typeof(Level).GetMethod("DoDraw"),
                postfix: new HarmonyMethod(typeof(UserInterfaceManager_DoDraw), "Postfix"));
        }

        [Binding(Keys.Insert)]
        public void Open()
        {
            if ((_state & UserInterfaceState.Open) == UserInterfaceState.Open)
                _state &= ~UserInterfaceState.Open;
            else
                _state |= UserInterfaceState.Open;
        }

        public void Update()
        {
            BindingManager.UsedBinding(this, "Open");

            foreach (Control control in _controls.OfType<IAutoUpdate>())
            {
                IAutoUpdate updatable = control as IAutoUpdate;
                updatable.Update();
            }
        }

        public void Draw()
        {
            foreach (Control control in _controls)
            {
                control.Draw();
            }
        }
    }
}
