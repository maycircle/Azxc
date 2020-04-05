using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.Bindings;
using Azxc.UI.Controls;

namespace Azxc.UI
{
    public class UserInterfaceInteract : IAutoUpdate, IBinding
    {
        private Controls.Window _activeWindow;
        public Controls.Window activeWindow
        {
            get { return _activeWindow; }
        }

        private int _selectedItem;
        public int selectedItem
        {
            get { return _selectedItem; }
        }

        [Binding(Keys.Enter, InputState.Pressed)]
        public void ActivateItem()
        {
            Control item = GetItem();
            if (item == null || !(item is IClickable))
                return;

            IClickable impl = item as IClickable;
            impl.Click();
        }

        [Binding(Keys.Up, InputState.Pressed)]
        public void MoveUp()
        {
            Deselect();

            // Press Up or Down to teleport to the other side of the world
            if (_selectedItem <= 0)
                _selectedItem = _activeWindow.items.OfType<ISelect>().Count() - 1;
            else
                _selectedItem -= 1;
        }

        [Binding(Keys.Down, InputState.Pressed)]
        public void MoveDown()
        {
            Deselect();

            if (_selectedItem >= _activeWindow.items.OfType<ISelect>().Count() - 1)
                _selectedItem = 0;
            else
                _selectedItem += 1;
        }

        public void Update()
        {
            if (Azxc.core.uiManager.controls.OfType<Controls.Window>().Count() > 0)
                _activeWindow = Azxc.core.uiManager.controls.OfType<Controls.Window>().Last();
            else
                return;

            _selectedItem = GetSelectedItem();

            BindingManager.UsedBinding(this, "ActivateItem");
            BindingManager.UsedBinding(this, "MoveUp");
            BindingManager.UsedBinding(this, "MoveDown");

            ISelect impl = GetItem() as ISelect;
            impl.selected = true;
        }

        private void Deselect()
        {
            ISelect impl = GetItem() as ISelect;
            impl.selected = false;
        }

        private int GetSelectedItem()
        {
            int index = 0;
            foreach (ISelect impl in _activeWindow.items.OfType<ISelect>())
            {
                if (impl.selected)
                    return index;
                index += 1;
            }
            return 0;
        }

        private Control GetItem()
        {
            int index = 0;
            foreach (Control control in _activeWindow.items.OfType<ISelect>())
            {
                if (index == _selectedItem)
                    return control;
                index += 1;
            }
            return null;
        }
    }
}
