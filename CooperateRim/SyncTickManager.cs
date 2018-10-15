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
        public static int syncRoundLength = 10;
        public static bool IsSyncTick;
        
        
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

            if (sw.ElapsedMilliseconds > 100 && !__instance.Paused)
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
#if FILE_TRANSFER
                    bool allSyncDataAvailable = SyncTickData.tickFileNames(___ticksGameInt).All(u => System.IO.File.Exists(u + ".sync"));
#else
                    NetDemo.FrameData fd = null;
                    NetDemo.SendStateRequest(___ticksGameInt, SyncTickData.cliendID);
                    NetDemo.TryGetPackets(false, u => { fd = u; });

                    bool allSyncDataAvailable = fd != null;
#endif

                    //CooperateRimming.Log("Frame " + ___ticksGameInt + " : " + " ::: " + allSyncDataAvailable + "[" + ___ticksGameInt + "] :: " + nextSyncTickValue + " [is synced : ] " + imInSync);

                    if (allSyncDataAvailable)
                    {
                        IsSyncTick = true;
                        nextSyncTickValue = ___ticksGameInt + syncRoundLength;
                        //CooperateRimming.Log("Synctick happened at " + ___ticksGameInt);

                        SyncTickData.IsDeserializing = true;
                        //JobTrackerPatch.FlushCData();
                        shouldReallyTick = true;
                        streamholder.WriteLine("pre-deserialize tick at " + ___ticksGameInt, "tickstate");
                        Rand.PushState(___ticksGameInt);
                        streamholder.WriteLine("data applied at " + ___ticksGameInt, "tickstate");
                        
                        foreach(var data in fd.frameData)
                        {
                            BinaryFormatter ser = new BinaryFormatter();
                            MemoryStream fs = new System.IO.MemoryStream(data);
                            SyncTickData sd = ser.Deserialize(fs) as SyncTickData;
                        }

                        //SyncTickData.Apply(___ticksGameInt);
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
