using Harmony;
using System;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Mote), MethodType.Constructor, new Type[] {})]
    public class Mote_patch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            getValuePatch.GuardedPush();
        }

        [HarmonyPostfix]
        public static void postfix()
        {
            getValuePatch.GuardedPop();
        }
    }
}
