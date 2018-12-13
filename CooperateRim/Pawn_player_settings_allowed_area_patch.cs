using Verse;
using RimWorld;
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
