using Harmony;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "ChoicesAtFor")]
    class floatMenuMakerpatch
    {
        public static void UseIndexedFloatMenuEntryAt(Vector3 clickPos, Pawn pawn, int index)
        {
            Action a = FloatMenuMakerMap.ChoicesAtFor(clickPos, pawn)[index].action;

            if (a != null)
            {
                a();
            }
        }

        [HarmonyPostfix]
        public static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> __result)
        {
            if (!SyncTickData.AvoidLoop)
            {
                int idx = 0;
                foreach (FloatMenuOption opt in __result)
                {
                    int _idx = idx;
                    opt.action = () => { UseIndexedFloatMenuEntryAt(clickPos, pawn, _idx); };
                    idx++;
                }
            }
        }
    }

    /*
    [HarmonyPatch(typeof(FloatMenuOption), "Chosen")]
    class FloatMenuOptionPatch
    {
        public static void InvokeAction(Action a)
        {
            if (a == null)
            {
                CooperateRimming.Log(">>>>>>>>>>>> Action is null, wtf!!!!!!!!!!!!!!!!!!");
            }
            else
            {
                a();
            }
        }

        public class CallbackHolder
        {
            public Action action;
        }

        [HarmonyPrefix]
        public static void Prefix(FloatMenuOption __instance, ref CallbackHolder __state)
        {
            __state = new CallbackHolder();
            __state.action = __instance.action;
            Action a = __instance.action;
            __instance.action = () => 
            {
                CooperateRimming.Log("deltype : " + a.GetType());
                InvokeAction(a);
            };
        }

        [HarmonyPostfix]
        public static void Postfix(FloatMenuOption __instance, ref CallbackHolder __state)
        {
            __instance.action = __state.action;
        }
    }*/
}
