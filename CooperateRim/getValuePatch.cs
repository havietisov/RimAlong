using CooperateRim.Utilities;
using Harmony;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Rand), "MTBEventOccurs")]
    public class MTBEventOccurs_patch
    {
        public class mtbdata
        {
            public int tickID;
            public float mtb;
            public float mtbUnit;
            public float checkDuration;
            public bool __result;
            public Type ctx;
            public StackFrame[] context;
        }

        public static List<mtbdata> mtb_dump = new List<mtbdata>();

        [HarmonyPostfix]
        public static void Postfix(float mtb, float mtbUnit, float checkDuration, bool __result)
        {
            if (Current.Game != null && Find.TickManager.TicksGame > 0)
            {
                mtb_dump.Add(new mtbdata() { checkDuration = checkDuration, mtb = mtb, mtbUnit = mtbUnit, __result = __result, ctx = RandContextCounter.currentContextName, tickID = Find.TickManager.TicksGame, context = new StackTrace().GetFrames() });
            }
        }
    }

    [HarmonyPatch(typeof(Rand), "get_Value", new Type[] { })]
    public class getValuePatch
    {
        public static bool diagnostics = true;

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

        [Serializable]
        public struct diagdata
        {
            public int frame;
            public string trace;
            public Type context;
            public int clientID;
            public int randomValue;
            public uint iteration;
        }
       

        public static List<diagdata> framelistForTicks = new List<diagdata>();

        public static void ClearFramelistData()
        {
            framelistForTicks.Clear();
        }

        public static void DumpFramelistData(System.IO.Stream s)
        {
            List<diagdata> fdls = framelistForTicks;
            RimLog.Message("framelistForTicks :: "+ framelistForTicks.Count);

            PirateRPC.PirateRPC.SendInvocation(s, (u) => 
            {
                Console.WriteLine("dumping data " + fdls.Count);

                foreach (var kv in fdls)
                {
                    if (!string.IsNullOrEmpty(kv.trace))
                    {
                        string str = kv.trace;

                        /*
                        foreach (StackFrame sf in kv.trace)
                        {
                            str += sf.ToString() + "\r\n";
                        }*/

                        System.IO.File.WriteAllText("C:/CoopReplays/" + kv.clientID + "/" + kv.frame + "_" + kv.context + "_" + kv.iteration + ".txt", str);
                    }
                }
            });
        }

        

        [HarmonyPostfix]
        public static void get_Value(ref uint ___iterations, ref float __result, RandomNumberGenerator ___random)
        {
            if(!diagnostics)
            {
                return;
            }

            if (Current.Game != null && Find.TickManager.TicksGame > 0)
            {
                //if (RandContextCounter.currentContextName == null)
                {
                    /*
                    StackTrace st = new StackTrace();

                    Utilities.RimLog.Message("=========== outside of context rand!==========");

                    foreach (var a in st.GetFrames())
                    {
                        Utilities.RimLog.Message(a.GetMethod().Name + "::" + a.GetMethod().DeclaringType);
                    }

                    Utilities.RimLog.Message("===========\\outside of context rand!==========");*/
                }
                //else
                {
                    
                    if (RandContextCounter.currentContextName != typeof(CooperateRimming.MoteBullshit_placeholder))
                    {
                        StackTrace st = new StackTrace();
                        string str = "";
                        string mtbstr = "";

                        foreach (StackFrame sf in st.GetFrames())
                        {
                            str += sf.GetMethod().Name + "::" + sf.GetMethod().DeclaringType + "\r\n";
                        }

                        foreach (var sf in MTBEventOccurs_patch.mtb_dump)
                        {
                            mtbstr += "=============[" + sf.ctx + "]=============\r\n";
                            
                            foreach (StackFrame sf_ in sf.context)
                            {
                                mtbstr += sf_.GetMethod().Name + "::" + sf_.GetMethod().DeclaringType + "\r\n";
                            }
                            
                            mtbstr +=  sf.mtb + ":" + sf.mtbUnit + ":" + sf.checkDuration + ":" + sf.__result + "\r\n";
                            mtbstr += "\\===========\r\n";
                        }

                        framelistForTicks.Add(new diagdata() { clientID = SyncTickData.cliendID, context = RandContextCounter.currentContextName, frame = Find.TickManager.TicksGame, iteration = ___iterations, trace = str });
                        framelistForTicks.Add(new diagdata() { clientID = SyncTickData.cliendID, context = typeof(MTBEventOccurs_patch), frame = Find.TickManager.TicksGame, iteration = ___iterations, trace = mtbstr });

                        MTBEventOccurs_patch.mtb_dump.Clear();

                        //RimLog.Message("framelistForTicks " + framelistForTicks.Count);
                        //System.IO.File.WriteAllText("C:/CoopReplays/" + SyncTickData.cliendID + "/" + RandContextCounter.currentContextName + "__" + "_" + ___random.seed + "_" + ___iterations + ".txt", str);
                    }
                }
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
                
                //Utilities.RimLog.Message(">>>>>>>>>>>>>>> FOUL RAND CALL");
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
