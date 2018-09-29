using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            CooperateRimming.Log("JobGiver_MoveToStandable ++ " + pawn);
            __result = null;
            return false;
        }
    }
}
