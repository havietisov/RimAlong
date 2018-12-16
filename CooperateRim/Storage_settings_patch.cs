using Harmony;
using RimWorld;
using System.Runtime.CompilerServices;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(StorageSettings))]
    [HarmonyPatch("set_Priority")]
    class Storage_settings_patch : common_patch_fields
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void set_storage_priority(object obj, StoragePriority priority)
        {
            use_native = true;
            try
            {
                if (obj is ThingWithComps)
                {
                    (obj as ThingWithComps).def.building.defaultStorageSettings.Priority = priority;
                }

                if (obj is Zone_Stockpile)
                {
                    (obj as Zone_Stockpile).settings.Priority = priority;
                }
            }
            finally

            {
                use_native = false;
            }
        }

        [HarmonyPrefix]
        public static bool prefix(StoragePriority value, IStoreSettingsParent ___owner)
        {
            if (use_native)
            {
                return true;
            }
            else
            {
                set_storage_priority(___owner, value);
                return false;
            }
        }
    }
}
