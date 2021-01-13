using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.Bindings;
using Azxc.UI;

namespace Azxc
{
    public class AzxcCore
    {
        public HarmonyInstance harmony;

        public Config config;
        public BindingManager bindingManager;
        public UserInterfaceManager uiManager;

        public AzxcCore()
        {
            harmony = HarmonyInstance.Create("harmony_ultra_unique_id");
        }

        public void Prepare()
        {
            bindingManager = new BindingManager();
            uiManager = new UserInterfaceManager(UserInterfaceState.Enabled);
            uiManager.hintsText = "@AZXCLEFTMOUSE@@AZXCACTIVATE@ACTIVATE  @AZXCRIGHTMOUSE@@AZXCBACK@BACK";
        }

        public void CreateConfig()
        {
            config = new Config(ModLoader.GetMod<Azxc>().configuration.directory + "/config.xml");
        }
    }
}
