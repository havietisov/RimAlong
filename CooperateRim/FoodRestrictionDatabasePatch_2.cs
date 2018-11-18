using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Verse;


namespace CooperateRim
{
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
}
