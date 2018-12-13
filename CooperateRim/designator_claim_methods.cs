using Verse;
using RimWorld;
using System.Runtime.Serialization;
using System.Linq;
using Harmony;

[HarmonyPatch(typeof(Area), "Invert")]
public class area_invert : common_patch_fields
{
    public static void Invert(Area area)
    {
        use_native = true;
        try
        {
            area.Invert();
        }
        finally
        {
            use_native = false;
        }
    }

    [HarmonyPrefix]
    public static bool prefix(Area __instance)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            Invert(__instance);
            return false;
        }
    }
}

    [HarmonyPatch(typeof(Area), "Delete")]
public class area_delete : common_patch_fields
{
    public static void Delete(Area area)
    {
        use_native = true;
        try
        {
            area.Delete();
        }
        finally
        {
            use_native = false;
        }
    }

    [HarmonyPrefix]
    public static bool prefix(Area __instance)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            Delete(__instance);
            return false;
        }
    }
}

public class designator_claim_methods : common_patch_fields
{
    public static void parrot_designate_thing(Thing t)
    {
        use_native = true;
        try
        {
            DesignatorUtility.FindAllowedDesignator<Designator_Claim>().DesignateThing(t);
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
}
