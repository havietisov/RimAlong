using CooperateRim.Utilities;
using Harmony;
using Verse.AI;

namespace CooperateRim
{
    /*
    [HarmonyPatch(typeof(JobGiver_MoveToStandable))]
    [HarmonyPatch("TryGiveJob")]
    class MoveToStandablePatch
    {
        [HarmonyPrefix]
        public static bool TryGiveJob(ref Job __result, PawnDuty pawn)
        {
<<<<<<< HEAD
            //Utilities.RimLog.Message("JobGiver_MoveToStandable ++ " + pawn);
=======
            RimLog.Message("JobGiver_MoveToStandable ++ " + pawn);
>>>>>>> Project_Cleanup
            __result = null;
            return false;
        }
    }*/
}
