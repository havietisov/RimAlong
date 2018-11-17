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

    [HarmonyPatch(typeof(FoodRestrictionDatabase), "MakeNewFoodRestriction")]
    public class FoodRestrictionDatabasePatch_2
    {
        public static void FixFoodRestruction(FoodRestrictionDatabase __instance, ref FoodRestriction __result, ref List<FoodRestriction> ___foodRestrictions)
        {
            int num;
            if (___foodRestrictions.Any<FoodRestriction>())
            {
                num = ___foodRestrictions.Max((FoodRestriction o) => o.id) + 1;
            }
            else
            {
                num = 1;
            }
            int id = num;
            FoodRestriction foodRestriction = new FoodRestriction(id, "FoodRestriction".Translate() + " " + id.ToString());
            ThingFilterPatch.thingFilterCallerStack.Push(foodRestriction);
            foodRestriction.filter.SetAllow(ThingCategoryDefOf.Foods, true, null, null);
            foodRestriction.filter.SetAllow(ThingCategoryDefOf.CorpsesHumanlike, true, null, null);
            foodRestriction.filter.SetAllow(ThingCategoryDefOf.CorpsesAnimal, true, null, null);
            ThingFilterPatch.thingFilterCallerStack.Pop();
            ___foodRestrictions.Add(foodRestriction);
            __result = foodRestriction;
        }

        [HarmonyPrefix]
        public static bool Prefix(ref FoodRestriction __result, FoodRestrictionDatabase __instance, ref List<FoodRestriction> ___foodRestrictions)
        {
            FixFoodRestruction(__instance, ref __result, ref ___foodRestrictions);
            return false;
        }
    }

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

    [HarmonyPatch(typeof(OutfitDatabase), "GenerateStartingOutfits")]
    public class OutfitDatabasePatch
    {
        public static void FixStartingOutfits(OutfitDatabase __instance)
        {
            Outfit outfit = __instance.MakeNewOutfit();
            outfit.label = "OutfitAnything".Translate();
            Outfit outfit2 = __instance.MakeNewOutfit();
            outfit2.label = "OutfitWorker".Translate();
            ThingFilterPatch.thingFilterCallerStack.Push(outfit2);
            outfit2.filter.SetDisallowAll(null, null);
            outfit2.filter.SetAllow(SpecialThingFilterDefOf.AllowDeadmansApparel, false);
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if (thingDef.apparel != null && thingDef.apparel.defaultOutfitTags != null && thingDef.apparel.defaultOutfitTags.Contains("Worker"))
                {
                    outfit2.filter.SetAllow(thingDef, true);
                }
            }
            ThingFilterPatch.thingFilterCallerStack.Pop();
            Outfit outfit3 = __instance.MakeNewOutfit();
            outfit3.label = "OutfitSoldier".Translate();
            ThingFilterPatch.thingFilterCallerStack.Push(outfit3);
            outfit3.filter.SetDisallowAll(null, null);
            outfit3.filter.SetAllow(SpecialThingFilterDefOf.AllowDeadmansApparel, false);
            foreach (ThingDef thingDef2 in DefDatabase<ThingDef>.AllDefs)
            {
                if (thingDef2.apparel != null && thingDef2.apparel.defaultOutfitTags != null && thingDef2.apparel.defaultOutfitTags.Contains("Soldier"))
                {
                    outfit3.filter.SetAllow(thingDef2, true);
                }
            }
            ThingFilterPatch.thingFilterCallerStack.Pop();
            Outfit outfit4 = __instance.MakeNewOutfit();
            outfit4.label = "OutfitNudist".Translate();
            ThingFilterPatch.thingFilterCallerStack.Push(outfit4);
            outfit4.filter.SetDisallowAll(null, null);
            outfit4.filter.SetAllow(SpecialThingFilterDefOf.AllowDeadmansApparel, false);
            foreach (ThingDef thingDef3 in DefDatabase<ThingDef>.AllDefs)
            {
                if (thingDef3.apparel != null && !thingDef3.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs) && !thingDef3.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso))
                {
                    outfit4.filter.SetAllow(thingDef3, true);
                }
            }
            ThingFilterPatch.thingFilterCallerStack.Pop();
        }

        [HarmonyPrefix]
        public static bool Prefix(OutfitDatabase __instance)
        {
            FixStartingOutfits(__instance);
            return false;
        }
    }

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

    [HarmonyPatch(typeof(Zone_Stockpile), MethodType.Constructor, new Type[] { typeof(StorageSettingsPreset), typeof(ZoneManager) })]
    public class zone_stockpile_ctor_patch
    {
        [HarmonyPrefix]
        public static void Prefix(Zone_Stockpile __instance)
        {
            ThingFilterPatch.thingFilterCallerStack.Push(__instance);
        }

        [HarmonyPostfix]
        public static void Postfix(Zone_Stockpile __instance)
        {
            ThingFilterPatch.thingFilterCallerStack.Pop();
        }
    }

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

    public class thingfilter_methods
    {
        public static void SetAllowAllFor(object o)
        {
            CooperateRimming.Log("Setallowall for " + o);
        }

        public static void SetDisallowAllFor(object o)
        {
            CooperateRimming.Log("Setdisallowall for " + o);
        }

        public static void SetAllowance(object o, ThingDef def, bool isSpecial, bool isAllow)
        {
            CooperateRimming.Log("SetAllowance for " + o + "::" + def + "::" + isAllow);
        }
    }


    [HarmonyPatch(typeof(ThingFilter), "SetAllow", new System.Type[] { typeof(ThingDef), typeof(bool) })]
    public class ThingFilter_setallow_wrapper
    {
        [HarmonyPrefix]
        public static bool Prefix(ThingFilter __instance, ThingDef thingDef, bool allow)
        {
            thingfilter_methods.SetAllowance(ThingFilterPatch.thingFilterCallerStack.Peek(), def : thingDef, isAllow: allow, isSpecial: false);
            return false;
        }
    }

    [HarmonyPatch(typeof(ThingFilter), "SetAllowAll", new System.Type[] { typeof(ThingFilter) })]
    public class ThingFilter_setallowall_wrapper
    {
        [HarmonyPrefix]
        public static bool Prefix(ThingFilter __instance)
        {
            thingfilter_methods.SetAllowAllFor(ThingFilterPatch.thingFilterCallerStack.Peek());
            return false;
        }
    }

    [HarmonyPatch(typeof(ThingFilter), "SetDisallowAll", new System.Type[] { typeof(IEnumerable<ThingDef>), typeof(IEnumerable<SpecialThingFilterDef>) })]
    public class ThingFilter_setdisallowall_wrapper
    {
        [HarmonyPrefix]
        public static bool Prefix(ThingFilter __instance)
        {
            thingfilter_methods.SetDisallowAllFor(ThingFilterPatch.thingFilterCallerStack.Peek());
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
                        CooperateRimming.Log("SetAllow :::::::::: " + o);

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
                            CooperateRimming.Log("this actions is yet invalid for bills! giver is " + t);
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
                        CooperateRimming.Log("SetAllow :::::::::: " + o);

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
                            CooperateRimming.Log("this actions is yet invalid for bills! giver is " + t);
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
                CooperateRimming.Log("ThingFilter_setallow_bill_with_billgiver");
                {
                    Thing issuer = things.Where(u => u.Count != 0).First(u => u.Any(uu => uu.thingIDNumber == thingIDNumber)).First(u => u.thingIDNumber == thingIDNumber);
                    CooperateRimming.Log(">>>>>>>>>>> issuer :  " + issuer + " :: " + (issuer as IBillGiver));
                    billgiver = issuer as IBillGiver;
                }

                if (billgiver != null)
                {
                    if (!isSpecial)
                    {
                        CooperateRimming.Log(">>>>>>>>>>> billgiver.BillStack.Bills[billIndex].ingredientFilter.SetAllow " + billIndex + " |" + thingDefName + "| " + billgiver.BillStack.Bills[billIndex]);
                        billgiver.BillStack.Bills[billIndex].ingredientFilter.SetAllow(DefDatabase<ThingDef>.GetNamed(thingDefName, true), allow);
                    }
                    else
                    {
                        CooperateRimming.Log(">>>>>>>>>>> billgiver.BillStack.Bills[billIndex].ingredientFilter.SetAllow " + billIndex + " |" + thingDefName + "| " + billgiver.BillStack.Bills[billIndex]);
                        billgiver.BillStack.Bills[billIndex].ingredientFilter.SetAllow(DefDatabase<SpecialThingFilterDef>.GetNamed(thingDefName, true), allow);
                    }
                }
                else
                {
                    CooperateRimming.Log("missing bill giver!");
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
                    if (ThingFilterPatch.thingFilterCallerStack.Count > 0)
                    {
                        object o = ThingFilterPatch.thingFilterCallerStack.Peek();
                        CooperateRimming.Log("SetAllow :::::::::: " + o);

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
                            CooperateRimming.Log("this actions is yet invalid for bills! giver is " + t);
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
                        CooperateRimming.Log("SetAllow :::::::::: " + o);

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
                            CooperateRimming.Log("this actions is yet invalid for bills! giver is " + t);
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
