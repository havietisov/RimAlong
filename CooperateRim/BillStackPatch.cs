using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(BillStack), "DoListing")]
    class BillStackPatch
    {
        [HarmonyPrefix]
        public static void Prefix(ref Func<List<FloatMenuOption>> recipeOptionsMaker, ITab_Bills __instance)
        {
            Func<List<FloatMenuOption>> bill_source = recipeOptionsMaker;

            recipeOptionsMaker = () =>
            {
                List<FloatMenuOption> optList = bill_source();

                foreach (FloatMenuOption opt in optList)
                {
                    opt.action = () => { CooperateRimming.Log("recipe option clicked "); };
                }

                return optList;
            };
        }
    }
}
