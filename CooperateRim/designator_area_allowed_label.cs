using RimWorld;
using Harmony;

[HarmonyPatch(typeof(Area_Allowed), "SetLabel")]
public class designator_area_allowed_label : common_patch_fields
{
    public static void setLabel(Area_Allowed area, string label)
    {
        use_native = true;
        try
        {
            area.SetLabel(label);
        }
        finally
        {
            use_native = false;
        }
    }

    [HarmonyPrefix]
    public static bool prefix(Area_Allowed __instance, string label)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            setLabel(__instance, label);
            return false;
        }
    }
}
