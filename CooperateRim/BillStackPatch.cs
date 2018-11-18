using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(BillStack), "DoListing")]
    class BillStackPatch
    {
        public static void MakeNewBillAt(Building_WorkTable table, RecipeDef recipe)
        {
            table.BillStack.AddBill(BillUtility.MakeNewBill(recipe));
        }

        [HarmonyPrefix]
        public static void Prefix(ref Func<List<FloatMenuOption>> recipeOptionsMaker, ITab_Bills __instance)
        {
            Func<List<FloatMenuOption>> bill_source = recipeOptionsMaker;
            Building_WorkTable selThing = (Building_WorkTable)Find.Selector.SingleSelectedThing;
            List<RecipeDef> rdef = selThing.def.AllRecipes;
            recipeOptionsMaker = () =>
            {
                List<FloatMenuOption> optList = bill_source();
                int ii = 0;

                foreach (FloatMenuOption opt in optList)
                {
                    int iii = ii;
                    RecipeDef _rdef = rdef[ii];
                    opt.action = () => { CooperateRimming.Log("recipe option clicked for recipe "  + _rdef); MakeNewBillAt(selThing, _rdef); };
                    ii++;
                }

                return optList;
            };
        }
    }
}
