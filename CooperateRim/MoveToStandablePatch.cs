using CooperateRim.Utilities;
using Harmony;
using Verse.AI;

namespace CooperateRim
{
    [HarmonyPatch(typeof(JobGiver_MoveToStandable))]
    [HarmonyPatch("TryGiveJob")]
    class MoveToStandablePatch
    {
        [HarmonyPrefix]
        public static bool TryGiveJob(ref Job __result, PawnDuty pawn)
        {
            RimLog.Message("JobGiver_MoveToStandable ++ " + pawn);
            __result = null;
            return false;
        }
    }
}
