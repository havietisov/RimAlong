using Harmony;
using RimWorld;
using Verse;


namespace CooperateRim
{
    [HarmonyPatch(typeof(FoodRestrictionDatabase), "GenerateStartingFoodRestrictions")]
    public class FoodRestrictionDatabasePatch
    {
        public static void FixFoodRestrictions(FoodRestrictionDatabase __instance)
        {
            FoodRestriction lavishRestriction = __instance.MakeNewFoodRestriction();
            lavishRestriction.label = "FoodRestrictionLavish".Translate();
            FoodRestriction fineRestriction = __instance.MakeNewFoodRestriction();
            fineRestriction.label = "FoodRestrictionFine".Translate();
            ThingFilterPatch.thingFilterCallerStack.Push(fineRestriction);
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if (thingDef.ingestible != null && thingDef.ingestible.preferability >= FoodPreferability.MealLavish && thingDef != ThingDefOf.InsectJelly)
                {
                    fineRestriction.filter.SetAllow(thingDef, false);
                }
            }
            ThingFilterPatch.thingFilterCallerStack.Pop();
            FoodRestriction simpleRestriction = __instance.MakeNewFoodRestriction();
            simpleRestriction.label = "FoodRestrictionSimple".Translate();
            ThingFilterPatch.thingFilterCallerStack.Push(simpleRestriction);
            foreach (ThingDef thingDef2 in DefDatabase<ThingDef>.AllDefs)
            {
                if (thingDef2.ingestible != null && thingDef2.ingestible.preferability >= FoodPreferability.MealFine && thingDef2 != ThingDefOf.InsectJelly)
                {
                    simpleRestriction.filter.SetAllow(thingDef2, false);
                }
            }
            simpleRestriction.filter.SetAllow(ThingDefOf.MealSurvivalPack, false);
            ThingFilterPatch.thingFilterCallerStack.Pop();
            FoodRestriction pasteRestriction = __instance.MakeNewFoodRestriction();
            pasteRestriction.label = "FoodRestrictionPaste".Translate();
            ThingFilterPatch.thingFilterCallerStack.Push(pasteRestriction);
            foreach (ThingDef thingDef3 in DefDatabase<ThingDef>.AllDefs)
            {
                if (thingDef3.ingestible != null && thingDef3.ingestible.preferability >= FoodPreferability.MealSimple && thingDef3 != ThingDefOf.MealNutrientPaste && thingDef3 != ThingDefOf.InsectJelly && thingDef3 != ThingDefOf.Pemmican)
                {
                    pasteRestriction.filter.SetAllow(thingDef3, false);
                }
            }
            ThingFilterPatch.thingFilterCallerStack.Pop();
            FoodRestriction rawRestriction = __instance.MakeNewFoodRestriction();
            rawRestriction.label = "FoodRestrictionRaw".Translate();
            ThingFilterPatch.thingFilterCallerStack.Push(rawRestriction);
            foreach (ThingDef thingDef4 in DefDatabase<ThingDef>.AllDefs)
            {
                if (thingDef4.ingestible != null && thingDef4.ingestible.preferability >= FoodPreferability.MealAwful)
                {
                    rawRestriction.filter.SetAllow(thingDef4, false);
                }
            }
            rawRestriction.filter.SetAllow(ThingDefOf.Chocolate, false);
            ThingFilterPatch.thingFilterCallerStack.Pop();
            FoodRestriction foodRestriction6 = __instance.MakeNewFoodRestriction();
            foodRestriction6.label = "FoodRestrictionNothing".Translate();
            ThingFilterPatch.thingFilterCallerStack.Push(foodRestriction6);
            foodRestriction6.filter.SetDisallowAll(null, null);
            ThingFilterPatch.thingFilterCallerStack.Pop();
        }

        [HarmonyPrefix]
        public static bool Prefix(FoodRestrictionDatabase __instance)
        {
            FixFoodRestrictions(__instance);
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
