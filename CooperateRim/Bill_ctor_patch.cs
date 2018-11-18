using Harmony;
using RimWorld;
using System;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Bill), MethodType.Constructor, new Type[] { typeof(RecipeDef) })]
    class Bill_ctor_patch
    {
        [HarmonyPrefix]
        public static void Prefix(Bill __instance)
        {
            ThingFilterPatch.avoidThingFilterUsage = true;
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            ThingFilterPatch.avoidThingFilterUsage = false;
        }
    }
}
