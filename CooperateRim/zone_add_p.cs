using Harmony;
using RimWorld;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Designator_ZoneAdd), "SelectedUpdate")]
    public class zone_add_p
    {
        [HarmonyPrefix]
        public static bool prefix()
        {
            CooperateRimming.Log(">>>>>>>>>>>>>>>>>>>>>>> selected update bullshit");
            return false;
        }
    }
}
