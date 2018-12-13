using System.Collections.Generic;
using Verse;
using CooperateRim.Utilities;
using RimWorld;

public class designator_zone_methods : common_patch_fields
{
    public static void parrot_designate_multicell(IEnumerable<IntVec3> cells, System.Type designator_type, Zone z)
    {
        use_native = true;
        try
        {
            Designator_Zone _z = (Designator_Zone)typeof(DesignatorUtility).GetMethod(nameof(DesignatorUtility.FindAllowedDesignator)).MakeGenericMethod(designator_type).Invoke(null, null);
            var sel = Find.Selector.SelectedObjects;
            Find.Selector.ClearSelection();

            if (z != null)
            {
                Find.Selector.Select(z);
            }

            _z.DesignateMultiCell(cells);
            sel.ForEach(u => Find.Selector.Select(u, false, false));
        }
        finally
        {
            use_native = false;
        }
    }
    
    public static bool prefix_designate_multicell(IEnumerable<IntVec3> cells, Designator_ZoneAdd __instance)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            System.Type t = __instance.GetType();
            Zone z = Find.Selector.SelectedZone;
            RimLog.Message("zone type : " + __instance.GetType());
            
            {
                parrot_designate_multicell(cells, __instance.GetType(), z);
            }
            return false;
        }
    }
}
