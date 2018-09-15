using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Designator_AreaBuildRoof))]
    [HarmonyPatch("DesignateSingleCell")]
    class Designator_AreaBuildRoof_patch
    {
        [HarmonyPrefix]
        public static bool DesignateSingleCell(ref Designator_AreaBuildRoof __instance, ref IntVec3 c)
        {
            CooperateRimming.Log("Designator_AreaBuildRoof.DesignateSingleCell");
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickDataArea(__instance, c);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
