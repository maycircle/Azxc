using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;

using Azxc.Bindings;
using Azxc.UI;

namespace Azxc
{
    public class AzxcCore
    {
        public HarmonyInstance harmony;

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
        }
    }
}
