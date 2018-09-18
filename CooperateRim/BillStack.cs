using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CooperateRim
{
    [HarmonyPatch(typeof(RimWorld.BillStack))]
    [HarmonyPatch("AddBill")]
    public class BillStackPatch
    {
        [HarmonyPrefix]
        public static bool AddBill(ref Bill bill, BillStack __instance)
        {
            if (!SyncTickData.AvoidLoop)
            {
                CooperateRimming.Log(">>>>>>>>>>>>> " + __instance.billGiver.ToString());
                SyncTickData.AppendSyncTickData(bill, __instance.billGiver);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
