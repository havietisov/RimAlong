using Harmony;
using System;
using Verse.Sound;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Sample), MethodType.Constructor, new Type[] { typeof(SubSoundDef) })]
    public class Sample_patch
    {
        static ulong state_old;
        static ulong state_mine;


        [HarmonyPrefix]
        public static void Prefix()
        {
            state_old = CRand.get_state();
            CRand.set_state(state_mine);
        }

        [HarmonyPostfix]
        public static void postfix()
        {
            state_mine = CRand.get_state();
            CRand.set_state(state_old);
        }
    }
}
