using CooperateRim.Utilities;
using Harmony;
using RimWorld;
using System.Runtime.CompilerServices;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(MainTabWindow_Research), "DoWindowContents", new System.Type[] { typeof(UnityEngine.Rect) })]
    public class MainTabWindow_Research_patch : common_patch_fields
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void parrotSetCurrentResearch(ResearchProjectDef project)
        {
            Find.ResearchManager.currentProj = project;
        }

        [HarmonyPrefix]
        public static void prefix(ref ResearchProjectDef __state)
        {
            __state = Find.ResearchManager.currentProj;
        }

        [HarmonyPostfix]
        public static void postfix(ref ResearchProjectDef __state)
        {
            if (Find.ResearchManager.currentProj != __state)
            {
                ResearchProjectDef buffered = Find.ResearchManager.currentProj;
                Find.ResearchManager.currentProj = __state;
                parrotSetCurrentResearch(buffered);
            }
        }
    }
}
