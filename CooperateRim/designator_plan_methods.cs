using Verse;
using RimWorld;

public class designator_plan_methods : common_patch_fields
{
    public static bool prefix_designate_cell(IntVec3 c, DesignateMode ___mode)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            parrot_designate_cell(c, ___mode);
            return false;
        }
    }

    public static void parrot_designate_cell(IntVec3 c, DesignateMode mode)
    {
        use_native = true;
        try
        {
            if (mode == DesignateMode.Remove)
            {
                DesignatorUtility.FindAllowedDesignator<Designator_PlanRemove>().DesignateSingleCell(c);
            }
            else
            {
                DesignatorUtility.FindAllowedDesignator<Designator_PlanAdd>().DesignateSingleCell(c);
            }
        }
        finally
        {
            use_native = false;
        }
    }
}
