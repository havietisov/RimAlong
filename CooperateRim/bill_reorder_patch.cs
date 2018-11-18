using Harmony;
using RimWorld;

namespace CooperateRim
{
    [HarmonyPatch(typeof(BillStack))]
    [HarmonyPatch("Reorder")]
    public class bill_reorder_patch
    {
        static bool avoid_loop_internal = false;

        public static void ReorderAt(BillStack stack, int index, int offset)
        {
            avoid_loop_internal = true;
            try
            {
                stack.Reorder(stack.Bills[index], offset);
            }
            finally
            {
                avoid_loop_internal = false;
            }
        }

        [HarmonyPrefix]
        public static bool Reorder(ref Bill bill, BillStack __instance, int offset)
        {
            if (avoid_loop_internal)
            {
                return true;
            }
            else
            {
                ReorderAt(__instance, __instance.Bills.IndexOf(bill), offset);
                return false;
            }
        }
    }
}
