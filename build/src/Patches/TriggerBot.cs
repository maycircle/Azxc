using DuckGame;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azxc.Patches
{
    internal static class TriggerBot
    {
        public static bool hooked, enabled;

        public static void HookAndToggle(bool toggle)
        {
            enabled = toggle;
            if (!hooked)
            {
                Azxc.GetCore().GetHarmony().Patch(AccessTools.Method(typeof(Duck), "UpdateHoldPosition"),
                    postfix: new HarmonyMethod(typeof(TriggerBot), "Postfix"));
                hooked = true;
            }
        }

        private static bool HandleRayCheck(Vec2 start, Vec2 end, float angle, Duck except = null,
            AmmoType ammoType = null)
        {
            float length = (end - start).length;

            int steps = (int)Math.Ceiling(length);
            float singleStep = length / steps;
            do
            {
                steps--;
                Vec2 offDir = new Vec2(start.x < end.x ? 1 : -1, start.y < end.y ? 1 : -1);
                start.x += (float)Math.Cos(angle) * singleStep * offDir.x;
                start.y += -(float)Math.Sin(angle) * singleStep * -offDir.x;

                List<MaterialThing> collisionList = new List<MaterialThing>();
                Level.current.CollisionBullet(start, collisionList);

                foreach (MaterialThing thing in collisionList)
                {
                    if (thing is Duck && (except == null || thing != except))
                        return true;
                    else if (thing is Block && (ammoType == null ||
                        ammoType.penetration < thing.thickness))
                        return false;
                }
            }
            while (steps > 0);

            return false;
        }

        private static float _pressActionFrame;

        // UpdateHoldPosition@Duck
        static void Postfix(Duck __instance)
        {
            if (!enabled)
                return;

            IEnumerable<Profile> localProfiles = Profiles.activeNonSpectators.Where(x => x.localPlayer);
            foreach (Profile localProfile in localProfiles)
            {
                if (localProfile.duck == null || localProfile.duck.holdObject == null ||
                    !(localProfile.duck.holdObject is Gun))
                    continue;
                Gun holdingGun = localProfile.duck.holdObject as Gun;
                if (holdingGun.ammoType == null)
                    continue;

                Vec2 start = holdingGun.position;
                Vec2 travel = new Vec2((float)Math.Cos(holdingGun.angle) *
                    holdingGun.ammoType.range * localProfile.duck.offDir,
                    -(float)Math.Sin(holdingGun.angle) * holdingGun.ammoType.range);
                Vec2 end = start + travel;
                end.y *= start.y < end.y ? 1 : -1;

                if (HandleRayCheck(start, end, holdingGun.angle, localProfile.duck,
                    holdingGun.ammoType) && _pressActionFrame > 0.5f)
                {
                    holdingGun.PressAction();
                    _pressActionFrame = 0;
                }
                _pressActionFrame += Maths.IncFrameTimer();
            }
        }
    }
}
