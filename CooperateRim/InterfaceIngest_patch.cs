using Harmony;
using RimWorld;
using System.Reflection;
using Verse;
using CooperateRim.Utilities;

namespace CooperateRim
{
    [HarmonyPatch(typeof(ITab_Pawn_Gear), "InterfaceIngest")]
    public class InterfaceIngest_patch : common_patch_fields
    {
        public static void Ingest(Thing t, Pawn p)
        {
            use_native = true;
            try
            {
                RimLog.Message("thing : " + t);
                RimLog.Message("pawn : " + p);
                ThingWithComps thingWithComps = t as ThingWithComps;
                var objects = Find.Selector.SelectedObjects.ToArray();
                Rand.PushState(0);
                Find.Selector.ClearSelection();
                Find.Selector.Select(p, false, false);
                Rand.PopState();
                typeof(ITab_Pawn_Gear).GetMethod("InterfaceIngest", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(new ITab_Pawn_Gear(), new object[] { t });

                Rand.PushState(0);
                Find.Selector.ClearSelection();
                foreach (object o in objects)
                {
                    Find.Selector.Select(o, false, false);
                }
                Rand.PopState();
            }
            finally
            {
                use_native = false;
            }
        }

        [HarmonyPrefix]
        public static bool InterfaceIngest(Thing t, ITab_Pawn_Gear __instance)
        {
            if (use_native)
            {
                return true;
            }
            else
            {
                Pawn p = (Pawn)__instance.GetType().GetMethod("get_SelPawnForGear", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { });
                Ingest(t, p);
                return false;
            }
        }
    }
}
