using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;
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

    [HarmonyPatch(typeof(SoundStarter), "PlayOneShotOnCamera")]
    public class SoundStarterPatchPlayOneShotOnCamera
    {
        [HarmonyPrefix]
        public static bool PlayOneShotOnCamera()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(SoundStarter), "PlayOneShot")]
    public class SoundStarterPatchPlayOneShot
    {
        [HarmonyPrefix]
        public static bool PlayOneShot()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(Sample))]
    [HarmonyPatch(new[] { typeof(SubSoundDef) })]
    public class Sample_patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SubSoundDef def, Sample __instance,  ref Dictionary<SoundParamTarget, float> ___volumeInMappings)
        {
            __instance.subDef = def;
            __instance.resolvedVolume = def.volumeRange.max;
            __instance.resolvedPitch = def.pitchRange.max;// UnityEngine.Random.Range(def.pitchRange.min, def.pitchRange.max);
            __instance.startRealTime = UnityEngine.Time.realtimeSinceStartup;
            if (Current.ProgramState == ProgramState.Playing)
            {
                __instance.startTick = Find.TickManager.TicksGame;
            }
            else
            {
                __instance.startTick = 0;
            }

            if (___volumeInMappings == null)
            {
                ___volumeInMappings = new Dictionary<SoundParamTarget, float>();
            }

            foreach (SoundParamTarget_Volume item in (from m in __instance.subDef.paramMappings select m.outParam).OfType<SoundParamTarget_Volume>())
            {
                ___volumeInMappings.Add(item, 0f);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(SubSoundDef), "RandomizedVolume")]
    public class SubSoundVolume
    {
        [HarmonyPrefix]
        public static bool Prefix(ref float __result, SubSoundDef __instance )
        {
            __result = UnityEngine.Random.Range(__instance.volumeRange.min, __instance.volumeRange.max) / 100f;
            return false;
        }
    }

    [HarmonyPatch(typeof(SubSoundDef), "RandomizedResolvedGrain")]
    public class SubSoundDefOf_patch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            CooperateRimming.dumpRand = false;
            Rand.PushState(System.DateTime.Now.GetHashCode());
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            Rand.PopState();
            if (Current.Game != null)
            {
                CooperateRimming.dumpRand = false;
            }
        }
    }

    [HarmonyPatch(typeof(Verse.Sound.SoundRoot), "Update")]
    public class soundRootPatch
    {
        [HarmonyPrefix]
        public static bool Update()
        {

            return false;
        }
    }

    [HarmonyPatch(typeof(RimWorld.MusicManagerPlay), "MusicUpdate")]
    public class music__MusicUpdate
    {
        [HarmonyPrefix]
        /*
            Verse.GenCollection::RandomElementByWeight
            RimWorld.MusicManagerPlay::ChooseNextSong
            RimWorld.MusicManagerPlay::StartNewSong
            RimWorld.MusicManagerPlay::MusicUpdate
        */
        public static bool MusicUpdate()
        {
            return false;
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
