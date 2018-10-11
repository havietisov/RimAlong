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

    [HarmonyPatch(typeof(Designator_ZoneAdd))]
    [HarmonyPatch("DesignateMultiCell")]
    public class Designator_zoneAdd
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Designator __instance, IEnumerable<IntVec3> cells)
        {
            CooperateRimming.Log("DesignateMultiCell_1");
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
            CooperateRimming.Log("Designator_Build designate single cell " + (___stuffDef == null ? "null" : ___stuffDef.ToString()) + " || " + (___stuffDef == null ? "null" : ___stuffDef.defName) );
            ThingDef td = ___stuffDef;
           
            if (!SyncTickData.AvoidLoop)
            {
                //DefDatabase<ThingDef>.AllDefs
                SyncTickData.AppendSyncTickDataDesignatorSingleCell(__instance, c , ___entDef, ___placingRot, ___stuffDef);
                return false;
            }
            else
            {
                return true;
            }
            /*
            CooperateRimming.Log("DesignateMultiCell_1");
            if (!SyncTickData.AvoidLoop)
            {
                SyncTickData.AppendSyncTickData(__instance, cells);
                return false;
            }
            else
            {
                return true;
            }*/
        }
    }

    [HarmonyPatch(typeof(Designator_Mine))]
    [HarmonyPatch("DesignateSingleCell")]
    public class Designator_Mine__
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Designator __instance, ref IntVec3 loc)
        {
            CooperateRimming.Log("Designator_mine designate single cell");
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
            CooperateRimming.Log("Designator_hunt designate single cell");
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
            CooperateRimming.Log("Designator_SmoothSurface designate single cell");
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
            CooperateRimming.Log("Designator_cancel designate single cell");
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
            CooperateRimming.Log("Designator_cancel designate single cell");
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
            //CooperateRimming.Log("FORRRRRRRRRRBIIIIIIIIIIIID " + __instance.parent.ThingID);
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

        /*
        [HarmonyPatch(typeof(Verse.ZoneManager))]
        [HarmonyPatch("RegisterZone")]
        public class ZoneManagerPatch_register
        {

            [HarmonyPrefix]
            public static bool Prefix(Zone newZone)
            {
                CooperateRimming.Log("new zone : " + newZone.label);
                if (SyncTickData.AvoidLoop)
                {
                    return false;
                }
                else
                {
                    SyncTickData.AppendSyncTickData(newZone);
                    return false;
                }
            }
        }*/

        /*
    [HarmonyPatch(typeof(Zone))]
    [HarmonyPatch("AddCell")]
    public class ZoneManagerPatch_deregister
    {
        [HarmonyPrefix]
        public static bool AddCell(IntVec3 c, ref Zone __instance)
        {
            if (!SyncTickData.AvoidLoop)
            {
                CooperateRimming.Log(__instance.label + " : " + c);
                SyncTickData.AppendSyncTickData(__instance.ID, c, true);
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(ZoneManager))]
    [HarmonyPatch("RegisterZone")]
    public class ZoneManagerPatch_RegisterZone
    {
        [HarmonyPrefix]
        public static bool RegisterZone(ref Zone newZone)
        {
            if (!SyncTickData.AvoidLoop)
            {
                CooperateRimming.Log(newZone.GetType().ToString());
                SyncTickData.AppendSyncTickData(newZone.ID, newZone.label, newZone.GetType());
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Designator_ZoneAddStockpile))]
    [HarmonyPatch("MakeNewZone")]
    public class Designator_ZoneAdd_patch
    {
        [HarmonyPrefix]
        public static bool MakeNewZone()
        {
            if (SyncTickData.AvoidLoop)
            {
                return true;
            }
            else
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                SyncTickData.AppendSyncTickDataWithNewZone(sf.GetMethod().DeclaringType.AssemblyQualifiedName);
                return false;
            }
        }
    }*/

        //DesignateMultiCell
        //Designator_ZoneAdd

        //AddHaulDestination
        //Notify_HaulDestinationChangedPriority
        //HaulDestinationManager
    }
