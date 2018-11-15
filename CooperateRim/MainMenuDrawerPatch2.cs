using Harmony;
using RimWorld;
using System;
using UnityEngine;

namespace CooperateRim
{

    public partial class CooperateRimming
    {
        [HarmonyPatch(typeof(MainMenuDrawer))]
        [HarmonyPatch("DoMainMenuControls")]
        [HarmonyAfter]
        [HarmonyPatch(new Type[] { typeof(Rect), typeof(bool) })]
        class MainMenuDrawerPatch2
        {
            static void Prefix(Rect rect, bool anyMapFiles)
            {
                GUI.EndGroup();
            }
        }
    }
}
