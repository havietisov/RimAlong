using Harmony;
using Verse;
using Verse.AI;

namespace CooperateRim
{
    [HarmonyPatch(typeof(HaulAIUtility), "HaulAsideJobFor")]
    public class haul_patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn p, Thing t, Job __result)
        {
            //if (SyncTickData.cliendID >= 0)
                //System.IO.File.AppendAllText("G:/CoopReplays/" + SyncTickData.cliendID + "/haul_aside_results.txt", p.ToString() + " for  " + t  + " and result is " + (__result == null ? "<:null:>" : __result.ToString() ) + "\r\n");
        }
    }
}
