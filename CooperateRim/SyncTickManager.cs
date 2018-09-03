using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace CooperateRim
{
    
    /*
    [HarmonyPatch(typeof(Verse.TickManager))]
    [HarmonyPatch("get_TickRateMultiplier")]
    class TickRateMultiplier_get_patch
    {
        
        static int lastCheckedTick = 0;

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> MyTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator generator, MethodBase __originalMethod)
        {
            var fld1 = typeof(TickManager).GetField(nameof(lastCheckedTick), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var prop1 = typeof(TickManagerPatch).GetProperty(nameof(TickManager.TicksGame), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            
            yield return new CodeInstruction(OpCodes.Call, prop1);
            yield return new CodeInstruction(OpCodes.Ldsfld, fld1);
            yield return new CodeInstruction(OpCodes.Blt, 5);
            
            //yield return new CodeInstruction(OpCodes.Ldc_I4, 1);
            //yield return new CodeInstruction(OpCodes.Ret);
            //yield return new CodeInstruction(OpCodes.Nop);
            
            foreach (var code in instr)
            {
                yield return code;
            }
        }
    }*/

    [HarmonyPatch(typeof(Verse.TickManager))]
    [HarmonyPatch("DoSingleTick")]
    public class TickManagerPatch
    {
        public static bool isNetworkLaunch = false;
        public static bool shouldReallyTick = false;
        public static DateTime nextFrameTime;
        public static int myTicksValue;
        public static int nextSyncTickValue = 0;

        public static bool IsSyncTick;
        
        public static bool Prepare()
        {
            return true;
        }
        
        public static void ReferenceTranspilerMethod(ref int ticksGameInt)
        {
            shouldReallyTick = false;

            IsSyncTick = false;
            SyncTickData.IsDeserializing = false;
            {
                if (DateTime.Now.Ticks > nextFrameTime.Ticks)
                {
                    if (nextSyncTickValue == myTicksValue)
                    {
                        IsSyncTick = true;
                        nextSyncTickValue = myTicksValue + 10;
                        //CooperateRimming.Log("Synctick happened at " + myTicksValue);
                        SyncTickData.FlushSyncTickData(myTicksValue);

                        SyncTickData.IsDeserializing = true;
                        SyncTickData.Apply(myTicksValue);
                        //JobTrackerPatch.FlushCData();
                    }
                    myTicksValue++;
                    //CooperateRimming.Log("[2] game ticks : " + ticksGameInt);
                    nextFrameTime = DateTime.Now + TimeSpan.FromSeconds(0.05);
                    //CooperateRimming.Log("[3] ctime : " + DateTime.Now.Ticks + "ftime : " + nextFrameTime.Ticks);
                    shouldReallyTick = true;
                }

                ticksGameInt = myTicksValue;
            }
        }

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
    }
}
