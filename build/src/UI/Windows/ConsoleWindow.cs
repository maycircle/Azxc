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
        private Controls.Window _killWindow, _callWindow;
        public Expander<FancyBitmapFont> killCmd, callCmd;

        public ConsoleWindow(Vec2 position, SizeModes sizeMode = SizeModes.Static) : base(position, sizeMode)
        {
            _killWindow = new Controls.Window(position, SizeModes.Flexible);
            _callWindow = new Controls.Window(position, SizeModes.Flexible);

            killCmd = new Expander<FancyBitmapFont>(_killWindow, "Kill", "Kill any player.",
                Azxc.core.uiManager.font);
            killCmd.onExpanded += KillCmd_Expanded;

            callCmd = new Expander<FancyBitmapFont>(_callWindow, "Call", "Call method on player.",
                Azxc.core.uiManager.font);
            callCmd.onExpanded += CallCmd_Expanded;

            Prepare();
        }

        public void Prepare()
        {
            AddItem(killCmd);
            AddItem(callCmd);
        }

        private List<Profile> GetProfiles()
        {
            List<Profile> profiles = new List<Profile>();
            foreach (Profile profile in Profiles.active)
            {
                if (profile != null && profile.duck != null)
                    profiles.Add(profile);
            }
            return profiles;
        }
        
        #region Kill command
        private void KillCmd_Expanded(object sender, ControlEventArgs e)
        {
            _killWindow.Clear();
            _killWindow.AddItem(new Label<FancyBitmapFont>("Profiles:", Azxc.core.uiManager.font));

            List<Profile> profiles = GetProfiles();
            if (profiles.Count > 0)
            {
                foreach (Profile profile in profiles)
                {
                    if (profile != null && profile.duck != null)
                    {
                        Button<FancyBitmapFont> player = new Button<FancyBitmapFont>(profile.name,
                            Azxc.core.uiManager.font);
                        player.onClicked += KillPlayer_Clicked;
                        _killWindow.AddItem(player);
                    }
                }
            }
            else
            {
                _killWindow.AddItem(new Label<FancyBitmapFont>("No available profiles!", Azxc.core.uiManager.font));
            }
            _killWindow.AddItem(new Separator());
            Controls.Window ducksWindow = new Controls.Window(Vec2.Zero, SizeModes.Flexible);

            Button<FancyBitmapFont> all = new Button<FancyBitmapFont>("All",
                "Kill all ducks.", Azxc.core.uiManager.font);
            all.onClicked += DucksAll_Clicked;
            ducksWindow.AddItem(all);

            Button<FancyBitmapFont> allButYou = new Button<FancyBitmapFont>("All but Local",
                "Basically kills all ducks but local, so only works in Network or Challenge Arcades.", Azxc.core.uiManager.font);
            allButYou.onClicked += DucksAllButYou_Clicked;
            ducksWindow.AddItem(allButYou);

            Expander<FancyBitmapFont> ducks = new Expander<FancyBitmapFont>(ducksWindow, "Ducks",
                Azxc.core.uiManager.font);
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

        private void DucksAllButYou_Clicked(object sender, ControlEventArgs e)
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

        #region Call command
        private Profile _selectedProfile;

        private void CallCmd_Expanded(object sender, ControlEventArgs e)
        {
            _callWindow.Clear();
            _callWindow.AddItem(new Label<FancyBitmapFont>("Profiles:", Azxc.core.uiManager.font));

            List<Profile> profiles = GetProfiles();
            if (profiles.Count == 0)
            {
                _callWindow.AddItem(new Label<FancyBitmapFont>("No available profiles!", Azxc.core.uiManager.font));
                return;
            }
            foreach (Profile profile in profiles)
            {
                if (profile != null && profile.duck != null)
                {
                    Expander<FancyBitmapFont> player = new Expander<FancyBitmapFont>(GetMethods(profile.duck.GetType()), 
                        profile.name, Azxc.core.uiManager.font);
                    player.onExpanded += CallPlayer_Expanded;
                    _callWindow.AddItem(player);
                }
            }
        }

        private void CallPlayer_Expanded(object sender, ControlEventArgs e)
        {
            Expander<FancyBitmapFont> expander = e.item as Expander<FancyBitmapFont>;
            foreach (Profile profile in Profiles.all)
            {
                if (profile.name == expander.text && profile.duck != null)
                    _selectedProfile = profile;
            }
        }

        private Controls.Window GetMethods(Type type)
        {
            Controls.Window window = new Controls.Window(Vec2.Zero, SizeModes.Flexible);
            foreach (MethodInfo method in type.GetMethods())
            {
                if (method.GetParameters().Count() == 0 && method.ReturnType == typeof(void))
                {
                    Button<FancyBitmapFont> executor = new Button<FancyBitmapFont>(method.Name,
                        Azxc.core.uiManager.font);
                    executor.onClicked += CallMethod_Clicked;
                    window.AddItem(executor);
                }
            }

            return window;
        }

        private void CallMethod_Clicked(object sender, ControlEventArgs e)
        {
            Button<FancyBitmapFont> button = e.item as Button<FancyBitmapFont>;
            //if (DuckNetwork.active)
            //{
                // I need to somehow change other ducks using Network, DuckNetwork, or Messages,
                // but i don't know how
                //Send.Message(new NMCallDuck(_selectedProfile.networkIndex, button.text, null));
            //}
            //else
            //{
                foreach (MethodInfo method in typeof(Duck).GetMethods())
                {
                    if (method.Name == button.text)
                        method.Invoke(_selectedProfile.duck, null);
                }
            //}
        }
        #endregion
    }
}
