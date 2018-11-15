using Harmony;
using System;
using Verse.Sound;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Sample), MethodType.Constructor, new Type[] { typeof(SubSoundDef) })]
    public class Sample_patch
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
