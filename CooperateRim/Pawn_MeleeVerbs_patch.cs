using Harmony;
using RimWorld;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Pawn_MeleeVerbs))]
    [HarmonyPatch("ChooseMeleeVerb")]
    public class Pawn_MeleeVerbs_patch
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
