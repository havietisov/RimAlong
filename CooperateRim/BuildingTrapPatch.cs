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

            //if (__instance.Faction == Faction.OfPlayer && __instance.def.blueprintDef != null && __instance.def.IsResearchFinished)
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
            /*
            FieldInfo fi = @this.GetType().GetField("autoRearm", BindingFlags.NonPublic | BindingFlags.Instance);
            SubIter((Building)*/
            return false;
        }
    }
}
