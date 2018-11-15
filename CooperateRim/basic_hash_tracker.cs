using Harmony;
using System.Diagnostics;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(RandomNumberGenerator_BasicHash), "GetInt")]
    public class basic_hash_tracker
    {
        [HarmonyPostfix]
        public static void Postfix(uint iterations, ref int __result, uint ___seed)
        {
            if (SyncTickData.cliendID >= 0 && Find.CurrentMap != null && Find.TickManager.TicksGame > 0)
            {
                StackTrace st = new StackTrace();
                string s = "=======BEGIN========\r\n";
                foreach (var f in st.GetFrames())
                {
                    s += f.GetMethod().ReflectedType + " (" + f.GetMethod().DeclaringType + ")::" + f.GetMethod().Name + "\r\n";
                }

                s += "=======END========";

                //System.IO.File.AppendAllText("Z:/CoopReplays/" + SyncTickData.cliendID + "/" +"tick_" + Find.TickManager.TicksGame + "_seed_" + ___seed + "_iter_" + iterations + "_res_" + __result + ".txt", s);
            }
        }
    }
}
