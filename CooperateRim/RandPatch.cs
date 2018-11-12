using CooperateRim.Utilities;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace CooperateRim
{

    [HarmonyPatch(typeof(IntermittentSteamSprayer), "SteamSprayerTick")]
    public class IntermittentSteamSprayerPatch
    {
        [HarmonyPrefix]
        public static bool tick()
        {
            return false;
        }
    }

    /*
     * some SPOOKY shit about geysers
     > CooperateRim.getValuePatch::get_Value
> Verse.Rand::get_Value_Patch1
> Verse.Rand::Range
> Verse.Mote::.ctor
> Verse.MoteThrown::.ctor
> System.Reflection.MonoCMethod::InternalInvoke
> System.Reflection.MonoCMethod::Invoke
> System.Reflection.MonoCMethod::Invoke
> System.Reflection.ConstructorInfo::Invoke
> System.Activator::CreateInstance
> System.Activator::CreateInstance
> Verse.ThingMaker::MakeThing
> RimWorld.MoteMaker::NewBaseAirPuff
> RimWorld.MoteMaker::ThrowAirPuffUp
> RimWorld.IntermittentSteamSprayer::SteamSprayerTick
> RimWorld.Building_SteamGeyser::Tick
> Verse.TickList::Tick_Patch1
> Verse.TickManager::DoSingleTick
> CooperateRim.TickManagerPatch::Prefix
> Verse.TickManager::TickManagerUpdate_Patch2
> Verse.Game::UpdatePlay
> Verse.Root_Play::Update
         */
         /*
    [HarmonyPatch(typeof(SoundStarter), "PlayOneShotOnCamera")]
    public class SoundStarterPatchPlayOneShotOnCamera
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            getValuePatch.GuardedPush();
            return false;
        }
        
        [HarmonyPostfix]
        public static void postfix()
        {
            getValuePatch.GuardedPop();
        }
    }*/

    /*
    [HarmonyPatch(typeof(SoundStarter), "PlayOneShot")]
    public class SoundStarterPatchPlayOneShot
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            getValuePatch.GuardedPush();
            return false;
        }

        [HarmonyPostfix]
        public static void postfix()
        {
            getValuePatch.GuardedPop();
        }
    }*/

    /*
    [HarmonyPatch(typeof(SustainerManager))]
    [HarmonyPatch("SustainerManagerUpdate")]
    public class SustainerManagerUpdate_patch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            getValuePatch.GuardedPush();
            return false;
        }

        [HarmonyPostfix]
        public static void postfix()
        {
            getValuePatch.GuardedPop();
        }
    }*/

    [HarmonyPatch(typeof(Selector), "Select")]
    public class Selector__patch
    {
        [HarmonyPrefix]
        public static void Prefix(ref bool playSound)
        {
            //playSound = false;
            //getValuePatch.GuardedPush();
        }

        [HarmonyPostfix]
        public static void postfix()
        {
            //getValuePatch.GuardedPop();
        }
    }

    [HarmonyPatch(typeof(Def), MethodType.Constructor, new Type[] { })]
    public class def_patch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            getValuePatch.GuardedPush();
        }

        [HarmonyPostfix]
        public static void postfix()
        {
            getValuePatch.GuardedPop();
        }
    }

    [HarmonyPatch(typeof(Mote), MethodType.Constructor, new Type[] {})]
    public class Mote_patch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            getValuePatch.GuardedPush();
        }

        [HarmonyPostfix]
        public static void postfix()
        {
            getValuePatch.GuardedPop();
        }
    }

    [HarmonyPatch(typeof(Sample), MethodType.Constructor, new Type[] { typeof(SubSoundDef) })]
    public class Sample_patch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            getValuePatch.GuardedPush();
        }

        [HarmonyPostfix]
        public static void postfix()
        {
            getValuePatch.GuardedPop();
        }
    }

    [HarmonyPatch(typeof(SubSoundDef), "RandomizedVolume")]
    public class SubSoundVolume
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            getValuePatch.GuardedPush();
        }

        [HarmonyPostfix]
        public static void postfix()
        {
            getValuePatch.GuardedPop();
        }
    }

    [HarmonyPatch(typeof(SubSoundDef), "RandomizedResolvedGrain")]
    public class SubSoundDefOf_patch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            getValuePatch.GuardedPush();
        }

        [HarmonyPostfix]
        public static void postfix()
        {
            getValuePatch.GuardedPop();
        }
    }

    [HarmonyPatch(typeof(Verse.Sound.SoundRoot), "Update")]
    public class soundRootPatch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            getValuePatch.GuardedPush();
        }

        [HarmonyPostfix]
        public static void postfix()
        {
            getValuePatch.GuardedPop();
        }
    }

    [HarmonyPatch(typeof(RimWorld.MusicManagerPlay), "MusicUpdate")]
    public class music__MusicUpdate
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            getValuePatch.GuardedPush();
        }

        [HarmonyPostfix]
        public static void postfix()
        {
            getValuePatch.GuardedPop();
        }
    }
    
    [HarmonyPatch(typeof(Pawn_MeleeVerbs))]
    [HarmonyPatch("ChooseMeleeVerb")]
    public class Pawn_MeleeVerbs_patch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            getValuePatch.GuardedPush();
        }

        [HarmonyPostfix]
        public static void postfix()
        {
            getValuePatch.GuardedPop();
        }
    }

    [HarmonyPatch(typeof(Designator_ZoneAdd), "SelectedUpdate")]
    public class zone_add_p
    {
        [HarmonyPrefix]
        public static bool prefix()
        {
            RimLog.Message(">>>>>>>>>>>>>>>>>>>>>>> selected update bullshit");
            return false;
        }
    }

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
                
                //RimLog.Message(">>>>>>>>>>>>>>> FOUL RAND CALL");
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

#if RAND_LOGS
    [HarmonyPatch(typeof(Rand), "MTBEventOccurs")]
    public class MTBEventOccurs
    {
        [HarmonyPostfix]
        public static void MTBEventOccurs__(ref bool __result, ref uint ___iterations)
        {
            if (CooperateRimming.dumpRand)
            {
                int tick = Current.Game == null ? -1 : Find.TickManager.TicksGame;
                StackTrace tr = new StackTrace();
                streamholder.WriteLine("====STACK====", "MTBE");

                foreach (var frame in tr.GetFrames())
                {
                    streamholder.WriteLine(frame.GetMethod().ReflectedType + "::" + frame.GetMethod().Name, "MTBE");
                }

                streamholder.WriteLine("====END====", "MTBE");
                streamholder.WriteLine(__result + " at  iter  " + ___iterations + " at tick " + tick, "MTBE");
            }
        }
    }

    [HarmonyPatch(typeof(Rand), "get_Value", new Type[] { })]
    public class getValuePatch
    {
        [HarmonyPostfix]
        public static void get_Value(ref uint ___iterations, ref float __result)
        {
            if (CooperateRimming.dumpRand)
            {
                int tick = Current.Game == null ? -1 : Find.TickManager.TicksGame;
                StackTrace tr = new StackTrace();
                streamholder.WriteLine("====STACK====", "state");

                foreach (var frame in tr.GetFrames())
                {
                    streamholder.WriteLine(frame.GetMethod().ReflectedType + "::" + frame.GetMethod().Name, "state");
                }

                streamholder.WriteLine("====END====", "state");
                streamholder.WriteLine(__result + " at  iter  " + ___iterations + " at tick " + tick, "state");
            }
        }
    }

    [HarmonyPatch(typeof(Rand), "PushState", new Type[] { })]
    public class pushState
    {
        [HarmonyPostfix]
        public static void get_Value(ref uint ___iterations, ref RandomNumberGenerator ___random)
        {
            if (CooperateRimming.dumpRand)
            {
                int tick = Current.Game == null ? -1 : Find.TickManager.TicksGame;
                StackTrace tr = new StackTrace();
                streamholder.WriteLine("====STACK====", "state");

                foreach (var frame in tr.GetFrames())
                {
                    streamholder.WriteLine(frame.GetMethod().ReflectedType + "::" + frame.GetMethod().Name, "state");
                }

                streamholder.WriteLine("====END====", "state");
                streamholder.WriteLine(___random.seed + " pushed at tick " + tick, "state");
            }
        }
    }

    [HarmonyPatch(typeof(Rand), "PushState", new Type[] { typeof(int) })]
    public class pushState_
    {
        [HarmonyPostfix]
        public static void get_Value(ref uint ___iterations, ref RandomNumberGenerator ___random, int replacementSeed)
        {
            if (CooperateRimming.dumpRand)
            {
                int tick = Current.Game == null ? -1 : Find.TickManager.TicksGame;
                StackTrace tr = new StackTrace();
                streamholder.WriteLine("====STACK====", "state");

                foreach (var frame in tr.GetFrames())
                {
                    streamholder.WriteLine(frame.GetMethod().ReflectedType + "::" + frame.GetMethod().Name, "state");
                }

                streamholder.WriteLine("====END====", "state");
                streamholder.WriteLine(___random.seed + " replaced at tick " + tick, "state");
            }
        }
    }

    [HarmonyPatch(typeof(Rand), "get_Int", new Type[] { })]
    public class getIntPatch
    {
        [HarmonyPostfix]
        public static void get_Int(ref uint ___iterations, ref float __result)
        {

            if (CooperateRimming.dumpRand)
            {
                int tick = Current.Game == null ? -1 : Find.TickManager.TicksGame;
                StackTrace tr = new StackTrace();
                streamholder.WriteLine("====STACK====", "state");

                foreach (var frame in tr.GetFrames())
                {
                    streamholder.WriteLine(frame.GetMethod().ReflectedType + "::" + frame.GetMethod().Name, "state");
                }

                streamholder.WriteLine("====END====", "state");
                streamholder.WriteLine(__result + " at  iter  " + ___iterations + " at tick " + tick, "state");
            }
        }
    }

#endif

    [HarmonyPatch(typeof(RandomNumberGenerator_BasicHash), "GetHash")]
    public class GeneralRandPatch
    {
        public static int TickID;
        public static int frameIterID;

        static int MAGIC_CONSTANT_1 = 399699;
        static int MAGIC_CONSTANT_2 = 674274;
        static int MAGIC_CONSTANT_3 = -109866;
        static int MAGIC_CONSTANT_4 = 969870;
        static int MAGIC_CONSTANT_5 = 540491;
        static int MAGIC_CONSTANT_6 = -479830;
        static int MAGIC_CONSTANT_7 = 188869;

        static int hash(int x, int y)
        {

            int result = x;

            result *= MAGIC_CONSTANT_1 | 1;

            result += MAGIC_CONSTANT_2;

            result = (result >> 32) + (result << 32);

            result ^= MAGIC_CONSTANT_3;

            result += y;

            result *= MAGIC_CONSTANT_4 | 1;

            result += MAGIC_CONSTANT_5;

            result = (result >> 32) + (result << 32);

            result ^= MAGIC_CONSTANT_6;

            result *= MAGIC_CONSTANT_7 | 1;

            return result;
        }

        public static void Reset()
        {
            TickID = 0;
            frameIterID = 0;
        }

        [HarmonyPostfix]
        public static void Prefix(ref uint __result, RandomNumberGenerator_BasicHash __instance, int buffer)
        {
#if RAND_LOGS
            if (CooperateRimming.dumpRand)
            {
                int tick = Current.Game == null ? -1 : Find.TickManager.TicksGame;
                StackTrace tr = new StackTrace();

                streamholder.WriteLine("====STACK====", "rand");

                foreach (var frame in tr.GetFrames())
                {
                    streamholder.WriteLine(frame.GetMethod().ReflectedType + "::" + frame.GetMethod().Name, "rand");
                }

                streamholder.WriteLine("====END====", "rand");
                streamholder.WriteLine(__result.ToString() + " seed " + __instance.seed + " and buffer " + buffer + " at tick " + tick, "rand");

            }
#endif
        }

        private static uint Rotate(uint value, int count)
        {
            return (value << count) | (value >> 32 - count);
        }
    }
}
