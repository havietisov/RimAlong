using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using System.Diagnostics;

namespace CooperateRim
{
    /*[HarmonyPatch(typeof(Verse.Designator))]
    [HarmonyPatch("DesignateSingleCell")]*/
    public class DesignatorPatch
    {
        /*
        [HarmonyPrefix]*/
        public static bool DesignateSingleCell_1(ref Designator __instance, IntVec3 c)
        {
            CooperateRimming.Log("DesignateSingleCell_1");
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

        public static bool DesignateSingleCell_2(ref Designator __instance, IntVec3 loc)
        {
            CooperateRimming.Log("DesignateSingleCell_2");
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
    }


    [HarmonyPatch(typeof(Designator_ZoneAdd))]
    [HarmonyPatch("DesignateMultiCell")]
    public class Designator_zoneAdd
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Designator __instance, IEnumerable<IntVec3> cells)
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
    }

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

    /*
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

[HarmonyPatch(typeof(Designator_ZoneAddStockpile))]
[HarmonyPatch("MakeNewZone")]
public class Designator_ZoneAdd_patch
{
    [HarmonyPrefix]
    public static bool MakeNewZone()
    {
        if (SyncTickData.AvoidLoop)
        {
            return true;
        }
        else
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            SyncTickData.AppendSyncTickDataWithNewZone(sf.GetMethod().DeclaringType.AssemblyQualifiedName);
            return false;
        }
    }
}*/

    //DesignateMultiCell
    //Designator_ZoneAdd

    //AddHaulDestination
    //Notify_HaulDestinationChangedPriority
    //HaulDestinationManager
}
