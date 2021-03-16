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
    class ConsoleWindow : Controls.Window
    {
        private Controls.Window _killWindow, _callWindow, _giveWindow;
        private Expander<FancyBitmapFont> _killCmd, _callCmd, _giveCmd;
        private Button<FancyBitmapFont> _skipCmd;

        public ConsoleWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) :
            base(position, sizeMode)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _killWindow = new Controls.Window(position, SizeModes.Flexible);
            _callWindow = new Controls.Window(position, SizeModes.Flexible);
            _giveWindow = new Controls.Window(position, SizeModes.Flexible);

            _killCmd = new Expander<FancyBitmapFont>(_killWindow, "Kill", "Kill player.",
                Azxc.GetCore().GetUI().font);
            _killCmd.onExpanded += KillCmd_Expanded; AddItem(_killCmd);

            _callCmd = new Expander<FancyBitmapFont>(_callWindow, "Call", "Call method on player.",
                Azxc.GetCore().GetUI().font);
            _callCmd.onExpanded += CallCmd_Expanded; AddItem(_callCmd);

            _giveCmd = new Expander<FancyBitmapFont>(_giveWindow, "Give", "Give something to player.",
                Azxc.GetCore().GetUI().font);
            _giveCmd.onExpanded += GiveCmd_Expanded; AddItem(_giveCmd);

            _skipCmd = new Button<FancyBitmapFont>("Skip", "Skip current level |RED|(Host only).",
                Azxc.GetCore().GetUI().font);
            _skipCmd.onClicked += Skip_Clicked; AddItem(_skipCmd);
        }
        
        #region Kill command
        private void KillCmd_Expanded(object sender, ControlEventArgs e)
        {
            _killWindow.Clear();
            _killWindow.AddItem(new Label<FancyBitmapFont>("Profiles:", Azxc.GetCore().GetUI().font));


            if (Profiles.activeNonSpectators.Count > 0)
            {
                foreach (Profile profile in Profiles.activeNonSpectators.Where(x => x.duck != null))
                {
                    if (profile != null && profile.duck != null)
                    {
                        Button<FancyBitmapFont> player = new Button<FancyBitmapFont>(profile.name,
                            Azxc.GetCore().GetUI().font);
                        player.onClicked += KillPlayer_Clicked;
                        _killWindow.AddItem(player);
                    }
                }
            }
            else
            {
                _killWindow.AddItem(new Label<FancyBitmapFont>("No available profiles!",
                    Azxc.GetCore().GetUI().font));
            }
            _killWindow.AddItem(new Separator());
            Controls.Window ducksWindow = new Controls.Window(Vec2.Zero, SizeModes.Flexible);

            Button<FancyBitmapFont> all = new Button<FancyBitmapFont>("All",
                "Kill all ducks.", Azxc.GetCore().GetUI().font);
            all.onClicked += DucksAll_Clicked;
            ducksWindow.AddItem(all);

            Button<FancyBitmapFont> allButYou = new Button<FancyBitmapFont>("All but Local",
                "Basically kills all ducks but local, so only works in Network or Challenge Arcades.",
                Azxc.GetCore().GetUI().font);
            allButYou.onClicked += DucksAllButLocal_Clicked;
            ducksWindow.AddItem(allButYou);

            Expander<FancyBitmapFont> ducks = new Expander<FancyBitmapFont>(ducksWindow, "Ducks",
                Azxc.GetCore().GetUI().font);
            _killWindow.AddItem(ducks);
        }

        private void KillPlayer_Clicked(object sender, ControlEventArgs e)
        {
            Button<FancyBitmapFont> button = e.item as Button<FancyBitmapFont>;
            foreach (Profile profile in Profiles.all)
            {
                if (profile.name == button.text && profile.duck != null)
                    profile.duck.Kill(new DTIncinerate(null));
            }
        }

        private void DucksAll_Clicked(object sender, ControlEventArgs e)
        {
            if (Level.current == null)
                return;
            foreach (Duck duck in Level.current.things.OfType<Duck>())
            {
                duck.Kill(new DTIncinerate(null));
            }
        }

        private void DucksAllButLocal_Clicked(object sender, ControlEventArgs e)
        {
            if (Level.current == null)
                return;
            foreach (Duck duck in Level.current.things.OfType<Duck>())
            {
                if (!duck.isLocal || duck is TargetDuck)
                    duck.Kill(new DTIncinerate(null));
            }
        }
        #endregion

        private Profile _selectedProfile;

        private void ListedPlayer_Expanded(object sender, ControlEventArgs e)
        {
            Expander<FancyBitmapFont> expander = e.item as Expander<FancyBitmapFont>;
            foreach (Profile profile in Profiles.all)
            {
                if (profile.name == expander.text && profile.duck != null)
                    _selectedProfile = profile;
            }
        }

        #region Call command
        private void CallCmd_Expanded(object sender, ControlEventArgs e)
        {
            _callWindow.Clear();
            _callWindow.AddItem(new Label<FancyBitmapFont>("Profiles:", Azxc.GetCore().GetUI().font));

            if (Profiles.activeNonSpectators.Count == 0)
            {
                _callWindow.AddItem(new Label<FancyBitmapFont>("No available profiles!", Azxc.GetCore().GetUI().font));
                return;
            }
            foreach (Profile profile in Profiles.activeNonSpectators.Where(x => x.duck != null))
            {
                if (profile != null && profile.duck != null)
                {
                    // TOOD: Optimize this. Every iteration it creates a new copy of same window which also contains all of
                    // the "Duck" type methods as button controls for the entire time
                    Expander<FancyBitmapFont> player = new Expander<FancyBitmapFont>(CallGetMethodsWindow(profile.duck.GetType()), 
                        profile.name, Azxc.GetCore().GetUI().font);
                    player.onExpanded += ListedPlayer_Expanded;
                    _callWindow.AddItem(player);
                }
            }
        }

        private Controls.Window CallGetMethodsWindow(Type type)
        {
            Controls.Window window = new Controls.Window(Vec2.Zero, SizeModes.Flexible);

            foreach (MethodInfo method in type.GetMethods())
            {
                if (method.GetParameters().Count() == 0 && method.ReturnType == typeof(void))
                {
                    Button<FancyBitmapFont> executor = new Button<FancyBitmapFont>(method.Name,
                        Azxc.GetCore().GetUI().font);
                    executor.onClicked += CallMethod_Clicked;
                    //executors.Add(executor);
                    window.AddItem(executor);
                }
            }
            window.Sort(delegate(Control x, Control y)
            {
                Button<FancyBitmapFont> bx = x as Button<FancyBitmapFont>;
                Button<FancyBitmapFont> by = y as Button<FancyBitmapFont>;
                if (bx.text == null && by.text == null) return 0;
                else if (bx.text == null) return -1;
                else if (by.text == null) return 1;
                else return bx.text.CompareTo(by.text);
            });

            return window;
        }

        private void CallMethod_Clicked(object sender, ControlEventArgs e)
        {
            if (_selectedProfile == null)
                return;

            Button<FancyBitmapFont> button = e.item as Button<FancyBitmapFont>;
            foreach (MethodInfo method in typeof(Duck).GetMethods())
            {
                if (method.Name == button.text)
                {
                    try
                    {
                        method.Invoke(_selectedProfile.duck, null);
                    }
                    catch { }
                }
            }
        }
        #endregion

        #region Give command
        private Controls.Window _giveItemsWindow;

        private void GiveCmd_Expanded(object sender, ControlEventArgs e)
        {
            _giveWindow.Clear();
            _giveWindow.AddItem(new Label<FancyBitmapFont>("Profiles:", Azxc.GetCore().GetUI().font));

            _giveItemsWindow = GiveGetItemsWindow();

            if (Profiles.activeNonSpectators.Count == 0)
            {
                _giveWindow.AddItem(new Label<FancyBitmapFont>("No available profiles!", Azxc.GetCore().GetUI().font));
                return;
            }
            foreach (Profile profile in Profiles.activeNonSpectators.Where(x => x.duck != null))
            {
                if (profile != null && profile.duck != null)
                {
                    Expander<FancyBitmapFont> player = new Expander<FancyBitmapFont>(_giveItemsWindow,
                        profile.name, Azxc.GetCore().GetUI().font);
                    player.onExpanded += ListedPlayer_Expanded;
                    _giveWindow.AddItem(player);
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

        private Controls.Window GiveGetItemsWindow()
        {
            List<Type> thingTypes = GiveGetEditorThings();
            // Sort types by their names in alphabetical order
            thingTypes.Sort(delegate(Type x, Type y)
            {
                if (x.Name == null && y.Name == null) return 0;
                else if (x.Name == null) return -1;
                else if (y.Name == null) return 1;
                else return x.Name.CompareTo(y.Name);
            });

            Controls.Window window = new Controls.Window(Vec2.Zero, SizeModes.Flexible);

            foreach (Type thing in thingTypes)
            {
                if (InheritsFrom(thing, typeof(Holdable)) && Editor.HasConstructorParameter(thing) &&
                    !thing.IsAbstract)
                {
                    Button<FancyBitmapFont> executor = new Button<FancyBitmapFont>(thing.Name,
                        Azxc.GetCore().GetUI().font);
                    executor.onClicked += GiveMethod_Clicked;
                    window.AddItem(executor);
                }
            }

            return window;
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
            if (_selectedProfile == null)
                return;

            Button<FancyBitmapFont> button = e.item as Button<FancyBitmapFont>;
            List<Type> thingTypes = GiveGetEditorThings();

            foreach (Type thing in thingTypes)
            {
                if (thing.Name == button.text)
                {
                    Holdable thingToGive = Editor.CreateThing(thing) as Holdable;
                    Level.Add(thingToGive);
                    _selectedProfile.duck.GiveHoldable(thingToGive);
                }
            }
        }
        #endregion

        private void Skip_Clicked(object sender, ControlEventArgs e)
        {
            GameMode.Skip();
        }
    }
}
