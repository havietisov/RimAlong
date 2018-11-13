using Harmony;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(FloatMenuOption), MethodType.Constructor, new Type[] { typeof(string), typeof(Action), typeof(MenuOptionPriority), typeof(Action), typeof(Thing), typeof(float), typeof(Func<Rect, bool>),  typeof(WorldObject) })]
    class FloatMenuOptionPatch
    {
        public static void InvokeAction(Action a)
        {
            a();
        }
        
        [HarmonyPostfix]
        public static void Postfix(FloatMenuOption __instance, string label, Action action, MenuOptionPriority priority, Action mouseoverGuiAction, Thing revalidateClickTarget, float extraPartWidth, Func<Rect, bool> extraPartOnGUI, WorldObject revalidateWorldClickTarget)
        {
            Action a = __instance.action;
            __instance.action = () => { CooperateRimming.Log("huehuehue"); InvokeAction(a); };
        }
    }
}
