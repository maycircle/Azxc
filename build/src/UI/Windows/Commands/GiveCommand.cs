using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Harmony;
using DuckGame;

using Azxc.UI.Controls;
using Azxc.UI.Events;

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
                if (InheritsFrom(thing, typeof(Holdable)) && Editor.HasConstructorParameter(thing) &&
                    !thing.IsAbstract)
                {
                    Button<FancyBitmapFont> executor = new Button<FancyBitmapFont>(thing.Name,
                        Azxc.GetCore().GetUI().font);
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
            Profile selectedProfile = (Profile)_selectors[0].GetValue();
            if (selectedProfile == null)
                return;

            Button<FancyBitmapFont> button = e.item as Button<FancyBitmapFont>;
            List<Type> thingTypes = GiveGetEditorThings();

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
            // Copied straight from Duck Game source code :P
            if (MonoMain.moddingEnabled)
                thingTypes = ManagedContent.Things.Types.ToList<Type>();
            else
                thingTypes = Editor.GetSubclasses(typeof(Thing)).ToList<Type>();
            return thingTypes;
        }
    }
}
