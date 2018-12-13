using Verse;
using CooperateRim.Utilities;
using RimWorld;

public class designator_cancel_methods : common_patch_fields
{
    public static void parrot_designate_single_cell(IntVec3 c)
    {
        use_native = true;
        try
        {
            DesignatorUtility.FindAllowedDesignator<Designator_Cancel>().DesignateSingleCell(c);
        }
        finally
        {
            use_native = false;
        }
    }

    public static void parrot_designate_thing(Thing t)
    {
        use_native = true;
        try
        {
            DesignatorUtility.FindAllowedDesignator<Designator_Cancel>().DesignateThing(t);
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

    public static bool prefix_designate_thing(Thing t)
    {
        if (use_native)
        {
            RimLog.Message("+cancel on thing " + t.ThingID);
            return true;
        }
        else
        {
            RimLog.Message("-cancel on thing " + t.ThingID);
            parrot_designate_thing(t);
            return false;
        }
    }
}
