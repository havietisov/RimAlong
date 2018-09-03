using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;
using Verse.AI;

namespace CooperateRim
{
    //TryTakeOrderedJobPrioritizedWork
    [HarmonyPatch(typeof(Verse.AI.Pawn_JobTracker))]
    [HarmonyPatch("TryTakeOrderedJobPrioritizedWork")]
    class JobTrackerPatch
    {
        [HarmonyPrefix]
        public static bool TryTakeOrderedJobPrioritizedWork(Job job, WorkGiver giver, IntVec3 cell)
        {
            //job.def
            CooperateRimming.Log("TryTakeOrderedJobPrioritizedWork.IsDes : " + SyncTickData.IsDeserializing);
            if (!SyncTickData.IsDeserializing)
            {
                SyncTickData.AllowJobAt(job, giver, cell);
                return false;
                //cData.Enqueue(new CachedJobData() { job = job, tag = tag, __instance = __instance });
            }
            else
            {
                return true;
            }
        }
    }
}
