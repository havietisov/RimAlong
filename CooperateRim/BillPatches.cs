using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(ITab_Storage))]
    [HarmonyPatch("FillTab")]
    class BillPatches__
    {
        [HarmonyPrefix]
        public static void Prefix(ITab_Storage __instance)
        {
            ThingFilterPatch.thingFilterCallerStack.Push(__instance.GetType().GetMethod("get_SelStoreSettingsParent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(__instance, null));
        }

        [HarmonyPostfix]
        public static void Postfix(ITab_Storage __instance)
        {
            ThingFilterPatch.thingFilterCallerStack.Pop();
        }
    }

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
