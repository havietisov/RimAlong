using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(MainTabWindow_Work), "DoManualPrioritiesCheckbox")]
    class MainTabWindow_Work_patch
    {
        public static void ChangeUseWorkPriorities(bool val)
        {
            Current.Game.playSettings.useWorkPriorities = val;

            foreach (Pawn pawn in PawnsFinder.AllMapsWorldAndTemporary_Alive)
            {
                if (pawn.Faction == Faction.OfPlayer && pawn.workSettings != null)
                {
                    pawn.workSettings.Notify_UseWorkPrioritiesChanged();
                }
            }
        }

        [HarmonyPrefix]
        public static bool Prefix()
        {
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect = new Rect(5f, 5f, 140f, 30f);
            bool useWorkPriorities = Current.Game.playSettings.useWorkPriorities;
            bool useWorkPriorities_ = Current.Game.playSettings.useWorkPriorities;//Current.Game.playSettings.useWorkPriorities
            Widgets.CheckboxLabeled(rect, "ManualPriorities".Translate(), ref useWorkPriorities_, false, null, null, false);
            if (useWorkPriorities != useWorkPriorities_)
            {
                ChangeUseWorkPriorities(useWorkPriorities_);
            }
            if (!Current.Game.playSettings.useWorkPriorities)
            {
                UIHighlighter.HighlightOpportunity(rect, "ManualPriorities-Off");
            }
            return false;
        }
    }
}
