using Harmony;

namespace CooperateRim
{
    [HarmonyPatch(typeof(RimWorld.MusicManagerPlay), "MusicUpdate")]
    public class music__MusicUpdate
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
