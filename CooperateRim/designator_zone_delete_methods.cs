using Verse;
using RimWorld;

public class designator_zone_delete_methods : common_patch_fields
{
    public static void parrot_designate_single_cell(IntVec3 c, System.Type t)
    {
        use_native = true;
        try
        {
            Designator_ZoneDelete _z = (Designator_ZoneDelete)typeof(DesignatorUtility).GetMethod(nameof(DesignatorUtility.FindAllowedDesignator)).MakeGenericMethod(t).Invoke(null, null);
            _z.DesignateSingleCell(c);
        }
        finally
        {
            use_native = false;
        }
    }

    public static bool prefix_designate_single_cell(IntVec3 c, Designator_ZoneDelete __instance)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            parrot_designate_single_cell(c, __instance.GetType());
            return false;
        }
    }
}
