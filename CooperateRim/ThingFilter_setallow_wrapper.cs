using Harmony;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(ThingFilter), "SetAllow", new System.Type[] { typeof(ThingDef), typeof(bool) })]
    public class ThingFilter_setallow_wrapper
    {
        [HarmonyPrefix]
        public static bool Prefix(ThingFilter __instance, ThingDef thingDef, bool allow)
        {
            {
                //Utilities.RimLog.Message("ThingFilterPatch.avoidThingFilterUsage == " + ThingFilterPatch.avoidThingFilterUsage + "|" + thingfilter_methods.avoidInternalLoop);
                if (!thingfilter_methods.avoidInternalLoop && !ThingFilterPatch.avoidThingFilterUsage)
                {
                    thingfilter_methods.SetAllowance(ThingFilterPatch.thingFilterCallerStack.Peek(), def: thingDef, isAllow: allow, isSpecial: false);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
