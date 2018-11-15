using Harmony;
using RimWorld;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Storyteller), "StorytellerTick")]
    public class StorytellerTick_patch
    {
        [HarmonyPrefix]
        public static bool prefix()
        {
            return false;//disables storyteller
        }
    }
}
