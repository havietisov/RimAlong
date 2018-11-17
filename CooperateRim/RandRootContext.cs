using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace CooperateRim
{
    public static class RandContextCounter
    {
        public static ulong counter = 9000;
    }

    public class RandRootContext<T>
    {
        public static ulong context;
        static ulong old_ctx;

        public static void Prefix()
        {
            old_ctx = CRand.get_state();
            CRand.set_state(context);
        }

        public static void Postfix()
        {
            context = CRand.get_state();
            CRand.set_state(old_ctx);
        }

        public static void ApplyPatch(string methodname)
        {
            context = RandContextCounter.counter++;
            MethodInfo targetmethod = AccessTools.Method(typeof(T), methodname);
            HarmonyMethod postfix = new HarmonyMethod(typeof(RandRootContext<T>).GetMethod("Postfix"));
            HarmonyMethod prefix = new HarmonyMethod(typeof(RandRootContext<T>).GetMethod("Prefix"));
            CooperateRimming.inst.harmonyInst.Patch(targetmethod, prefix, postfix, null);
        }

        public static void ApplyPatch(string methodname, Type tt)
        {
            MethodInfo targetmethod = AccessTools.Method(tt, methodname);
            HarmonyMethod postfix = new HarmonyMethod(typeof(RandRootContext<T>).GetMethod("Postfix"));
            HarmonyMethod prefix = new HarmonyMethod(typeof(RandRootContext<T>).GetMethod("Prefix"));
            CooperateRimming.inst.harmonyInst.Patch(targetmethod, prefix, postfix, null);
        }
    }
}
