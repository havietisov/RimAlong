using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace CooperateRim
{
    public static class GenericRand
    {
        public static System.Random r = new Random(0);
    }
    
    [HarmonyPatch(typeof(Verse.Rand))]
    [HarmonyPatch("get_Int")]
    public class RandomNumberGenerator_BasicHash_patch
    {
        public static int lastIter;
        public static int FrameIter;
        internal static bool shouldLog;

        /*
const int RAND_MAX = 32767;

public static ulong next = 1;

static int rand()
{
next = next * 1103515245 + 12345;
return (int)(next / 65536) % (RAND_MAX + 1);
}*/

        public static bool iterationsReset = false;

        [HarmonyPrefix]
        public static bool GetInt(ref uint ___iterations, ref int __result)
        {
            {
                if (RandomNumberGenerator_BasicHash_patch.lastIter == TickManagerPatch.myTicksValue)
                {
                    FrameIter++;
                }
                else
                {
                    RandomNumberGenerator_BasicHash_patch.FrameIter = 0;
                }
                lastIter = TickManagerPatch.myTicksValue;
                Random r = new Random(TickManagerPatch.myTicksValue + RandomNumberGenerator_BasicHash_patch.FrameIter);
                __result = r.Next();
                if (shouldLog)
                {
                    CooperateRimming.Log("GetInt + " + lastIter + " :: " + __result);
                }
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(Verse.Rand))]
    [HarmonyPatch("get_Value")]
    public class RandomNumberGenerator_BasicHash_patch_float
    {
        /*
const int RAND_MAX = 32767;

public static ulong next = 1;

static int rand()
{
next = next * 1103515245 + 12345;
return (int)(next / 65536) % (RAND_MAX + 1);
}*/

        public static bool iterationsReset = false;

        [HarmonyPrefix]
        public static bool GetInt(ref uint ___iterations, ref float __result)
        {
            {
                if (RandomNumberGenerator_BasicHash_patch.lastIter == TickManagerPatch.myTicksValue)
                {
                    RandomNumberGenerator_BasicHash_patch.FrameIter++;
                }
                else
                {
                    RandomNumberGenerator_BasicHash_patch.FrameIter = 0;
                }
                RandomNumberGenerator_BasicHash_patch.lastIter = TickManagerPatch.myTicksValue;
                Random r = new Random(TickManagerPatch.myTicksValue + RandomNumberGenerator_BasicHash_patch.FrameIter);
                __result = (float)r.NextDouble();
                if (RandomNumberGenerator_BasicHash_patch.shouldLog)
                {
                    CooperateRimming.Log("GetValue + " + RandomNumberGenerator_BasicHash_patch.lastIter + " :: " + __result);
                }
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(Rand))]
    [HarmonyPatch("Range")]
    [HarmonyPatch(new []{ typeof(int), typeof(int)})]
    public class Rand_patch
    {
        [HarmonyPrefix]
        public static bool Range(ref int __result, ref int min, ref int max)
        {
            if (min >= max)
            {
                __result = min;
                return false;
            }
            __result = GenericRand.r.Next(min, max);
            return false;
        }
    }

    [HarmonyPatch(typeof(Rand))]
    [HarmonyPatch("Range")]
    [HarmonyPatch(new[] { typeof(float), typeof(float) })]
    public class Rand_patch_flpat
    {
        [HarmonyPrefix]
        public static bool Range(ref float __result, ref float min, ref float max)
        {
            if (min >= max)
            {
                __result = min;
                return false;
            }
            __result = (float)(GenericRand.r.NextDouble() * (max - min) + min);
            return false;
        }
    }
}
