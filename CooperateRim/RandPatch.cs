using Harmony;
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
    [HarmonyPatch(typeof(Verse.Sound.SoundRoot), "Update")]
    public class soundRootPatch
    {
        [HarmonyPrefix]
        public static bool Update()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(SoundStarter), "PlayOneShotOnCamera")]
    public class SoundStarterPatchPlayOneShotOnCamera
    {
        [HarmonyPrefix]
        public static bool PlayOneShotOnCamera()
        {
            return false;
        }
    }

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
        }

        private static uint Rotate(uint value, int count)
        {
            return (value << count) | (value >> 32 - count);
        }
    }
}
