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
    public class UserInterfaceInteract : IAutoUpdate
    {
        public Controls.Window activeWindow { get; private set; }

        public int selectedItem { get; private set; }

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

        public void MoveUp()
        {
            Deselect();

            if (selectedItem <= 0)
                selectedItem = activeWindow.OfType<ISelect>().Count() - 1;
            else
                selectedItem -= 1;
            Select();
        }

        public void MoveDown()
        {
            Deselect();

            if (selectedItem >= activeWindow.OfType<ISelect>().Count() - 1)
                selectedItem = 0;
            else
                selectedItem += 1;
            Select();
        }

        public void MoveLeft()
        {
            if (Azxc.GetCore().GetUI().updatable.OfType<Controls.Window>().Count() > 1)
                activeWindow.Close();
        }

        public void MoveRight()
        {
            ActivateItem();
        }

        public void MouseLeft()
        {
            if (InRange(GetItem()))
                ActivateItem();
            else
                MoveLeft();
        }

        public void MouseRight()
        {
            MoveLeft();
        }

        public void UpdateSelection()
        {
            foreach (Control control in activeWindow.OfType<ISelect>())
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
            if (Azxc.GetCore().GetUI().updatable.OfType<Controls.Window>().Count() > 0)
                activeWindow = Azxc.GetCore().GetUI().updatable.OfType<Controls.Window>().Last();
            else if (Azxc.GetCore().GetUI().updatable.OfType<Controls.Window>().Count() <= 0 ||
                !Azxc.GetCore().GetUI().state.HasFlag(UserInterfaceState.Open))
                return;

            if (Keyboard.Pressed(Keys.Left))
                MoveLeft();
            if (Mouse.right == InputState.Pressed)
                MouseRight();

            if (activeWindow.OfType<ISelect>().Count() == 0)
                return;

            selectedItem = GetSelectedItem();
            UpdateSelection();

            if (Keyboard.Pressed(Keys.Up))
                MoveUp();
            if (Keyboard.Pressed(Keys.Down))
                MoveDown();

            if (Keyboard.Pressed(Keys.Right))
                MoveRight();
            if (Mouse.left == InputState.Pressed)
                MouseLeft();

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
            foreach (ISelect impl in activeWindow.OfType<ISelect>())
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
            foreach (Control control in activeWindow.OfType<ISelect>())
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
            foreach (Control control in activeWindow.OfType<ISelect>())
            {
                if (control == item)
                    return index;
                index += 1;
            }
            return 0;
        }

        private bool InRange(Control item)
        {
            Cursor cursor = Azxc.GetCore().GetUI().cursor;

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
