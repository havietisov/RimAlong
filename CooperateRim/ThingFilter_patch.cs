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
    public class ThingFilter_patch
    {
        [HarmonyPrefix]
        public static bool SetAllow(ref ThingDef thingDef, ref bool allow, ThingFilter __instance)
        {
            if (!SyncTickData.AvoidLoop)
            {
                if (Find.CurrentMap != null && (Find.Selector.SelectedZone != null || Find.Selector.SingleSelectedThing != null))
                {
                    SyncTickData.AppendSyncTickDataDeltaFilter(thingDef, Find.Selector.SingleSelectedThing, Find.Selector.SelectedZone, allow);
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
    }
}
