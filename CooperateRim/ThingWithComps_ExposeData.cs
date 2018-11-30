using Harmony;
using Verse;

namespace CooperateRim
{

    public partial class CooperateRimming
    {
        [HarmonyPatch(typeof(ThingWithComps), "ExposeData")]
        public class ThingWithComps_ExposeData
        {
            [HarmonyPrefix]
            public static void Prefix(ThingWithComps __instance)
            {
                ThingFilterPatch.thingFilterCallerStack.Push(__instance);
            }
            
            [HarmonyPrefix]
            public static void Postfix()
            {
                ThingFilterPatch.thingFilterCallerStack.Pop();
            }
        }
    }
}
