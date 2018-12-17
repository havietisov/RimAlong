using Harmony;
using RimWorld;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Pawn_TimetableTracker), "SetAssignment")]
    class pawnTimetableAssignPatch : common_patch_fields
    {
        public static void set_assignment(Pawn p, int hour, TimeAssignmentDef ta)
        {
            use_native = true;
            try
            {
                p.timetable.SetAssignment(hour, ta);
            }
            finally
            {
                use_native = false;
            }
        }

        [HarmonyPrefix]
        public static bool Prefix(Pawn ___pawn, int hour, TimeAssignmentDef ta)
        {
            if (use_native)
            {
                return true;
            }
            else
            {
                set_assignment(___pawn, hour, ta);
                return false;
            }
        }
    }
}
