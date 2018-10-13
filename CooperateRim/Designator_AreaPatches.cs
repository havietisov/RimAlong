using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CooperateRim
{
    //[HarmonyPatch(typeof(Designator_AreaBuildRoof))]
    //[HarmonyPatch("DesignateSingleCell")]
    class Designator_patch
    {
        //[HarmonyPrefix]
        public static bool DesignateSingleCell_1(ref Designator __instance, ref IntVec3 c)
        {
            CooperateRimming.Log(__instance.GetType().AssemblyQualifiedName + ".DesignateSingleCell");
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickData(__instance, c);
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool DesignateSingleCell_2(ref Designator __instance, ref IntVec3 loc)
        {
            CooperateRimming.Log(__instance.GetType().AssemblyQualifiedName + ".DesignateSingleCell");
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickData(__instance, loc);
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool DesignateMultiCell(ref Designator __instance, IEnumerable<IntVec3> cells)
        {
            CooperateRimming.Log("DesignateMultiCell_1");
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickData(__instance, cells);
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool DesignateThing(Designator __instance, Thing t)
        {
            CooperateRimming.Log(__instance.GetType().AssemblyQualifiedName + ".DesignateThing");
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickDesignatorApplyToThing(__instance, t, t.Position);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
