using Harmony;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Verse.TickManager))]
    [HarmonyPatch("TickManagerUpdate")]
    public class TickManagerPatch
    {
        public static bool isNetworkLaunch = false;
        public static bool shouldReallyTick = false;
        public static DateTime nextFrameTime;
        // public static int myTicksValue;

        public static int nextSyncTickValue = 0;
        public static int clientsInSync = 0;
        public static bool imInSync;

        public static bool IsSyncTick;
        
        public static bool Prepare()
        {
            return true;
        }
        /*
        public static void ReferenceTranspilerMethod(ref int ticksGameInt)
        {
            shouldReallyTick = false;
            IsSyncTick = false;
            SyncTickData.IsDeserializing = false;
            {
                if (DateTime.Now.Ticks > nextFrameTime.Ticks)
                {
                    bool expectToSync = nextSyncTickValue == myTicksValue;
                    
                    if (!imSynced)
                    {
                        if (expectToSync)
                        {
                            SyncTickData.FlushSyncTickData(myTicksValue);
                            imSynced = true;
                        }
                    }

                    bool allSyncDataAvailable = SyncTickData.tickFileNames(myTicksValue).All(u => System.IO.File.Exists(u + ".sync"));

                    //CooperateRimming.Log("Frame " + myTicksValue + " : " + expectToSync + " ::: " + allSyncDataAvailable + "[" + myTicksValue + "] :: " + nextSyncTickValue + " [is synced : ] " + imSynced);

                    if (!expectToSync || (expectToSync && allSyncDataAvailable))
                    {
                        if (expectToSync)
                        {
                            IsSyncTick = true;
                            nextSyncTickValue = myTicksValue + 10;
                            //CooperateRimming.Log("Synctick happened at " + myTicksValue);

                            SyncTickData.IsDeserializing = true;
                            SyncTickData.Apply(myTicksValue);
                            imSynced = false;
                            //JobTrackerPatch.FlushCData();
                        }
                        myTicksValue++;
                        shouldReallyTick = true;
                    }
                    
                    nextFrameTime = DateTime.Now + TimeSpan.FromSeconds(0.05);
                }

                ticksGameInt = myTicksValue;
            }
        }*/

        public static bool TestInjMethod()
        {
            return true;
        }

        public static void ILSourceMethod(Action originalMethod)
        {
            TestInjMethod();
        }

        public static void Retr()
        {

        }

        static Stopwatch sw;

        [HarmonyPrefix]
        public static bool Prefix(ref int ___ticksGameInt, ref TickManager __instance)
        {
            CooperateRimming.dumpRand = true;
            if (sw == null)
            {
                sw = new Stopwatch();
                sw.Start();
            }
            shouldReallyTick = false;

            if (sw.ElapsedMilliseconds > 20 && !__instance.Paused)
            {
                sw.Reset();
                sw.Start();
                bool canNormallyTick = nextSyncTickValue > ___ticksGameInt;

                //CooperateRimming.Log("Frame " + ___ticksGameInt + " canNormallyTick " + canNormallyTick);

                if (canNormallyTick)
                {
                    
                    streamholder.WriteLine("normal tick at " + ___ticksGameInt, "tickstate");
                    Rand.PushState(___ticksGameInt);
                    __instance.DoSingleTick();
                    Rand.PopState();
                }
                else
                {
                    if (!imInSync)
                    {
                        SyncTickData.FlushSyncTickData(___ticksGameInt);
                        imInSync = true;
                    }

                    bool allSyncDataAvailable = SyncTickData.tickFileNames(___ticksGameInt).All(u => System.IO.File.Exists(u + ".sync"));

                    //CooperateRimming.Log("Frame " + ___ticksGameInt + " : " + " ::: " + allSyncDataAvailable + "[" + ___ticksGameInt + "] :: " + nextSyncTickValue + " [is synced : ] " + imInSync);

                    if (allSyncDataAvailable)
                    {
                        IsSyncTick = true;
                        nextSyncTickValue = ___ticksGameInt + 10;
                        //CooperateRimming.Log("Synctick happened at " + ___ticksGameInt);

                        SyncTickData.IsDeserializing = true;
                        //JobTrackerPatch.FlushCData();
                        shouldReallyTick = true;
                        streamholder.WriteLine("pre-deserialize tick at " + ___ticksGameInt, "tickstate");
                        Rand.PushState(___ticksGameInt);
                        streamholder.WriteLine("data applied at " + ___ticksGameInt, "tickstate");
                        SyncTickData.Apply(___ticksGameInt);
                        __instance.DoSingleTick();
                        Rand.PopState();
                        imInSync = false;
                    }
                }
            }

            //ReferenceTranspilerMethod(ref ___ticksGameInt);
            return false;
        }

        /*
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> MyTranspiler(IEnumerable<CodeInstruction> instr, MethodBase __originalMethod)
        {
            var fld = typeof(TickManager).GetField("ticksGameInt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var shouldTick = typeof(TickManagerPatch).GetField("shouldReallyTick", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            var isNetfls = typeof(TickManagerPatch).GetField("isNetworkLaunch", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var retrMethod = typeof(TickManagerPatch).GetMethod("Retr"); ;
            var mtd = typeof(TickManagerPatch).GetMethod("ReferenceTranspilerMethod");
            
            CooperateRimming.Log("patched : " + fld + " : " + mtd + " : " + isNetfls);
            
            yield return new CodeInstruction(System.Reflection.Emit.OpCodes.Ldarg_0);
            yield return new CodeInstruction(System.Reflection.Emit.OpCodes.Ldflda, fld);
            yield return new CodeInstruction(System.Reflection.Emit.OpCodes.Call, mtd);
            yield return new CodeInstruction(System.Reflection.Emit.OpCodes.Ldsfld, isNetfls);
            yield return new CodeInstruction(System.Reflection.Emit.OpCodes.Brfalse, 6);
            yield return new CodeInstruction(System.Reflection.Emit.OpCodes.Ret);
            yield return new CodeInstruction(System.Reflection.Emit.OpCodes.Nop);
            yield return new CodeInstruction(System.Reflection.Emit.OpCodes.Ldsfld, shouldTick);
            yield return new CodeInstruction(System.Reflection.Emit.OpCodes.Brtrue, 11);
            yield return new CodeInstruction(System.Reflection.Emit.OpCodes.Call, retrMethod);
            yield return new CodeInstruction(System.Reflection.Emit.OpCodes.Ret);
            yield return new CodeInstruction(System.Reflection.Emit.OpCodes.Nop);
            foreach (var @in in instr)
            {
                yield return @in;
            }
        }
        */
        
        public static ResearchProjectDef cachedRDef;

        [HarmonyPostfix]
        public static void Postfix()
        {
            if (Find.ResearchManager.currentProj != cachedRDef)
            {
                SyncTickData.AppendSyncTickData(Find.ResearchManager.currentProj);
                Find.ResearchManager.currentProj = cachedRDef;
            }
        }
    }
}
