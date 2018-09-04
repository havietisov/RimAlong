using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CooperateRim
{
    /*
    [HarmonyPatch(typeof(Verse.ZoneManager))]
    [HarmonyPatch("RegisterZone")]
    public class ZoneManagerPatch_register
    {
        
        [HarmonyPrefix]
        public static bool Prefix(Zone newZone)
        {
            CooperateRimming.Log("new zone : " + newZone.label);
            if (SyncTickData.AvoidLoop)
            {
                return false;
            }
            else
            {
                SyncTickData.AppendSyncTickData(newZone);
                return false;
            }
        }
    }*/

    [HarmonyPatch(typeof(Zone))]
    [HarmonyPatch("AddCell")]
    public class ZoneManagerPatch_deregister
    {
        [HarmonyPrefix]
        public static bool AddCell(IntVec3 c, ref Zone __instance)
        {
            if (!SyncTickData.AvoidLoop)
            {
                CooperateRimming.Log(__instance.label + " : " + c);
                SyncTickData.AppendSyncTickData(__instance.ID, c, true);
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(ZoneManager))]
    [HarmonyPatch("RegisterZone")]
    public class ZoneManagerPatch_RegisterZone
    {
        [HarmonyPrefix]
        public static bool RegisterZone(ref Zone newZone)
        {
            if (!SyncTickData.AvoidLoop)
            {
                CooperateRimming.Log(newZone.GetType().ToString());
                SyncTickData.AppendSyncTickData(newZone.ID, newZone.label, newZone.GetType());
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    //DesignateMultiCell
    //Designator_ZoneAdd

    //AddHaulDestination
    //Notify_HaulDestinationChangedPriority
    //HaulDestinationManager
}
