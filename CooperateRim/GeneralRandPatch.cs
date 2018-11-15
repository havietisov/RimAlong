using Harmony;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace CooperateRim
{

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
