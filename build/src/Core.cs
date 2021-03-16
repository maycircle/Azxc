using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.UI;

namespace Azxc
{
    public class AzxcCore
    {
        private HarmonyInstance _harmony;

        private Config _config;
        private UserInterfaceManager _uiManager;

        public AzxcCore()
        {
            _harmony = HarmonyInstance.Create("harmony_ultra_unique_id");
        }

        public void Prepare()
        {
            _uiManager = new UserInterfaceManager(UserInterfaceState.Enabled);
            _uiManager.hintsText = "@AZXCLEFTMOUSE@@AZXCACTIVATE@ACTIVATE  @AZXCRIGHTMOUSE@@AZXCBACK@BACK";
        }

        public void CreateConfig()
        {
            _config = new Config(ModLoader.GetMod<Azxc>().configuration.directory + "/config.xml");
        }

        public HarmonyInstance GetHarmony() => _harmony;
        public Config GetConfig() => _config;
        public UserInterfaceManager GetUI() => _uiManager;
    }
}
