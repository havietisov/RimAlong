using Verse;
using RimWorld;

public class designator_area_buildoof_methods : common_patch_fields
{
    public static void parrot_designate_single_cell(IntVec3 c)
    {
        use_native = true;
        try
        {
            DesignatorUtility.FindAllowedDesignator<Designator_AreaBuildRoof>().DesignateSingleCell(c);
        }
        finally
        {
            use_native = false;
        }
    }

    public static bool prefix_designate_single_cell(IntVec3 c)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            parrot_designate_single_cell(c);
            return false;
        }
    }
}
