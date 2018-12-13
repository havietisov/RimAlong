using Verse;
using CooperateRim.Utilities;
using RimWorld;
using Harmony;

[HarmonyPatch(typeof(Zone_Growing), "SetPlantDefToGrow")]
public class Zone_growing_setplantdef_patch
{
    static bool use_native = false;

    public static void parrot(ThingDef plantDef, Zone_Growing inst)
    {
        use_native = true;
        try
        {
            inst.SetPlantDefToGrow(plantDef);
        }
        finally
        {
            use_native = false;
        }
    }

    [HarmonyPrefix]
    public static bool prefix(ThingDef plantDef, Zone_Growing __instance)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            parrot(plantDef, __instance);
            return false;
        }
    }
}
