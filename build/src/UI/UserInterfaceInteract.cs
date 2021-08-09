using Azxc.UI.Controls;
using DuckGame;
using System.Linq;

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
        }

        public void MoveUp()
        {
            Deselect();

            if (selectedItem <= 0)
                selectedItem = activeWindow.OfType<ISelectable>().Count() - 1;
            else
                selectedItem -= 1;
            Select();
        }

        public void MoveDown()
        {
            Deselect();

            if (selectedItem >= activeWindow.OfType<ISelectable>().Count() - 1)
                selectedItem = 0;
            else
                selectedItem += 1;
            Select();
        }

        public void MoveLeft()
        {
            if (Azxc.GetCore().GetUI().GetUpdatable().OfType<Controls.Window>().Count() > 1)
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
            foreach (Control control in activeWindow.OfType<ISelectable>())
            {
                ISelectable impl = control as ISelectable;
                if (InRange(control) && !impl.isSelected)
                {
                    Deselect();
                    selectedItem = GetItemIndex(control);
                    impl.isSelected = true;
                    break;
                }
            }
        }

        public void Update()
        {
            if (Azxc.GetCore().GetUI().GetUpdatable().OfType<Controls.Window>().Count() > 0)
                activeWindow = Azxc.GetCore().GetUI().GetUpdatable().OfType<Controls.Window>().Last();
            else if (!Azxc.GetCore().GetUI().GetState().HasFlag(UserInterfaceState.Open))
                return;

            if (Keyboard.Pressed(Keys.Left))
                MoveLeft();
            if (Mouse.right == InputState.Pressed)
                MouseRight();

            if (activeWindow.OfType<ISelectable>().Count() == 0)
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
            ISelectable impl = GetItem() as ISelectable;
            impl.isSelected = false;
            impl.Select();
        }

        private void Select()
        {
            ISelectable impl = GetItem() as ISelectable;
            if (!impl.isSelected)
            {
                impl.isSelected = true;
                impl.Select();
            }
        }

        private int GetSelectedItem()
        {
            int index = 0;
            foreach (ISelectable impl in activeWindow.OfType<ISelectable>())
            {
                if (impl.isSelected)
                    return index;
                index += 1;
            }
            return 0;
        }

        private Control GetItem()
        {
            int index = 0;
            foreach (Control control in activeWindow.OfType<ISelectable>())
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
            foreach (Control control in activeWindow.OfType<ISelectable>())
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
