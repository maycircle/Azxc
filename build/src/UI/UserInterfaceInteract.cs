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
        public Controls.Window activeWindow { get; private set; }

        public int selectedItem { get; private set; }

        // [Binding(Keys.Enter, InputState.Pressed)]
        public void ActivateItem()
        {
            Control item = GetItem();
            if (item == null || !(item is IClickable))
                return;

            IClickable impl = item as IClickable;
            impl.Click();

            // Disabled due to its poor realization
            // DuckNetwork.core.enteringText = false;
        }

        [Binding(Keys.Up, InputState.Pressed)]
        public void MoveUp()
        {
            Deselect();

            if (selectedItem <= 0)
                selectedItem = activeWindow.items.OfType<ISelect>().Count() - 1;
            else
                selectedItem -= 1;
            Select();
        }

        [Binding(Keys.Down, InputState.Pressed)]
        public void MoveDown()
        {
            Deselect();

            if (selectedItem >= activeWindow.items.OfType<ISelect>().Count() - 1)
                selectedItem = 0;
            else
                selectedItem += 1;
            Select();
        }

        [Binding(Keys.Left, InputState.Pressed)]
        public void MoveLeft()
        {
            if (Azxc.core.uiManager.controls.OfType<Controls.Window>().Count() > 1)
                activeWindow.Close();
        }

        [Binding(Keys.Right, InputState.Pressed)]
        public void MoveRight()
        {
            // Pressing right works the same as activating an item
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
            foreach (Control control in activeWindow.items.OfType<ISelect>())
            {
                ISelect impl = control as ISelect;
                if (InRange(control) && !impl.selected)
                {
                    Deselect();
                    selectedItem = GetItemIndex(control);
                    impl.selected = true;
                    break;
                }
            }
        }

        public void Update()
        {
            if (Azxc.core.uiManager.controls.OfType<Controls.Window>().Count() > 0)
                activeWindow = Azxc.core.uiManager.controls.OfType<Controls.Window>().Last();
            else if (Azxc.core.uiManager.controls.OfType<Controls.Window>().Count() <= 0 ||
                !Azxc.core.uiManager.state.HasFlag(UserInterfaceState.Open))
                return;


            BindingManager.UsedBinding(this, "MoveLeft");
            BindingManager.UsedBinding(this, "MouseRight");

            if (activeWindow.items.OfType<ISelect>().Count() == 0)
                return;

            selectedItem = GetSelectedItem();
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
            foreach (ISelect impl in activeWindow.items.OfType<ISelect>())
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
            foreach (Control control in activeWindow.items.OfType<ISelect>())
            {
                if (index == selectedItem)
                    return control;
                index += 1;
            }
            return null;
        }

        private int GetItemIndex(Control item)
        {
            int index = 0;
            foreach (Control control in activeWindow.items.OfType<ISelect>())
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
