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
            if (!SyncTickData.IsDeserializing)
            {
                SyncTickData.AppendSyncTickData(new SyncTickData.TemporaryJobData() { pawn = pawn, target = t, forced = forced, __result = __result });
            }
            //CooperateRimming.Log("::___::" + __result.ToString());
        }

        public static void JobOnThing_2(Pawn pawn, Thing thing, bool forced, ref Job __result)
        {
            if (!SyncTickData.IsDeserializing)
            {
                SyncTickData.AppendSyncTickData(new SyncTickData.TemporaryJobData() { pawn = pawn, target = thing, forced = forced, __result = __result });
            }
            //CooperateRimming.Log("::___::" + __result.ToString());
        }
    }
}
