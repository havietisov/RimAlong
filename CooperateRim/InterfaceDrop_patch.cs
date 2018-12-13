using Harmony;
using RimWorld;
using System.Reflection;
using Verse;
using Verse.AI;
using CooperateRim.Utilities;

namespace CooperateRim
{
    [HarmonyPatch(typeof(ITab_Pawn_Gear), "InterfaceDrop")]
    public class InterfaceDrop_patch
    {
        static bool avoidLoop = true;

        public static void DropGear(Thing t, Pawn p)
        {
            RimLog.Message("thing : " + t);
            RimLog.Message("pawn : " + p);
            ThingWithComps thingWithComps = t as ThingWithComps;
            Apparel apparel = t as Apparel;

            if (apparel != null)
            {
                if (apparel != null && p.apparel != null && p.apparel.WornApparel.Contains(apparel))
                {
                    p.jobs.TryTakeOrderedJob(new Job(JobDefOf.RemoveApparel, apparel), JobTag.Misc);
                }
            }

            if (thingWithComps != null)
            {
                if (p.equipment != null && p.equipment.AllEquipmentListForReading.Contains(thingWithComps))
                {
                    p.jobs.TryTakeOrderedJob(new Job(JobDefOf.DropEquipment, thingWithComps), JobTag.Misc);
                }
            }

            if (!t.def.destroyOnDrop)
            {
                Thing thing;
                p.inventory.innerContainer.TryDrop(t, p.Position, p.Map, ThingPlaceMode.Near, out thing, null, null);
            }
        }

        [HarmonyPrefix]
        public static bool InterfaceDrop(Thing t, ITab_Pawn_Gear __instance)
        {
            if (avoidLoop)
            {
                Pawn p = (Pawn)__instance.GetType().GetMethod("get_SelPawnForGear", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { });
                DropGear(t, p);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
