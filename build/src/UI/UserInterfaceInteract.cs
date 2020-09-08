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

            // So chat doesn't open when press Enter
            DuckNetwork.core.enteringText = false;
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
            Select();
        }

        [Binding(Keys.Down, InputState.Pressed)]
        public void MoveDown()
        {
            Deselect();

            if (_selectedItem >= _activeWindow.items.OfType<ISelect>().Count() - 1)
                _selectedItem = 0;
            else
                _selectedItem += 1;
            Select();
        }

        [Binding(Keys.Left, InputState.Pressed)]
        public void MoveLeft()
        {
            if (Azxc.core.uiManager.controls.OfType<Controls.Window>().Count() > 1)
                _activeWindow.Close();
        }

        [Binding(Keys.Right, InputState.Pressed)]
        public void MoveRight()
        {
            // Alternative for Enter
            ActivateItem();
        }

        [Binding(MouseButtons.Left, InputState.Pressed)]
        public void MouseLeft()
        {
            if (InRange(GetItem()))
                ActivateItem();
            else
                MoveLeft();
        }

        [Binding(MouseButtons.Right, InputState.Pressed)]
        public void MouseRight()
        {
            MoveLeft();
        }

        public void UpdateSelection()
        {
            foreach (Control control in _activeWindow.items.OfType<ISelect>())
            {
                ISelect impl = control as ISelect;
                if (InRange(control) && !impl.selected)
                {
                    Deselect();
                    _selectedItem = GetItemIndex(control);
                    impl.selected = true;
                    break;
                }
            }
        }

        public void Update()
        {
            if (Azxc.core.uiManager.controls.OfType<Controls.Window>().Count() > 0)
                _activeWindow = Azxc.core.uiManager.controls.OfType<Controls.Window>().Last();
            else if (Azxc.core.uiManager.controls.OfType<Controls.Window>().Count() <= 0 ||
                !Azxc.core.uiManager.state.HasFlag(UserInterfaceState.Open))
                return;


            BindingManager.UsedBinding(this, "MoveLeft");
            BindingManager.UsedBinding(this, "MouseRight");

            if (_activeWindow.items.OfType<ISelect>().Count() == 0)
                return;

            _selectedItem = GetSelectedItem();
            UpdateSelection();

            BindingManager.UsedBinding(this, "ActivateItem");

            BindingManager.UsedBinding(this, "MoveUp");
            BindingManager.UsedBinding(this, "MoveDown");

            BindingManager.UsedBinding(this, "MoveRight");
            BindingManager.UsedBinding(this, "MouseLeft");

            Select();
        }

        private void Deselect()
        {
            ISelect impl = GetItem() as ISelect;
            impl.selected = false;
            impl.Select();
        }

        private void Select()
        {
            ISelect impl = GetItem() as ISelect;
            if (!impl.selected)
            {
                impl.selected = true;
                impl.Select();
            }
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

        private int GetItemIndex(Control item)
        {
            int index = 0;
            foreach (Control control in _activeWindow.items.OfType<ISelect>())
            {
                if (control == item)
                    return index;
                index += 1;
            }
            return 0;
        }

        private bool InRange(Control item)
        {
            Cursor cursor = Azxc.core.uiManager.cursor;

            Vec2 end = item.position + item.size;
            if ((item.x <= cursor.x && item.y <= cursor.y) &&
                (end.x >= cursor.x && end.y >= cursor.y))
            {
                return true;
            }
            return false;
        }
    }
}
