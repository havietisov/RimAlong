using Verse;
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
