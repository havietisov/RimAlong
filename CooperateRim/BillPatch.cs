using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Bill))]
    [HarmonyPatch("InitializeAfterClone")]
    public class BillPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Bill __instance)
        {
            ReverseBillTable.Associate(__instance, __instance.billStack);
        }
    }
}
