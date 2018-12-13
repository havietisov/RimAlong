using Harmony;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Zone), "Delete")]
    public class zone_delete_patch
    {
        static bool use_native;

        public static void parrot(Zone inst)
        {
            use_native = true;
            try
            {
                inst.Delete();
            }
            finally
            {
                use_native = false;
            }
        }

        [HarmonyPrefix]
        public static bool prefix(Zone __instance)
        {
            if (use_native)
            {
                return true;
            }
            else
            {
                parrot(__instance);
                return false;
            }
        }
    }
}
