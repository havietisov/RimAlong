using Verse;
using RimWorld;

public class designator_area_allowed_expand_methods : common_patch_fields
{
    public static void parrot_designate_single_cell(IntVec3 c, Area z)
    {
        use_native = true;
        try
        {
            Area prev = Designator_AreaAllowed.SelectedArea;
            typeof(Designator_AreaAllowed).GetField("selectedArea", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, z);
            DesignatorUtility.FindAllowedDesignator<Designator_AreaAllowedExpand>().DesignateSingleCell(c);
            typeof(Designator_AreaAllowed).GetField("selectedArea", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, prev);
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
            parrot_designate_single_cell(c, Designator_AreaAllowed.SelectedArea);
            return false;
        }
    }
}
