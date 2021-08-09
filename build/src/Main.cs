using Azxc.UI;
using DuckGame;
using Harmony;
using System.Collections.Generic;
using System.Reflection;

namespace Azxc
{
    public class Azxc : ClientMod, IAutoUpdate
    {
        private static AzxcCore _core;

        public static AzxcCore GetCore() => _core;

        public Azxc()
        {
            _core = new AzxcCore();
            // Probably, in future I will create a special AutoPatcher for this
            _core.GetHarmony().PatchAll();
            _core.Prepare();
        }

        protected override void OnPostInitialize()
        {
            _core.CreateConfig();
            _core.LoadConfig();

            AutoUpdatables.Add(this);
            LoadContent();

            // Enable DevConsole Implementation on start-up
            bool.TryParse(_core.GetConfig().TryGetSingle("EnableDevConsoleImpl", "True"),
                out DevConsoleImpl.enabled);
            DevConsoleImpl.HookAndToggle(DevConsoleImpl.enabled);

            MainWindow mainWindow = new MainWindow();
            mainWindow.position = new Vec2(5.0f);
            mainWindow.Show();
        }

        public void LoadContent()
        {
            FieldInfo fieldTriggerImageMap = AccessTools.Field(typeof(Input), "_triggerImageMap");
            Dictionary<string, Sprite> triggerImageMap = (Dictionary<string, Sprite>)fieldTriggerImageMap.GetValue(null);

            // Image-triggers that visualise when you type their IDs. Example: @AZXCLEFTMOUSE@.
            // I especially made sprites that don't have outlines in them because of the `DrawOutline`
            // function, that draws outline on top of outline
            triggerImageMap.Add("AZXCLEFTMOUSE", new Sprite(GetPath("buttons/left_mouse_nooutline.png")));
            triggerImageMap.Add("AZXCRIGHTMOUSE", new Sprite(GetPath("buttons/right_mouse_nooutline.png")));
            triggerImageMap.Add("AZXCACTIVATE", new Sprite(GetPath("buttons/activate_nooutline.png")));
            triggerImageMap.Add("AZXCBACK", new Sprite(GetPath("buttons/back_nooutline.png")));

            fieldTriggerImageMap.SetValue(null, triggerImageMap);
        }

        public void Update()
        {
            _core.GetUI().Update();
        }
    }
}
