using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.UI;
using Azxc.UI.Controls;

namespace Azxc
{
    public class Azxc : ClientMod
    {
        public static AzxcCore core;

        public Azxc()
        {
            core = new AzxcCore();
            // Probably, in future I will create a special AutoPatcher for this
            core.harmony.PatchAll();
            core.Prepare();
        }

        protected override void OnPostInitialize()
        {
            core.CreateConfig();

            // OnTick gets called for the entire time game running, so I will use it as some sort
            // of "update"
            core.harmony.Patch(typeof(RockWeather).GetMethod("TickWeather"),
                postfix: new HarmonyMethod(typeof(Azxc), "OnTick"));

            LoadContent();

            MainWindow mainWindow = new MainWindow(new Vec2(5f), SizeModes.Flexible);
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

        public static void OnTick()
        {
            core.bindingManager.Update();
            core.uiManager.Update();
        }
    }
}
