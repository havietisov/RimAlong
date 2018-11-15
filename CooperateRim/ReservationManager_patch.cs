using Harmony;
using Verse;
using Verse.AI;

namespace CooperateRim
{
    [HarmonyPatch(typeof(ReservationManager), "Reserve")]
    public class ReservationManager_patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn claimant, LocalTargetInfo target, Job job, bool __result)
        {
            //if (SyncTickData.cliendID >= 0)
                //System.IO.File.AppendAllText("G:/CoopReplays/" + SyncTickData.cliendID + "/ReservationManager_reserve.txt", claimant.ToString() + " for  " + job + " at " + target + " and result is " + __result + "\r\n");
        }
    }
}
