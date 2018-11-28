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
        public static int context_counter;
        public static Type currentContextName;
        public static List<Action<ulong>> replacement_seed = new List<Action<ulong>>();
    }

    public static class ContextStack
    {
        public static Stack<ulong> stack = new Stack<ulong>();
    }

    public class RandRootContext<T>
    {
        public static ulong context;
        static Type oldContext;

        public static void Prefix()
        {
            ContextStack.stack.Push(CRand.get_state());
            CRand.set_state(context);
            oldContext = RandContextCounter.currentContextName;
            RandContextCounter.currentContextName = typeof(T);
            RandContextCounter.context_counter++;
        }

        public static void Postfix()
        {
            context = CRand.get_state();
            CRand.set_state(ContextStack.stack.Pop());
            RandContextCounter.currentContextName = oldContext;
            oldContext = null;
            RandContextCounter.context_counter--;
        }
        
        public static void ApplyPatch(string methodname)
        {
            context = RandContextCounter.counter++;
            RandContextCounter.replacement_seed.Add(u => { context = u; });
            MethodInfo targetmethod = AccessTools.Method(typeof(T), methodname);
            HarmonyMethod postfix = new HarmonyMethod(typeof(RandRootContext<T>).GetMethod("Postfix"));
            HarmonyMethod prefix = new HarmonyMethod(typeof(RandRootContext<T>).GetMethod("Prefix"));
            //Utilities.RimLog.Message(postfix + "::" + prefix);
            CooperateRimming.inst.harmonyInst.Patch(targetmethod, prefix, postfix, null);
        }

        public static void ApplyPatch(string methodname, Type tt)
        {
            MethodInfo targetmethod = AccessTools.Method(tt, methodname);
            HarmonyMethod postfix = new HarmonyMethod(typeof(RandRootContext<T>).GetMethod("Postfix"));
            HarmonyMethod prefix = new HarmonyMethod(typeof(RandRootContext<T>).GetMethod("Prefix"));
            //Utilities.RimLog.Message(postfix + "::" + prefix);
            CooperateRimming.inst.harmonyInst.Patch(targetmethod, prefix, postfix, null);
        }

        public static void ApplyPatch(MethodInfo targetmethod, Type tt)
        {
            HarmonyMethod postfix = new HarmonyMethod(typeof(RandRootContext<T>).GetMethod("Postfix"));
            HarmonyMethod prefix = new HarmonyMethod(typeof(RandRootContext<T>).GetMethod("Prefix"));
            //Utilities.RimLog.Message(postfix + "::" + prefix);
            CooperateRimming.inst.harmonyInst.Patch(targetmethod, prefix, postfix, null);
        }
    }
}
