using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Verse.DesignationManager))]
    [HarmonyPatch("AddDesignation")]
    class DesignationManagerPatch
    {
        [HarmonyPrefix]
        public static bool AddDesignation(ref Designation newDes)
        {
            if (!SyncTickData.AvoidLoop)
            {
                CooperateRimming.Log("AddDesignation++");
                SyncTickData.AppendSyncTickData(newDes);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
