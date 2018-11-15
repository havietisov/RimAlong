using Harmony;
using System;
using System.Collections.Generic;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Rand), "get_Value", new Type[] { })]
    public class getValuePatch
    {
        public static int rand_guard = 0;

        public static List<string> diagData = new List<string>();

        public static void GuardedPush()
        {
            getValuePatch.rand_guard++;
            Verse.Rand.PushState();
        }

        public static void GuardedPop()
        {
            Verse.Rand.PopState();
            getValuePatch.rand_guard--;
        }

        public static void SendDiagDataToServer()
        {
            string s = "";
            int cid = SyncTickData.cliendID;

            foreach (var a in diagData)
            {
                s += a;
            }

            if (s.Length > 0)
            {
                //System.IO.File.WriteAllText("G:\\CoopReplays\\" + (cid + "_tick_" + Find.TickManager.TicksGame) + ".txt", s);
            }
        }

        static int clid = 0;

        public static int getclid()
        {
            return clid++;
        }
        
        [HarmonyPostfix]
        public static void get_Value(ref uint ___iterations, ref float __result)
        {
            if (Current.Game != null && Find.TickManager.TicksGame > 0)
            {

                //int tick = Current.Game == null ? -1 : Find.TickManager.TicksGame;
                //StackTrace tr = new StackTrace();
                //string s = "";

                bool bb = false;
                
                
                {
                    //foreach (var b in tr.GetFrames())
                    {
                        //System.IO.File.AppendAllText("G:\\CoopReplays\\" + (SyncTickData.cliendID + "_tick_" + Find.TickManager.TicksGame) + ".txt", b.GetMethod().Name + "::" + b.GetMethod().ReflectedType + " || " + "\r\n");
                    }
                }
                /*
                foreach (var frame in tr.GetFrames())
                {
                    streamholder.WriteLine(frame.GetMethod().ReflectedType + "::" + frame.GetMethod().Name, "state");
                }*/
                
                //CooperateRimming.Log(">>>>>>>>>>>>>>> FOUL RAND CALL");
            }
            //if (CooperateRimming.dumpRand)
            {
                /*
                int tick = Current.Game == null ? -1 : Find.TickManager.TicksGame;
                StackTrace tr = new StackTrace();
                streamholder.WriteLine("====STACK====", "state");

                foreach (var frame in tr.GetFrames())
                {
                    streamholder.WriteLine(frame.GetMethod().ReflectedType + "::" + frame.GetMethod().Name, "state");
                }

                streamholder.WriteLine("====END====", "state");
                streamholder.WriteLine(__result + " at  iter  " + ___iterations + " at tick " + tick, "state");*/
            }
        }
    }
}
