using Harmony;
using System;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Def), MethodType.Constructor, new Type[] { })]
    public class def_patch
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
