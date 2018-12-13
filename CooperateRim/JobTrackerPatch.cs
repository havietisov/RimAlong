using CooperateRim.Utilities;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            RimLog.Message("TryTakeOrderedJobPrioritizedWork.IsDes : " + SyncTickData.IsDeserializing);
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
    [HarmonyPatch(typeof(Job), MethodType.Constructor, new Type[] { typeof(JobDef) , typeof(LocalTargetInfo) , typeof(LocalTargetInfo)  })]
    class JobTrackerPatch_
    {
        [HarmonyPostfix]
        public static void TryTakeOrderedJob(Job __instance)
        {
            //job.def
            RimLog.Message("TryTakeOrderedJobPrioritizedWork.IsDes : " + SyncTickData.IsDeserializing);
            //if (!SyncTickData.AvoidLoop)
            {
                StackTrace st = new StackTrace();
                List<string> strs = new List<string>();

                foreach(var a in st.GetFrames())
                {
                    strs.Add(a.GetMethod().ReflectedType + "::" + a.GetMethod().Name);
                }

                System.IO.File.WriteAllLines("C:\\CoopReplays\\" + SyncTickData.cliendID + "\\" + Find.TickManager.TicksGame + "__job__" + ".txt", strs.ToArray());
            }
        }
    }*///this one is diagnostic

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
