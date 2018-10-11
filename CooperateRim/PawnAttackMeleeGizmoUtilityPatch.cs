using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace CooperateRim
{
    /*
    [HarmonyPatch(typeof(PawnAttackGizmoUtility))]
    [HarmonyPatch("GetMeleeAttackGizmo")]
    public class PawnAttackMeleeGizmoUtilityPatch
    {
        [HarmonyPrefix]
        public bool Prefix(Pawn pawn, Gizmo __result, MethodInfo __originalMethod)
        {
            if (SyncTickData.AvoidLoop)
            {
                Gizmo g = __originalMethod.Invoke(null, new object[] { pawn }) as Gizmo;
                Command_Target gt = g as Command_Target;
                gt.action
                return false;
            }
            else
            {
                return true;
            }
        }
    }*/
}
