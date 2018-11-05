using Harmony;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
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

        public static int nextProcessionTick = 0;
        public static int nextCommunicationTick = 0;
        public static int clientsInSync = 0;
        public static bool imInSync;
        public static int syncRoundLength = 50;
        public static int syncTickRoundOffset = 4;
        public static bool IsSyncTick;
        static Stopwatch sw;
        static Stopwatch ACKSW;

        public static void SetSyncTickValue(int val)
        {
            //CooperateRimming.Log("new sync tick value :" + val);
            nextProcessionTick = val;
        }

        public static SyncTickData[] cachedData;
        public static void SetCachedData(SyncTickData[] __cd)
        {
            cachedData = __cd;
        }

        public static int expectedSRoundRealtimeLenMiliseconds = 200;
        public static int ticksPerSecond = 250;

        [HarmonyPrefix]
        public static bool Prefix(ref int ___ticksGameInt, ref TickManager __instance)
        {
            CooperateRimming.dumpRand = true;
            if (sw == null)
            {
                sw = new Stopwatch();
                ACKSW = new Stopwatch();
                sw.Start();
                ACKSW.Start();
            }
            shouldReallyTick = false;
            
            if (NetDemo.HasAllDataForFrame(Verse.Find.TickManager.TicksGame))
            {
                NetDemo.Receive();
            }

            if (sw.ElapsedMilliseconds > (1000 / ticksPerSecond) && !__instance.Paused)
            {
                sw.Reset();
                sw.Start();
                bool canNormallyTick = nextProcessionTick > Verse.Find.TickManager.TicksGame;

                //CooperateRimming.Log("Frame " + ___ticksGameInt + " canNormallyTick " + canNormallyTick);

                if (canNormallyTick)
                {
                    //CooperateRimming.Log("normal tick at " + Verse.Find.TickManager.TicksGame + " nsync " + nextSyncTickValue);
                    Rand.PushState(Verse.Find.TickManager.TicksGame);
                    __instance.DoSingleTick();
                    Rand.PopState();
                }

                if (SyncTickData.cliendID > -1 && ACKSW.ElapsedMilliseconds > 50)
                {
                    ACKSW.Reset();
                    ACKSW.Start();
                    //CooperateRimming.Log("Sending state request for " + nextSyncTickValue);
                    //NetDemo.SendStateRequest(nextSyncTickValue, SyncTickData.cliendID);
                }


                if (!imInSync)
                {
                    if (nextCommunicationTick == Verse.Find.TickManager.TicksGame)
                    {
                        imInSync = SyncTickData.FlushSyncTickData(nextProcessionTick + syncRoundLength * syncTickRoundOffset);
                    }
                }

                {
                    if (nextProcessionTick == Verse.Find.TickManager.TicksGame)
                    {
                       
#if FILE_TRANSFER
                    bool allSyncDataAvailable = SyncTickData.tickFileNames(___ticksGameInt).All(u => System.IO.File.Exists(u + ".sync"));
#else
                        
#endif

                        //CooperateRimming.Log("Frame " + ___ticksGameInt + " : " + " ::: " + allSyncDataAvailable + "[" + ___ticksGameInt + "] :: " + nextSyncTickValue + " [is synced : ] " + imInSync);

                        Action onA = LocalDB.OnApply;

                        if(cachedData != null)
                        {
                            IsSyncTick = true;

                            //CooperateRimming.Log("Synctick happened at " + ___ticksGameInt);

                            SyncTickData.IsDeserializing = true;
                            //JobTrackerPatch.FlushCData();
                            shouldReallyTick = true;
                            streamholder.WriteLine("pre-deserialize tick at " + Verse.Find.TickManager.TicksGame, "tickstate");
                            Rand.PushState(Verse.Find.TickManager.TicksGame);
                            streamholder.WriteLine("data applied at " + Verse.Find.TickManager.TicksGame, "tickstate");

                            //lock (LocalDB.OnApply)
                            {
                                try
                                {
                                    //CooperateRimming.Log("applying at tick " + Verse.Find.TickManager.TicksGame);

                                    var cd = cachedData;
                                    cachedData = null;

                                    foreach (var a in cd)
                                    {
                                        a.AcceptResult();
                                    }
                                    
                                    TickManagerPatch.SetSyncTickValue(nextProcessionTick + syncRoundLength);
                                    nextCommunicationTick += syncRoundLength;
                                    //Interlocked.Exchange(ref LocalDB.OnApply, null);
                                    //LocalDB.clientLocalStorage.ForEach(u => u.Value.AcceptResult());

                                    //nextSyncTickValue = Verse.Find.TickManager.TicksGame + syncRoundLength;
                                }
                                catch (Exception ee)
                                {
                                    CooperateRimming.Log(ee.ToString());
                                }

                                LocalDB.clientLocalStorage.Clear();
                            }
                            //SyncTickData.Apply(___ticksGameInt);
                            //__instance.DoSingleTick();
                            Rand.PopState();
                            imInSync = false;
                        }
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
