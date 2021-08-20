using Azxc.UI.Controls;
using Azxc.UI.Events;
using DuckGame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azxc.UI
{
    class GiveCommand : Controls.Misc.Command
    {
        public GiveCommand()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            List<Type> thingTypes = GiveGetEditorThings();
            // Sort types by their names in alphabetical order
            thingTypes.Sort(delegate (Type x, Type y)
            {
                if (x.Name == null && y.Name == null) return 0;
                else if (x.Name == null) return -1;
                else if (y.Name == null) return 1;
                else return x.Name.CompareTo(y.Name);
            });

            foreach (Type thing in thingTypes)
            {
                if (InheritsFrom(thing, typeof(Holdable)) &&
                    Editor.HasConstructorParameter(thing) &&
                    !thing.IsAbstract)
                {
                    Button executor = new Button(thing.Name);
                    executor.onClicked += GiveMethod_Clicked;
                    AddItem(executor);
                }
            }
        }

        private bool InheritsFrom(Type type, Type baseType)
        {
            if (type == null)
                return false;

            if (baseType == null)
                return type.IsInterface || type == typeof(object);

            if (baseType.IsInterface)
                return type.GetInterfaces().Contains(baseType);

            Type currentType = type;
            while (currentType != null)
            {
                if (currentType.BaseType == baseType)
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            return false;
        }

        private void GiveMethod_Clicked(object sender, ControlEventArgs e)
        {
            Button button = sender as Button;
            List<Type> thingTypes = GiveGetEditorThings();

            Profile selectedProfile = (Profile)_selectors[0].GetValue();
            if (selectedProfile == null)
                return;

            foreach (Type thing in thingTypes)
            {
                if (thing.Name == button.text)
                {
                    Holdable thingToGive = Editor.CreateThing(thing) as Holdable;
                    Level.Add(thingToGive);
                    selectedProfile.duck.GiveHoldable(thingToGive);
                }
            }
        }

        private List<Type> GiveGetEditorThings()
        {
            List<Type> thingTypes;
            if (MonoMain.moddingEnabled)
                thingTypes = ManagedContent.Things.Types.ToList<Type>();
            else
                thingTypes = Editor.GetSubclasses(typeof(Thing)).ToList<Type>();
            return thingTypes;
        }
    }
}
