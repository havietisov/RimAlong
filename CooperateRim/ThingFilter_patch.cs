using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Verse;


namespace CooperateRim
{
    /*
    [HarmonyPatch(typeof(Dialog_BillConfig), methodName: "DoWindowContents")]
    public class Dialog_billConfigPatch
    {
        public static IntVec3 pos;
        public static Bill_Production bill;
        public static bool valid = false;
        public static Pawn restriction;

        [HarmonyPrefix]
        public static bool Prefix(ref IntVec3 ___billGiverPos, Bill_Production ___bill)
        {
            pos = ___billGiverPos;
            bill = ___bill;
            valid = true;
            restriction = ___bill.pawnRestriction;
            return true;
        }

        [HarmonyPostfix]
        public static void PostFix(Bill_Production ___bill, ref IntVec3 ___billGiverPos)
        {
            valid = false;
                    //SyncTickData.AppendSyncTickDataJobPawnRestriction(___billGiverPos, ___bill, ___bill.pawnRestriction);
                
        }
    }*/

    [HarmonyPatch(typeof(Dialog_BillConfig), methodName: "GeneratePawnRestrictionOptions")]
    public class Dialog_billConfigPatch_
    {
        public static IntVec3 pos;
        public static Bill_Production bill;
        public static bool valid = false;
        public static Pawn restriction;
        
        [HarmonyPostfix]
        public static void PostFix(Bill_Production ___bill, IntVec3 ___billGiverPos, ref IEnumerable<Widgets.DropdownMenuElement<Pawn>> __result)
        {
            if (!SyncTickData.AvoidLoop)
            {
                List<Widgets.DropdownMenuElement<Pawn>> widgetElements = new List<Widgets.DropdownMenuElement<Pawn>>();
                int i = 0;
                foreach (var val in __result)
                {
                    var tempVal = val;
                    Pawn p = val.payload;
                    int k = i;
                    tempVal.option.action = () => { SyncTickData.AppendSyncTickDataIndexedBillConfigRestrictionOptionCall(p, k, ___bill, ___billGiverPos); };
                    widgetElements.Add(tempVal);
                    i++;
                }

                __result = widgetElements;
            }
        }
    }

    
    [HarmonyPatch(typeof(ThingFilter), "SetAllow", new System.Type[] { typeof(ThingDef), typeof(bool) })]
    public class ThingFilter_wrapper
    {
        internal static bool avoid_internal_loop = false;

        public static void Thingfilter_setallow_wrap(string thingDefName, bool allow, int thingIDNumber, bool isSpecial)
        {
            avoid_internal_loop = true;
            try
            {
                IStoreSettingsParent storeSettings = null;
                List<Thing>[] things = (List<Thing>[])Find.CurrentMap.thingGrid.GetType().GetField("thingGrid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(Find.CurrentMap.thingGrid);
                
                {
                    Thing issuer = things.Where(u => u.Count != 0).First(u => u.Any(uu => uu.thingIDNumber == thingIDNumber)).First(u => u.thingIDNumber == thingIDNumber);
                    storeSettings = issuer as IStoreSettingsParent;
                }

                if (storeSettings != null)
                {
                    if (!isSpecial)
                    {
                        storeSettings.GetStoreSettings().filter.SetAllow(DefDatabase<ThingDef>.GetNamed(thingDefName, true), allow);
                    }
                    else
                    {
                        storeSettings.GetStoreSettings().filter.SetAllow(DefDatabase<SpecialThingFilterDef>.GetNamed(thingDefName, true), allow);
                    }
                }
            }
            finally
            {
                avoid_internal_loop = false;
            }
        }

        public static void Thingfilter_setallowzone_wrap(string thingDefName, bool allow, int zoneID, bool isSpecial)
        {
            avoid_internal_loop = true;
            try
            {
                IStoreSettingsParent storeSettings = null;
                Zone z = Find.CurrentMap.zoneManager.AllZones.First(u => u.ID == zoneID);

                if (z as IStoreSettingsParent != null)
                {
                    storeSettings = z as IStoreSettingsParent;
                }
                
                if (storeSettings != null)
                {
                    if (!isSpecial)
                    {
                        storeSettings.GetStoreSettings().filter.SetAllow(DefDatabase<ThingDef>.GetNamed(thingDefName, true), allow);
                    }
                    else
                    {
                        CooperateRimming.Log(">>>>>>>>>> SPECIAL : " + thingDefName + " :: " + DefDatabase<SpecialThingFilterDef>.GetNamed(thingDefName, true) + " :: ");
                        storeSettings.GetStoreSettings().filter.SetAllow(DefDatabase<SpecialThingFilterDef>.GetNamed(thingDefName, true), allow);
                    }
                }
            }
            finally
            {
                avoid_internal_loop = false;
            }
        }

        [HarmonyPrefix]
        public static bool SetAllow(ThingDef thingDef, ref bool allow, ThingFilter __instance)
        {
            if (!avoid_internal_loop)
            {
                if (Find.CurrentMap != null)
                {
                    if (Find.Selector.SingleSelectedThing != null)
                    {
                        Thingfilter_setallow_wrap(thingDef.defName, allow, Find.Selector.SingleSelectedThing.thingIDNumber, false);
                    }

                    if (Find.Selector.SelectedZone != null)
                    {
                        Thingfilter_setallowzone_wrap(thingDef.defName, allow, Find.Selector.SelectedZone.ID, false);
                    }
                    //SyncTickData.AppendSyncTickDataDeltaFilter(thingDef, Find.Selector.SingleSelectedThing, Find.Selector.SelectedZone, allow);
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(ThingFilter), "SetAllow", new System.Type[] { typeof(SpecialThingFilterDef), typeof(bool) })]
    public class ThingFilter_wrapper_special
    {
        [HarmonyPrefix]
        public static bool SetAllow(SpecialThingFilterDef sfDef, ref bool allow, ThingFilter __instance)
        {
            if (!ThingFilter_wrapper.avoid_internal_loop)
            {
                if (Find.CurrentMap != null)
                {
                    if (Find.Selector.SingleSelectedThing != null)
                    {
                        ThingFilter_wrapper.Thingfilter_setallow_wrap(sfDef.defName, allow, Find.Selector.SingleSelectedThing.thingIDNumber, true);
                    }

                    if (Find.Selector.SelectedZone != null)
                    {
                        ThingFilter_wrapper.Thingfilter_setallowzone_wrap(sfDef.defName, allow, Find.Selector.SelectedZone.ID, true);
                    }
                    //SyncTickData.AppendSyncTickDataDeltaFilter(thingDef, Find.Selector.SingleSelectedThing, Find.Selector.SelectedZone, allow);
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    /*
    [HarmonyPatch(typeof(ThingFilter), "SetAllow", new System.Type[] { typeof(SpecialThingFilterDef), typeof(bool) })]
    public class ThingFilter_special_patch
    {
        [HarmonyPrefix]
        public static bool SetAllow(ref SpecialThingFilterDef sfDef, ref bool allow)
        {
            if (!SyncTickData.AvoidLoop)
            {
                if (Find.CurrentMap != null && (Find.Selector.SelectedZone != null || Find.Selector.SingleSelectedThing != null))
                {
                    SyncTickData.AppendSyncTickDataDeltaFilterSpecial(sfDef, Find.Selector.SingleSelectedThing, Find.Selector.SelectedZone, allow);
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }*/
}
