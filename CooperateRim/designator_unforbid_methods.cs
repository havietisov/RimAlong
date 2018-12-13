using Verse;
using RimWorld;

public class designator_unforbid_methods : common_patch_fields
{
    public static void parrot_designate_thing(Thing t)
    {
        use_native = true;
        try
        {
            DesignatorUtility.FindAllowedDesignator<Designator_Unforbid>().DesignateThing(t);
        }
        finally
        {
            use_native = false;
        }
    }

    public static void parrot_designate_cell(IntVec3 c)
    {
        use_native = true;
        try
        {
            DesignatorUtility.FindAllowedDesignator<Designator_Unforbid>().DesignateSingleCell(c);
        }
        finally
        {
            use_native = false;
        }
    }

    public static bool prefix_designate_thing(Thing t)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            parrot_designate_thing(t);
            return false;
        }
    }

    public static bool prefix_designate_cell(IntVec3 c)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            parrot_designate_cell(c);
            return false;
        }
    }
}
