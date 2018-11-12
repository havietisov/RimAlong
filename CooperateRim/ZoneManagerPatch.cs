using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using System.Diagnostics;

namespace CooperateRim
{
    /*
    [HarmonyPatch(typeof(Designator_ZoneAdd))]
    [HarmonyPatch("DesignateMultiCell")]
    public class Designator_zoneAdd
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Designator __instance, IEnumerable<IntVec3> cells)
        {
            CooperateRimming.RimLog.Message("DesignateMultiCell_1");
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickData(__instance, cells);
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Designator_Hunt))]
    [HarmonyPatch("DesignateThing")]
    public class Designator_Hunt_
    {
        [HarmonyPrefix]
        public static bool DesignateThing(Designator_Hunt __instance, Thing t)
        {
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickDesignatorApplyToThing(__instance, t, t.Position);
                return false;
            }
            else
            {
                return true;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(Designator_Strip))]
    [HarmonyPatch("DesignateThing")]
    public class Designator_strip_
    {
        [HarmonyPrefix]
        public static bool DesignateThing(Designator_Strip __instance, Thing t)
        {
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickDesignatorApplyToThing(__instance, t, t.Position);
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Designator_Claim))]
    [HarmonyPatch("DesignateSingleCell")]
    public class Designator_Claim_Tile
    {
        [HarmonyPrefix]
        public static bool DesignateSingleCell(Designator_Tame __instance, IntVec3 c)
        {
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickData(__instance, c);
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Designator_Claim))]
    [HarmonyPatch("DesignateThing")]
    public class Designator_Claim_Thing
    {
        [HarmonyPrefix]
        public static bool DesignateThing(Designator_Tame __instance, Thing t)
        {
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickDesignatorApplyToThing(__instance, t, t.Position);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    
    [HarmonyPatch(typeof(Designator_Tame))]
    [HarmonyPatch("DesignateThing")]
    public class Designator_Tame_
    {
        [HarmonyPrefix]
        public static bool DesignateThing(Designator_Tame __instance, Thing t)
        {
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickDesignatorApplyToThing(__instance, t, t.Position);
                return false;
            }
            else
            {
                return true;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(Designator_Build))]
    [HarmonyPatch("DesignateSingleCell")]
    public class Designator_build
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Designator __instance, ref IntVec3 c, ref BuildableDef ___entDef, ref Rot4 ___placingRot, ref ThingDef ___stuffDef)
        {
            CooperateRimming.RimLog.Message("Designator_Build designate single cell " + (___stuffDef == null ? "null" : ___stuffDef.ToString()) + " || " + (___stuffDef == null ? "null" : ___stuffDef.defName) );
            ThingDef td = ___stuffDef;
           
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickDataDesignatorSingleCell(__instance, c , ___entDef, ___placingRot, ___stuffDef);
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Designator_Mine))]
    [HarmonyPatch("DesignateSingleCell")]
    public class Designator_Mine__
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Designator __instance, ref IntVec3 loc)
        {
            CooperateRimming.RimLog.Message("Designator_mine designate single cell");
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickData(__instance, loc);
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Designator_Hunt))]
    [HarmonyPatch("DesignateThing")]
    public class Designator_Hunt__
    {
        [HarmonyPrefix]
        public static bool Prefix(Designator __instance, Thing t)
        {
            CooperateRimming.RimLog.Message("Designator_hunt designate single cell");
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickDesignatorApplyToThing(__instance, t, t.Position);
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Designator_SmoothSurface))]
    [HarmonyPatch("DesignateSingleCell")]
    public class Designator_SmoothSurface__
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Designator __instance, ref IntVec3 c)
        {
            CooperateRimming.RimLog.Message("Designator_SmoothSurface designate single cell");
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickData(__instance, c);
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Designator_Cancel))]
    [HarmonyPatch("DesignateSingleCell")]
    public class Designator_cancel
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Designator __instance, ref IntVec3 c)
        {
            CooperateRimming.RimLog.Message("Designator_cancel designate single cell");
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickData(__instance, c);
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Designator_Cancel))]
    [HarmonyPatch("DesignateThing")]
    public class Designator_cancel_
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Designator __instance, ref Thing t)
        {
            CooperateRimming.RimLog.Message("Designator_cancel designate single cell");
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickDesignatorApplyToThing(__instance, t, t.Position);
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(CompForbiddable))]
    [HarmonyPatch("set_Forbidden")]
    public class __CompForbiddable
    {
        [HarmonyPrefix]
        public static bool SetForbidden(CompForbiddable __instance, bool value)
        {
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.CompForbiddableSetForbiddenCall(__instance.parent.ThingID, value);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    */
}
