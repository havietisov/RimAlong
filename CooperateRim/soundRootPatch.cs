using Harmony;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Verse.Sound.SoundRoot), "Update")]
    public class soundRootPatch
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
