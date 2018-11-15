using Harmony;
using Verse.Sound;

namespace CooperateRim
{
    [HarmonyPatch(typeof(SubSoundDef), "RandomizedResolvedGrain")]
    public class SubSoundDefOf_patch
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
