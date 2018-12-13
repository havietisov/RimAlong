using Verse;
using System.Reflection;
using RimWorld;

public class designator_build_methods : common_patch_fields
{
    public static void parrot_designate_single_cell(IntVec3 c, ThingDef stuffDef, Rot4 placingRot, BuildableDef entDef)
    {
        use_native = true;
        try
        {
            var designator = BuildCopyCommandUtility.FindAllowedDesignator(entDef, false);
            designator.SetStuffDef(stuffDef);
            designator.GetType().GetField("placingRot", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(designator, placingRot);
            designator.DesignateSingleCell(c);
        }
        finally
        {
            use_native = false;
        }
    }

    public static bool prefix_designate_single_cell(IntVec3 c, Rot4 ___placingRot, BuildableDef ___entDef, ThingDef ___stuffDef)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            parrot_designate_single_cell(c, ___stuffDef, ___placingRot, ___entDef);
            return false;
        }
    }
}
