
using DuckGame;
using Harmony;

namespace Azxc.Patches
{
    internal static class CustomNickname
    {
        public static bool hooked, enabled;
        public static string customNickname;

        public static void HookAndToggle(bool toggle, string customNickname)
        {
            enabled = toggle;

            CustomNickname.customNickname = customNickname;
            SetCustomNickname(!toggle);

            if (!hooked)
            {
                Azxc.GetCore().GetHarmony().Patch(AccessTools.Method(typeof(DuckNetwork), "PrepareProfile"),
                    postfix: new HarmonyMethod(typeof(CustomNickname), "Postfix"));
                hooked = true;
            }
        }

        private static void SetCustomNickname(bool restoreOriginal = false)
        {
            Profile localProfile = Profiles.active.Find(x =>
            {
                if (x.duck != null)
                    return x.duck.isLocal;
                return false;
            });
            if (localProfile == null)
                return;
            localProfile.keepSetName = !restoreOriginal;
            if (!restoreOriginal)
                localProfile.name = customNickname;
        }

        // PrepareProfile@DuckNetwork
        static void Postfix(Profile pProfile)
        {
            if (enabled && DuckNetwork.localProfile != null)
            {
                DuckNetwork.localProfile.keepSetName = true;
                DuckNetwork.localProfile.name = customNickname;
            }
        }
    }
}
