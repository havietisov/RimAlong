using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Zone_Growing), "SetPlantDefToGrow")]
    public class Zone_GrowingPatch
    {
        [HarmonyPrefix]
        public static bool SetPlantDefToGrow(Zone_Growing __instance, ThingDef plantDef)
        {
            if (SyncTickData.AvoidLoop)
            {
                return true;
            }
            else
            {
                SyncTickData.AppendSyncTickData(__instance, plantDef);
                return false;
            }
        }
    }
}
