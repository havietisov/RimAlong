using Harmony;
using RimWorld;

namespace CooperateRim
{
    [HarmonyPatch(typeof(IntermittentSteamSprayer), "SteamSprayerTick")]
    public class IntermittentSteamSprayerPatch
    {
        [HarmonyPrefix]
        public static bool tick()
        {
            return false;
        }
    }
}
