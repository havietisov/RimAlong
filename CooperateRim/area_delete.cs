using Verse;
using Harmony;

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
