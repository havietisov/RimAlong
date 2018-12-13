using Verse;
using RimWorld;
using System.Runtime.Serialization;
using System.Linq;
using Harmony;

[HarmonyPatch(typeof(Pawn_PlayerSettings), "set_AreaRestriction")]
public class Pawn_player_settings_allowed_area_patch : common_patch_fields
{
    public static void set_restriction(Pawn pawn, Area area)
    {
        use_native = true;
        try
        {
            pawn.playerSettings.AreaRestriction = area;
        }
        finally
        {
            use_native = false;
        }
    }

    [HarmonyPrefix]
    public static bool prefix(Pawn ___pawn, Area value)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            set_restriction(___pawn, value);
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
