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
        public delegate void delUseIndexedFloatMenuEntryAt(Vector3 clickPos, Pawn pawn, int index, int totalCount, string delegateName);

        public static void UseIndexedFloatMenuEntryAt(Vector3 clickPos, Pawn pawn, int index, int totalCount, string delegateName)
        {
            var entries = FloatMenuMakerMap.ChoicesAtFor(clickPos, pawn);

            if (totalCount == entries.Count)
            {
                Action a = entries[index].action;

                if (a != null && (a.Method.Name + "::" + a.Method.DeclaringType.Name).SequenceEqual(delegateName))
                {
                    a();
                }
                else
                {
                    CooperateRimming.Log("Warning! Different methods for float menu entry! But this might be fine");
                }
            }
            else
            {
                CooperateRimming.Log("Warning! Different method count for float menu entry! But this might be fine");
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
                    int totalCount = __result.Count;
                    string delegateName = __result[idx].action != null ? __result[idx].action.Method.Name + "::" + __result[idx].action.Method.DeclaringType.Name : null;
                    opt.action = () => { UseIndexedFloatMenuEntryAt(clickPos, pawn, _idx, totalCount, delegateName); };
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
