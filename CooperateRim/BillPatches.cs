using CooperateRim.Utilities;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Dialog_BillConfig))]
    [HarmonyPatch("DoWindowContents")]
    class BillPatches
    {
        [HarmonyPrefix]
        public static void Prefix(Dialog_BillConfig __instance)
        {
            ThingFilterPatch.thingFilterCallerStack.Push(__instance.GetType().GetField("bill", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(__instance));
        }

        [HarmonyPostfix]
        public static void Postfix(Dialog_BillConfig __instance)
        {
            ThingFilterPatch.thingFilterCallerStack.Pop();
        }
    }
}
