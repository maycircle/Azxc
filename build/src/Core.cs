using Azxc.UI;
using DuckGame;
using Harmony;
using System;

namespace Azxc
{
    public class AzxcCore
    {
        private readonly HarmonyInstance _harmony;

        private AzxcConfig _config;
        private UserInterfaceManager _uiManager;

        public HarmonyInstance GetHarmony() => _harmony;
        public AzxcConfig GetConfig() => _config;
        public UserInterfaceManager GetUI() => _uiManager;

        public AzxcCore()
        {
            _harmony = HarmonyInstance.Create(typeof(Azxc).AssemblyQualifiedName);
        }

        public void Prepare()
        {
            _uiManager = new UserInterfaceManager(UserInterfaceState.Enabled);
            _uiManager.hintsText = "@AZXCLEFTMOUSE@@AZXCACTIVATE@ACTIVATE  @AZXCRIGHTMOUSE@@AZXCBACK@BACK";
        }

        public void CreateConfig()
        {
            _config = new AzxcConfig(ModLoader.GetMod<Azxc>().configuration.directory + "/config.xml");
        }

        public void LoadConfig()
        {
            if (_config == null)
                throw new NullReferenceException("Initialize config before using it");
        }
    }
}
