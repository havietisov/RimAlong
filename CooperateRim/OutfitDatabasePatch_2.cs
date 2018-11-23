﻿using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace CooperateRim
{
    [HarmonyPatch(typeof(OutfitDatabase), "MakeNewOutfit")]
    public class OutfitDatabasePatch_2
    {
        public static void FixMakeNewOutfit(OutfitDatabase __instance, ref Outfit __result, ref List<Outfit> ___outfits)
        {
            int num;
            if (___outfits.Any<Outfit>())
            {
                num = ___outfits.Max((Outfit o) => o.uniqueId) + 1;
            }
            else
            {
                num = 1;
            }
            int uniqueId = num;
            Outfit outfit = new Outfit(uniqueId, "Outfit".Translate() + " " + uniqueId.ToString());
            ThingFilterPatch.thingFilterCallerStack.Push(outfit);
            outfit.filter.SetAllow(ThingCategoryDefOf.Apparel, true, null, null);
            ThingFilterPatch.thingFilterCallerStack.Pop();
            ___outfits.Add(outfit);
            __result = outfit;
        }

        [HarmonyPrefix]
        public static bool Prefix(ref Outfit __result, OutfitDatabase __instance, ref List<Outfit> ___outfits)
        {
            FixMakeNewOutfit(__instance, ref __result, ref ___outfits);
            return false;
        }
    }

    /*
    [HarmonyPatch(typeof(ThingFilter), "SetAllowAll", new System.Type[] { typeof(ThingFilter) })]
    public class ThingFilter_setallowall_wrapper
    {
        internal static bool avoid_internal_loop = false;

        public static void ThingFilter_setallowall_zone(int thingIDNumber, bool actuallyDisallow)
        {
            avoid_internal_loop = true;
            try
            {
                IStoreSettingsParent storeSettings = null;
                Zone z = Find.CurrentMap.zoneManager.AllZones.First(u => u.ID == thingIDNumber);

                if (z as IStoreSettingsParent != null)
                {
                    storeSettings = z as IStoreSettingsParent;
                }

                if (storeSettings != null)
                {
                    if (!actuallyDisallow)
                    {
                        storeSettings.GetStoreSettings().filter.SetAllowAll(null);
                    }
                    else
                    {
                        storeSettings.GetStoreSettings().filter.SetDisallowAll();
                    }
                }
            }
            finally
            {
                avoid_internal_loop = false;
            }
        }

        public static void ThingFilter_setallowall_thing(int thingIDNumber, bool actuallyDisallow)
        {

        }

        public static void ThingFilter_setallowall_bill(int thingIDNumber, int billNumber, bool actuallyDisallow)
        {

        }


        [HarmonyPrefix]
        public static bool Prefix(ThingFilter __instance)
        {
            if (!avoid_internal_loop)
            {
                if (Find.CurrentMap != null)
                {
                    if (ThingFilterPatch.thingFilterCallerStack.Count > 0)
                    {
                        object o = ThingFilterPatch.thingFilterCallerStack.Peek();
                        Utilities.RimLog.Message("SetAllow :::::::::: " + o);

                        if (o is Zone)
                        {
                            ThingFilter_setallowall_zone((o as Zone).ID, false);
                        }

                        if (o is Thing)
                        {
                            ThingFilter_setallowall_thing((o as Thing).thingIDNumber, false);
                        }

                        if (o is Bill)
                        {
                            Bill b = o as Bill;
                            BillStack bs = b.billStack;
                            Thing t = bs.billGiver as Thing;
                            int index = bs.Bills.IndexOf(b);
                            ThingFilter_setallowall_bill(t.thingIDNumber, index, false);
                            Utilities.RimLog.Message("this actions is yet invalid for bills! giver is " + t);
                        }

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

    [HarmonyPatch(typeof(ThingFilter), "SetDisallowAll")]
    public class ThingFilter_setdisallowall_wrapper
    {
        [HarmonyPrefix]
        public static bool Prefix(ThingFilter __instance)
        {
            if (!ThingFilter_setallowall_wrapper.avoid_internal_loop)
            {
                if (Find.CurrentMap != null)
                {
                    if (ThingFilterPatch.thingFilterCallerStack.Count > 0)
                    {
                        object o = ThingFilterPatch.thingFilterCallerStack.Peek();
                        Utilities.RimLog.Message("SetAllow :::::::::: " + o);

                        if (o is Zone)
                        {
                            ThingFilter_setallowall_wrapper.ThingFilter_setallowall_zone((o as Zone).ID, true);
                        }

                        if (o is Thing)
                        {
                            ThingFilter_setallowall_wrapper.ThingFilter_setallowall_thing((o as Thing).thingIDNumber, true);
                        }

                        if (o is Bill)
                        {
                            Bill b = o as Bill;
                            BillStack bs = b.billStack;
                            Thing t = bs.billGiver as Thing;
                            int index = bs.Bills.IndexOf(b);
                            ThingFilter_setallowall_wrapper.ThingFilter_setallowall_bill(t.thingIDNumber, index, true);
                            Utilities.RimLog.Message("this actions is yet invalid for bills! giver is " + t);
                        }
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


    [HarmonyPatch(typeof(ThingFilter), "SetAllow", new System.Type[] { typeof(ThingDef), typeof(bool) })]
    public class ThingFilter_wrapper
    {
        internal static bool avoid_internal_loop = false;

        public static void ThingFilter_setallow_bill_with_billgiver(string thingDefName, bool allow, int thingIDNumber, bool isSpecial, int billIndex)
        {
            avoid_internal_loop = true;
            try
            {
                IBillGiver billgiver = null;
                List<Thing>[] things = (List<Thing>[])Find.CurrentMap.thingGrid.GetType().GetField("thingGrid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(Find.CurrentMap.thingGrid);
                Utilities.RimLog.Message("ThingFilter_setallow_bill_with_billgiver");
                {
                    Thing issuer = things.Where(u => u.Count != 0).First(u => u.Any(uu => uu.thingIDNumber == thingIDNumber)).First(u => u.thingIDNumber == thingIDNumber);
                    Utilities.RimLog.Message(">>>>>>>>>>> issuer :  " + issuer + " :: " + (issuer as IBillGiver));
                    billgiver = issuer as IBillGiver;
                }

                if (billgiver != null)
                {
                    if (!isSpecial)
                    {
                        Utilities.RimLog.Message(">>>>>>>>>>> billgiver.BillStack.Bills[billIndex].ingredientFilter.SetAllow " + billIndex + " |" + thingDefName + "| " + billgiver.BillStack.Bills[billIndex]);
                        billgiver.BillStack.Bills[billIndex].ingredientFilter.SetAllow(DefDatabase<ThingDef>.GetNamed(thingDefName, true), allow);
                    }
                    else
                    {
                        Utilities.RimLog.Message(">>>>>>>>>>> billgiver.BillStack.Bills[billIndex].ingredientFilter.SetAllow " + billIndex + " |" + thingDefName + "| " + billgiver.BillStack.Bills[billIndex]);
                        billgiver.BillStack.Bills[billIndex].ingredientFilter.SetAllow(DefDatabase<SpecialThingFilterDef>.GetNamed(thingDefName, true), allow);
                    }
                }
                else
                {
                    Utilities.RimLog.Message("missing bill giver!");
                }
            }
            finally
            {
                avoid_internal_loop = false;
            }
        }

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
                        Utilities.RimLog.Message(">>>>>>>>>> SPECIAL : " + thingDefName + " :: " + DefDatabase<SpecialThingFilterDef>.GetNamed(thingDefName, true) + " :: ");
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
                    if (ThingFilterPatch.thingFilterCallerStack.Count > 0)
                    {
                        object o = ThingFilterPatch.thingFilterCallerStack.Peek();
                        Utilities.RimLog.Message("SetAllow :::::::::: " + o);

                        if (o is Zone)
                        {
                            Thingfilter_setallowzone_wrap(thingDef.defName, allow, (o as Zone).ID, false);
                        }

                        if (o is Thing)
                        {
                            Thingfilter_setallow_wrap(thingDef.defName, allow, (o as Thing).thingIDNumber, false);
                        }

                        if (o is Bill)
                        {
                            Bill b = o as Bill;
                            BillStack bs = b.billStack;
                            Thing t = bs.billGiver as Thing;
                            int index = bs.Bills.IndexOf(b);
                            ThingFilter_setallow_bill_with_billgiver(thingDef.defName, allow, t.thingIDNumber, false, index);
                            Utilities.RimLog.Message("this actions is yet invalid for bills! giver is " + t);
                        }

                        if (o is FoodRestriction)
                        {
                            FoodRestriction fs = o as FoodRestriction;

                        }
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
                    if (ThingFilterPatch.thingFilterCallerStack.Count > 0)
                    {
                        object o = ThingFilterPatch.thingFilterCallerStack.Peek();
                        Utilities.RimLog.Message("SetAllow :::::::::: " + o);

                        if (o is Zone)
                        {
                            ThingFilter_wrapper.Thingfilter_setallowzone_wrap(sfDef.defName, allow, (o as Zone).ID, true);
                        }

                        if (o is Thing)
                        {
                            ThingFilter_wrapper.Thingfilter_setallow_wrap(sfDef.defName, allow, (o as Thing).thingIDNumber, true);
                        }

                        if (o is Bill)
                        {
                            Bill b = o as Bill;
                            BillStack bs = b.billStack;
                            Thing t = bs.billGiver as Thing;
                            int index = bs.Bills.IndexOf(b);
                            ThingFilter_wrapper.ThingFilter_setallow_bill_with_billgiver(sfDef.defName, allow, t.thingIDNumber, true, index);
                            Utilities.RimLog.Message("this actions is yet invalid for bills! giver is " + t);
                        }
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
    }*/

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
