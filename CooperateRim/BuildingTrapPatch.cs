using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Pawn_DraftController), "set_Drafted")]
    public class Pawn_DraftControllerPatch
    {
        [HarmonyPrefix]
        public static bool set_Drafted(Pawn_DraftController __instance, bool value)
        {
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickDataPawnDrafted(__instance.pawn, value);
                return false;
            }
            else
            {
                return true;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(Building_Door), "GetGizmos")]
    public class BuildingDoorPatch
    {
        static DynamicMethod compilerMethod;
        static FieldInfo fi = (typeof(Building_Door)).GetField("holdOpenInt", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);

        public static void SetHoldOpenValue(Building_Door instt, bool val)
        {
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncFieldForThingCommand(instt, fi, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase, val);
            }
            else
            {
                fi.SetValue(instt, val);
            }
        }

        [HarmonyPrefix]
        public static bool LambdaPatch(Building_Door __instance, ref IEnumerable<Gizmo> __result, Func<IEnumerable<Gizmo>> __originalMethod)
        {
            CooperateRimming.Log("FI : " + fi);//this one is null, btw. probably 1.0 become different
            if (compilerMethod == null)
            {
                var methodInfo = typeof(Building).GetMethod("GetGizmos", BindingFlags.Instance | BindingFlags.Public);

                // Create DynamicMethod based on the methodInfo
                var dynamicMethod = new DynamicMethod("GetGizmos", methodInfo.ReturnType, new[] { methodInfo.DeclaringType }, methodInfo.DeclaringType);

                var il = dynamicMethod.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                // Emit method call
                il.EmitCall(OpCodes.Call, methodInfo, null);
                // Emit method return value
                il.Emit(OpCodes.Ret);
                var b = __instance;
                //dynamicMethod.Invoke(null, new object[] { b });
                compilerMethod = dynamicMethod;
            }

            List<Gizmo> li = new List<Gizmo>();

            foreach (var i in (IEnumerable<Gizmo>)compilerMethod.Invoke(null, new object[] { __instance }))
            {
                li.Add(i);
            }

            {
                if (__instance.Faction == Faction.OfPlayer)
                {
                    li.Add((Gizmo)new Command_Toggle()
                    {
                        defaultLabel = "CommandToggleDoorHoldOpen".Translate(),
                        defaultDesc = "CommandToggleDoorHoldOpenDesc".Translate(),
                        hotKey = KeyBindingDefOf.Misc3,
                        icon = TexCommand.HoldOpen,
                        isActive = (Func<bool>)(() => !(bool)fi.GetValue(__instance)),
                        toggleAction = (Action)delegate
                        {
                            SetHoldOpenValue(__instance, !(bool)fi.GetValue(__instance));
                        }
                    });
                }
            }

            __result = li;
            return false;
        }
    }

    [HarmonyPatch(typeof(Building_Trap), "GetGizmos")]
    public class BuildingTrapPatch
    {
        static DynamicMethod compilerMethod;
        static FieldInfo fi = (typeof(Building_Trap)).GetField("autoRearm", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);

        public static void SetAutoRearmValue(Building_Trap instt, bool val)
        {
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickData(instt, val);
            }
            else
            {
                fi.SetValue(instt, val);
            }
        }

        [HarmonyPrefix]
        public static bool LambdaPatch(Building_Trap __instance, ref IEnumerable<Gizmo> __result, Func<IEnumerable<Gizmo>> __originalMethod)
        {
            CooperateRimming.Log("FI : "  + fi );//this one is null, btw. probably 1.0 become different
            if (compilerMethod == null)
            {
                var methodInfo = typeof(Building).GetMethod("GetGizmos", BindingFlags.Instance | BindingFlags.Public);

                // Create DynamicMethod based on the methodInfo
                var dynamicMethod = new DynamicMethod("GetGizmos", methodInfo.ReturnType, new[] { methodInfo.DeclaringType }, methodInfo.DeclaringType);
                
                var il = dynamicMethod.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                // Emit method call
                il.EmitCall(OpCodes.Call, methodInfo, null);
                // Emit method return value
                il.Emit(OpCodes.Ret);
                var b = __instance;
                //dynamicMethod.Invoke(null, new object[] { b });
                compilerMethod = dynamicMethod;
            }
            List<Gizmo> li = new List<Gizmo>();

            foreach (var i in (IEnumerable<Gizmo>)compilerMethod.Invoke(null, new object[] { __instance }))
            {
                li.Add(i);
            }
            
            {
                li.Add((Gizmo)new Command_Toggle()
                {
                    defaultLabel = "CommandAutoRearm".Translate(),
                    defaultDesc = "CommandAutoRearmDesc".Translate(),
                    hotKey = KeyBindingDefOf.Misc3,
                    icon = TexCommand.RearmTrap,
                    isActive = (Func<bool>)(() => !(bool)fi.GetValue(__instance)),
                    toggleAction = (Action)delegate
                    {
                        SetAutoRearmValue(__instance, !(bool)fi.GetValue(__instance));
                    }
                });
            }

            __result = li;
            return false;
        }
    }
}
