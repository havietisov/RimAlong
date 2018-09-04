using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Verse.RandomNumberGenerator_BasicHash))]
    [HarmonyPatch("GetInt")]
    public class RandomNumberGenerator_BasicHash_patch
    {
        const int RAND_MAX = 32767;

        public static ulong next = 1;

        static int rand()
        {
            next = next * 1103515245 + 12345;
            return (int)(next / 65536) % (RAND_MAX + 1);
        }
        
        [HarmonyPostfix]
        public static void GetInt(uint iterations, ref int __result)
        {
            Random r = new Random((int)iterations);
            __result = r.Next();
            
        }
    }
}
