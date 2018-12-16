using Harmony;
using RimWorld;

namespace CooperateRim
{
    [HarmonyPatch(typeof(ITab_Storage))]
    [HarmonyPatch("FillTab")]
    class itab_storage_fill_patch
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
}
