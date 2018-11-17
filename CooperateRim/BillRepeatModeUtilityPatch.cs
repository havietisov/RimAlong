using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(BillRepeatModeUtility), "MakeConfigFloatMenu")]
    public class BillRepeatModeUtilityPatch
    {
        public static void SetBillRepeatType(Bill_Production bill, BillRepeatModeDef repeatMode)
        {
            bill.repeatMode = repeatMode;
        }

        public static void SetBillRepeatCount(Bill_Production bill, int count)
        {
            bill.repeatCount = count;
        }

        public static bool Prefix(Bill_Production bill)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            list.Add(new FloatMenuOption(BillRepeatModeDefOf.RepeatCount.LabelCap, delegate ()
            {
                SetBillRepeatType(bill, BillRepeatModeDefOf.RepeatCount);
            }, MenuOptionPriority.Default, null, null, 0f, null, null));
            FloatMenuOption item = new FloatMenuOption(BillRepeatModeDefOf.TargetCount.LabelCap, delegate ()
            {
                if (!bill.recipe.WorkerCounter.CanCountProducts(bill))
                {
                    Messages.Message("RecipeCannotHaveTargetCount".Translate(), MessageTypeDefOf.RejectInput, false);
                }
                else
                {
                    SetBillRepeatType(bill, BillRepeatModeDefOf.TargetCount);
                }
            }, MenuOptionPriority.Default, null, null, 0f, null, null);
            list.Add(item);
            list.Add(new FloatMenuOption(BillRepeatModeDefOf.Forever.LabelCap, delegate ()
            {
                SetBillRepeatType(bill, BillRepeatModeDefOf.Forever);
            }, MenuOptionPriority.Default, null, null, 0f, null, null));
            Find.WindowStack.Add(new FloatMenu(list));
            return false;
        }
    }
}
