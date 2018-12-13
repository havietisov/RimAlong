using Verse;
using CooperateRim.Utilities;
using System.Reflection;
using RimWorld;

public class designator_install_methods : designator_build_methods
{
    public static void parrot_designate_single_cell(IntVec3 c, Rot4 placingRot, Thing thingToInstall)
    {
        use_native = true;
        try
        {
            var sel = Find.Selector.SelectedObjects;
            Find.Selector.ClearSelection();
            Designator_Install ins = (Designator_Install)DesignatorUtility.FindAllowedDesignator<Designator_Install>();
            var field = ins.GetType().GetField("placingRot", BindingFlags.NonPublic | BindingFlags.Instance);
            RimLog.Message("thing : " + thingToInstall);
            RimLog.Message("field : " + field);
            field.SetValue(ins, placingRot);
            Rand.PushState(0);
            Find.Selector.Select(thingToInstall, false, false);
            ins.DesignateSingleCell(c);
            Find.Selector.ClearSelection();
            sel.ForEach(u => Find.Selector.Select(u, false, false));
        }
        finally
        {
            use_native = false;
        }
    }

    public static bool prefix_designate_single_cell(IntVec3 c, Rot4 ___placingRot, Designator_Install __instance)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            Thing thingToInstall = (Thing)__instance.GetType().GetMethod("get_MiniToInstallOrBuildingToReinstall", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { });
            parrot_designate_single_cell(c, ___placingRot, thingToInstall);
            return false;
        }
    }

}
