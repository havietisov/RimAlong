using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.AI;

namespace CooperateRim
{
    [HarmonyPatch(typeof(JobGiver_MoveToStandable))]
    [HarmonyPatch("TryGiveJob")]
    class IdleJobGiverPatch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            CooperateRimming.Log("idle job hits!");
        }
    }
}
