using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CooperateRim
{
    //Pawn_WorkSettings
    [HarmonyPatch(typeof(Pawn_WorkSettings))]
    [HarmonyPatch("SetPriority")]
    class Pawn_WorkSettings_patch
    {
        public class InternalState
        {
            public int? oldPrio;
        }

        [HarmonyPrefix]
        public static bool SetPriority(WorkTypeDef w, int priority, ref DefMap<WorkTypeDef, int> ___priorities, ref InternalState __state, ref Pawn ___pawn)
        {
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickData(w, priority, ___pawn);
                return false;
            }
            else
            {
                return true;
            }
        }

        [HarmonyPostfix]
        public static void __SetPriority(WorkTypeDef w, int priority, ref bool ___workGiversDirty, ref DefMap<WorkTypeDef, int> ___priorities, ref InternalState __state)
        {

        }
    }
}
