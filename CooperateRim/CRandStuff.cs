using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Steam;

//namespace CooperateRim
//{
//    
/*
[HarmonyPatch(typeof(Verse.Game))]
[HarmonyPatch("UpdatePlay")]
public class upd_
{
    [HarmonyPrefix]
    public static bool prefix(Verse.Game __instance, List<Map> ___maps)
    {
        root_play_upd.scopename = nameof(root_play_upd.tick_manager_state);
        CRand.set_state(root_play_upd.tick_manager_state);
        __instance.tickManager.TickManagerUpdate();
        root_play_upd.tick_manager_state = CRand.get_state();
        root_play_upd.scopename = "<none>";

        root_play_upd.scopename = nameof(root_play_upd.map_state);
        CRand.set_state(root_play_upd.map_state);
        for (int i = 0; i < ___maps.Count; i++)
        {
            ___maps[i].MapUpdate();
        }
        root_play_upd.map_state = CRand.get_state();
        root_play_upd.scopename = "<none>";
        return false;
    }
}

    */

    /*
[HarmonyPatch(typeof(Verse.Root))]
[HarmonyPatch("OnGUI")]
public class upd_r
{
    static ulong buffered_state;

    [HarmonyPrefix]
    public static void prefix()
    {
        buffered_state = CRand.get_state();
        root_play_upd.scopename = nameof(root_play_upd.tick_manager_state);
        CRand.set_state(root_play_upd.ui_state);
    }

    [HarmonyPostfix]
    public static void postfix()
    {
        root_play_upd.scopename = nameof(root_play_upd.tick_manager_state);
        root_play_upd.tick_manager_state = CRand.get_state();
        root_play_upd.scopename = "<none>";
        CRand.set_state(buffered_state);
    }
}*/


//[HarmonyPatch(typeof(Verse.Root))]
//[HarmonyPatch("Update")]
//public partial class root_play_upd
//{
//    static bool initialized = false;

//    [HarmonyPrefix]
//    public static bool prefix(ref Verse.Root __instance, ref bool ___destroyed, ref bool ___prefsApplied)
//    {
//        try
//        {
//            CRand.set_state(realtime_state);
//            ResolutionUtility.Update();
//            RealTime.Update();
//            realtime_state = CRand.get_state();
//            bool flag;
//            LongEventHandler.LongEventsUpdate(out flag);
//            if (flag)
//            {
//                ___destroyed = true;
//            }
//            else if (!LongEventHandler.ShouldWaitForEvent)
//            {
//                Rand.EnsureStateStackEmpty();
//                Widgets.EnsureMousePositionStackEmpty();
//                SteamManager.Update();
//                CRand.set_state(portraits_state);
//                PortraitsCache.PortraitsCacheUpdate();
//                portraits_state = CRand.get_state();
//                CRand.set_state(atk_targets_state);
//                AttackTargetsCache.AttackTargetsCacheStaticUpdate();
//                atk_targets_state = CRand.get_state();
//                //Pawn_MeleeVerbs.PawnMeleeVerbsStaticUpdate();
//                //Storyteller.StorytellerStaticUpdate();
//                //CaravanInventoryUtility.CaravanInventoryUtilityStaticUpdate();
//                CRand.set_state(ui_state);
//                __instance.uiRoot.UIRootUpdate();
//                ui_state = CRand.get_state();
//                if (Time.frameCount > 3 && !___prefsApplied)
//                {
//                    ___prefsApplied = true;
//                    Prefs.Apply();
//                }
//                //__instance.soundRoot.Update();
//                try
//                {
//                    //MemoryTracker.Update();
//                }
//                catch (Exception arg)
//                {
//                    RimLog.Message.Error("Error in MemoryTracker: " + arg, false);
//                }
//                try
//                {
//                    //MapLeakTracker.Update();
//                }
//                catch (Exception arg2)
//                {
//                    RimLog.Message.Error("Error in MapLeakTracker: " + arg2, false);
//                }
//            }
//        }
//        catch (Exception arg3)
//        {
//            RimLog.Message.Error("Root level exception in Update(): " + arg3, false);
//        }

//        return false;
//    }
//}
//}
