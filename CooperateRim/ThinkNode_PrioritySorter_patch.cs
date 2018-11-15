using Harmony;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace CooperateRim
{
    public class streamholder
    {
        static Dictionary<string, System.IO.StreamWriter> _sw = new Dictionary<string, System.IO.StreamWriter>();

        public static void WriteLine(string s, string filename)
        {
            if (!_sw.ContainsKey(filename))
            {
                //_sw.Add(filename, System.IO.File.AppendText(@"D:\CoopReplays\" + (filename) + SyncTickData.cliendID + ".txt"));
            }

            //_sw[filename].WriteLine(s);
            //_sw[filename].Flush();
        }
    }

    public class ThinkNode_patch
    {
        static System.IO.StreamWriter sw;
        public static void TryIssueJobPackage(Pawn pawn, ThinkNode __instance)
        {
            {
                streamholder.WriteLine("===FRM===", "thinktree_");
                StackTrace tr = new StackTrace();
                streamholder.WriteLine("====STACK====", "thinktree_");

                foreach (var frame in tr.GetFrames())
                {
                    streamholder.WriteLine(frame.GetMethod().ReflectedType + "::" + frame.GetMethod().Name, "thinktree_");
                }

                streamholder.WriteLine("====END====", "thinktree_");
                streamholder.WriteLine(pawn + " ::: " + __instance.GetType() + "::TryIssueJobPackage :: " + "[" + TickManagerPatch.nextProcessionTick + "]", "thinktree_");
                streamholder.WriteLine("//===FRM===", "thinktree_");
            }
            //CooperateRimming.Log(pawn  + " ::: " + __instance.GetType() + "::TryIssueJobPackage :: " + "[" + TickManagerPatch.nextSyncTickValue + "]");
        }
    }

    [HarmonyPatch(typeof(TickList))]
    [HarmonyPatch("RegisterThing")]
    public class TickListPatch
    {
        [HarmonyPrefix]
        public static void RegisterThing(Thing t)
        {
            {
                streamholder.WriteLine("===REGISTER_THING===", "RegisterThing");
                StackTrace tr = new StackTrace();
                streamholder.WriteLine("====STACK====", "RegisterThing");

                foreach (var frame in tr.GetFrames())
                {
                    streamholder.WriteLine(frame.GetMethod().ReflectedType + "::" + frame.GetMethod().Name, "RegisterThing");
                }

                streamholder.WriteLine("====END====", "RegisterThing");
                streamholder.WriteLine(t + " ::: " + "[" + TickManagerPatch.nextProcessionTick + "]", "RegisterThing");
                streamholder.WriteLine("//===REGISTER_THING===", "RegisterThing");
            }
        }
    }

    [HarmonyPatch(typeof(Gen), "IsNestedHashIntervalTick", new[] { typeof(Thing), typeof(int), typeof(int) })]
    public class GenPatch
    {
        [HarmonyPostfix]
        public static void Offset(Thing t, ref bool __result)
        {
            streamholder.WriteLine("===HASH OFFSET FOR THING===", "HASHOFF_");
            streamholder.WriteLine(t.ToString() + " ::: " + __result + "[" + Find.TickManager.TicksAbs + "]", "HASHOFF_");
            streamholder.WriteLine("//===HASH OFFSET FOR THING===", "HASHOFF_");
        }
    }

    [HarmonyPatch(typeof(Pawn_JobTracker))]
    [HarmonyPatch("JobTrackerTick")]
    public class _job_tracker
    {
        [HarmonyPrefix]
        public static void JobTrackerTick(Pawn ___pawn, Pawn_JobTracker __instance)
        {
            streamholder.WriteLine("===HASH OFFSET FOR THING===", "jobtrackerargs_");
            if (__instance.curJob != null)
            {
                //__instance.curJob.expiryInterval = 50;
                var rSeed = (typeof(Rand).GetField("random", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null) as RandomNumberGenerator).seed;
                streamholder.WriteLine(___pawn.ToString() + " ::: " + "[" + Find.TickManager.TicksAbs + "], start tick : " + __instance.curJob.startTick + ", override on expire : " + __instance.curJob.checkOverrideOnExpire + ", expiry interval : " + __instance.curJob.expiryInterval + ", rand seed : " + rSeed, "jobtrackerargs_");
            }
            else
            {
                streamholder.WriteLine(___pawn.ToString() + " ::: " + "[" + Find.TickManager.TicksAbs + "], job is null", "jobtrackerargs_");
            }
            streamholder.WriteLine("//===HASH OFFSET FOR THING===", "jobtrackerargs_");
        }
    }

    [HarmonyPatch(typeof(TickList))]
    [HarmonyPatch("Tick")]
    public class TickListPatch_tick
    {
        [HarmonyPrefix]
        public static void Tick(ref Pawn __instance, ref List<List<Thing>> ___thingLists, ref TickerType ___tickType)
        {
            {
                streamholder.WriteLine("===UPDATE_THING===", "Upd");
                StackTrace tr = new StackTrace();
                streamholder.WriteLine("====STACK====", "Upd");
                
                int TickInterval = -1;

                switch (___tickType)
                {
                    case TickerType.Normal:
                        TickInterval = 1;
                        break;
                    case TickerType.Rare:
                        TickInterval = 250;
                        break;
                    case TickerType.Long:
                        TickInterval = 2000;
                        break;
                    default:
                        TickInterval = -1;
                        break;

                }

                foreach (var frame in tr.GetFrames())
                {
                    streamholder.WriteLine(frame.GetMethod().ReflectedType + "::" + frame.GetMethod().Name, "Upd");
                }

                streamholder.WriteLine("====END====", "Upd");
                var tt = ___thingLists[Find.TickManager.TicksGame % TickInterval];

                foreach (var t in tt)
                {
                    streamholder.WriteLine(t + " ::: " + "[" + TickManagerPatch.nextProcessionTick + "] {" + ___thingLists.Count + "}", "Upd");
                }

                streamholder.WriteLine("//===UPDATE_THING===", "Upd");
            }
        }
    }

    public class JobGiver_patch
    {
        static int callCount;

        public static void TryGiveJob(Pawn pawn, ThinkNode __instance, Job __result)
        {
            //if(pawn.ToString() == "Peters")
            if(SyncTickData.cliendID >= 0)
            {
                List<string> sr = new List<string>();

                sr.Add("====STACK====");
                StackTrace tr = new StackTrace();
                foreach (var frame in tr.GetFrames())
                {
                    sr.Add(frame.GetMethod().ReflectedType + "::" + frame.GetMethod().Name);
                }

                sr.Add("====END====");
                sr.Add("result : " + __result);
                System.IO.File.AppendAllText("G:/CoopReplays/" + SyncTickData.cliendID + "/" + __instance.ToString() + "_" + (callCount++) + "_" + pawn + ".txt", string.Join("\r\n", sr.ToArray()));

                /*
                streamholder.WriteLine("===JOB===", "JobGiver__" + TickManagerPatch.nextProcessionTick + "__");
                StackTrace tr = new StackTrace();
                streamholder.WriteLine("====STACK====", "JobGiver__" + TickManagerPatch.nextProcessionTick + "__");

                foreach (var frame in tr.GetFrames())
                {
                    streamholder.WriteLine(frame.GetMethod().ReflectedType + "::" + frame.GetMethod().Name, "JobGiver__" + TickManagerPatch.nextProcessionTick + "__");
                }

                streamholder.WriteLine("====END====", "JobGiver__" + TickManagerPatch.nextProcessionTick + "__");
                streamholder.WriteLine(pawn + " ::: " + __instance.GetType().ToString() +  "::TryGiveJob :: " + "[" + TickManagerPatch.nextProcessionTick + "]", "JobGiver__" + TickManagerPatch.nextProcessionTick + "__");
                streamholder.WriteLine("//===JOB===", "JobGiver__" + TickManagerPatch.nextProcessionTick + "__");*/
            }
            //CooperateRimming.Log(pawn  + " ::: " + __instance.GetType() + "::TryIssueJobPackage :: " + "[" + TickManagerPatch.nextSyncTickValue + "]");
        }
    }

    /*
    [HarmonyPatch(typeof(ThinkNode_PrioritySorter))]
    [HarmonyPatch("TryIssueJobPackage")]
    public class ThinkNode_PrioritySorter_patch
    {
        [HarmonyPrefix]
        public static bool TryIssueJobPackage(ref List<ThinkNode> ___subNodes, Pawn pawn, ref JobIssueParams jobParams, ref ThinkResult __result)
        {
            CooperateRimming.Log("ThinkNode_PrioritySorter::TryIssueJobPackage :: " + pawn + "[" + TickManagerPatch.nextSyncTickValue + "]");
            List<ThinkNode> tn = new List<ThinkNode>(___subNodes);
            //tn.Sort((u1, u2) => (int)(10000 * (u1.GetPriority(pawn) - u2.GetPriority(pawn))));

            __result = ThinkResult.NoJob;
            try
            {
                __result = tn[0].TryIssueJobPackage(pawn, jobParams);
            }
            catch (Exception ee)
            {

            }

            return false;
        }
    }

    [HarmonyPatch(typeof(ThinkNode_Random))]
    [HarmonyPatch("TryIssueJobPackage")]
    public class ThinkNode_Random_patch
    {
        [HarmonyPrefix]
        public static bool TryIssueJobPackage(ref List<ThinkNode> ___subNodes, Pawn pawn, ref JobIssueParams jobParams, ref ThinkResult __result)
        {
            CooperateRimming.Log("ThinkNode_Random::TryIssueJobPackage :: " + pawn + "[" + TickManagerPatch.nextSyncTickValue + "]");
            List<ThinkNode> tn = new List<ThinkNode>(___subNodes);

            __result = ThinkResult.NoJob;
            try
            {
                __result = tn[0].TryIssueJobPackage(pawn, jobParams);
            }
            catch (Exception ee)
            {

            }

            return false;
        }
    }*/
}
