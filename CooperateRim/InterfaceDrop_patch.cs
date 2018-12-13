using Harmony;
using RimWorld;
using System.Reflection;
using Verse;
using Verse.AI;
using CooperateRim.Utilities;

namespace CooperateRim
{
    [HarmonyPatch(typeof(ITab_Pawn_Gear), "InterfaceDrop")]
    public class InterfaceDrop_patch : common_patch_fields
    {
        public static void DropGear(Thing t, Pawn p)
        {
            use_native = true;
            try
            {
                RimLog.Message("thing : " + t);
                RimLog.Message("pawn : " + p);
                ThingWithComps thingWithComps = t as ThingWithComps;
                Find.Selector.Select(p, false, false);
                typeof(ITab_Pawn_Gear).GetMethod("InterfaceDrop", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(new ITab_Pawn_Gear(), new object[] { t });
            }
            finally
            {
                use_native = false;
            }
        }

        [HarmonyPrefix]
        public static bool InterfaceDrop(Thing t, ITab_Pawn_Gear __instance)
        {
            if (use_native)
            {
                return true;
            }
            else
            {
                Pawn p = (Pawn)__instance.GetType().GetMethod("get_SelPawnForGear", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { });
                DropGear(t, p);
                return false;
            }
        }
    }
}
