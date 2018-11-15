using Harmony;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(GenClosest), "ClosestThing_Global_Reachable")]
    public class closest_tracker
    {
        [HarmonyPostfix]
        public static void Postfix(IntVec3 center, Thing  __result)
        {
            //if(SyncTickData.cliendID >= 0)
                //System.IO.File.AppendAllText("G:/CoopReplays/" + SyncTickData.cliendID + "/gen_closest.txt", center.ToString() + " :: " + (__result == null ? "<:null:>" : __result.ToString() ) + "\r\n");
        }
    }
}
