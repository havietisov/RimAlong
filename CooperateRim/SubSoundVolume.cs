using Harmony;
using Verse.Sound;

namespace CooperateRim
{
    [HarmonyPatch(typeof(SubSoundDef), "RandomizedVolume")]
    public class SubSoundVolume
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
