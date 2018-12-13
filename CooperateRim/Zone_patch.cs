using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CooperateRim
{
    //[HarmonyPatch(typeof(Verse.Zone))]
    //[HarmonyPatch("Delete")]
    //class Zone_patch
    //{
    //    [HarmonyPrefix]
    //    public static bool Prefix(Zone __instance)
    //    {
    //        if (SyncTickData.AvoidLoop)
    //        {

    //            return true;
    //        }
    //        else
    //        {
    //            SyncTickData.AppendSyncTickDataDeleteZone(__instance);
    //            return false;
    //        }
    //    }
    //}
    /*
    [HarmonyPatch(typeof(Verse.Command_Action))]
    [HarmonyPatch("ProcessInput")]
    class Command_Action_
    {
        [HarmonyPrefix]
        public static void Prefix(Command_Action __instance)
        {
            CooperateRimming.RimLog.Message(__instance.action.Method.DeclaringType.Name + "::" +__instance.action.Method.Name.ToString());
        }
    }*/
}
