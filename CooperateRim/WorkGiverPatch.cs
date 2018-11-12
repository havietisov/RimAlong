using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace CooperateRim
{
    //[HarmonyPatch(typeof(RimWorld.WorkGiver_PlantsCut))]
    //[HarmonyPatch("JobOnThing")]
    class WorkGiverPatch
    {
        //[HarmonyPostfix]
        public static void JobOnThing_1(Pawn pawn, Thing t, bool forced, ref Job __result)
        {
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickData(new SyncTickData.TemporaryJobData() { pawn = pawn, targetThing = t, forced = forced, __result = __result, jobTargetA = __result != null ? __result.targetA : null, jobTargetB = __result != null ? __result.targetB : null, jobTargetC = __result != null ? __result.targetC : null });
            }
            //CooperateRimming.RimLog.Message("::___::" + __result.ToString());
        }

        public static void JobOnThing_2(Pawn pawn, Thing thing, bool forced, ref Job __result)
        {
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickData(new SyncTickData.TemporaryJobData() { pawn = pawn, targetThing = thing, forced = forced, __result = __result, jobTargetA = __result != null ? __result.targetA : null, jobTargetB = __result != null ? __result.targetB : null, jobTargetC = __result != null ? __result.targetC : null });
            }
            //CooperateRimming.RimLog.Message("::___::" + __result.ToString());
        }
    }
}
