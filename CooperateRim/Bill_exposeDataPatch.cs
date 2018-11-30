using Harmony;

namespace CooperateRim
{

    public partial class CooperateRimming
    {
        [HarmonyPatch(typeof(RimWorld.Bill), "ExposeData")]
        public class Bill_exposeDataPatch
        {
            [HarmonyPrefix]
            public static void Prefix(RimWorld.Bill __instance)
            {
                ThingFilterPatch.thingFilterCallerStack.Push(__instance);
            }
            
            [HarmonyPrefix]
            public static void Postfix()
            {
                ThingFilterPatch.thingFilterCallerStack.Pop();
            }
        };
    }
}
