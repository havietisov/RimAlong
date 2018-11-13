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
    /*
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
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AllowJobAt(job, giver, cell);
                return false;
            }
            else
            {
                return true;
            }
        }
    }*/

    [HarmonyPatch(typeof(KeyBindingDef))]
    [HarmonyPatch("get_IsDownEvent")]
    class KeyBindingDefPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(bool __result)
        {
            __result = false;
            return false;
        }
    }
    /*
    [HarmonyPatch(typeof(Verse.AI.Pawn_JobTracker))]
    [HarmonyPatch("TryTakeOrderedJob")]
    class JobTrackerPatch_
    {
        [HarmonyPrefix]
        public static bool TryTakeOrderedJob(Pawn_JobTracker __instance, Job job, JobTag tag, Pawn ___pawn)
        {
            //job.def
            CooperateRimming.Log("TryTakeOrderedJobPrioritizedWork.IsDes : " + SyncTickData.IsDeserializing);
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AllowJobAt(job, ___pawn, tag, ___pawn.Position);
                return false;
            }
            else
            {
                return true;
            }
        }
    }*/

    //[HarmonyPatch(typeof(Verse.AI.Pawn_JobTracker))]
    //[HarmonyPatch("DetermineNextJob")]
    //class JobTrackerPatch_DetermineNextJob
    //{
    //    [HarmonyPrefix]
    //    public static bool DetermineNextJob(out ThinkTreeDef thinkTree, ref Pawn ___pawn)
    //    {

    //        if (___pawn.thinker.ConstantThinkTree != null)
    //        {
    //            ThinkResult rr = ___pawn.thinker.ConstantThinkNodeRoot.TryIssueJobPackage(___pawn, default(JobIssueParams));
    //            thinkTree = ___pawn.thinker.ConstantThinkTree;
    //        }
    //        else
    //        {
    //            thinkTree = null;
    //        }

    //        thinkTree = null;
    //        return true;
    //    }
    //}
}
