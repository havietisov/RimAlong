using Harmony;
using RimWorld;

namespace CooperateRim
{
    /*
    [HarmonyPatch(typeof(BillStack))]
    [HarmonyPatch("AddBill")]
    public class BillStackPatch
    {
        [HarmonyPrefix]
        public static bool AddBill(ref Bill bill, BillStack __instance)
        {
            if (!SyncTickData.AvoidLoop)
            {
                Utilities.RimLog.Message(">>>>>>>>>>>>> " + __instance.billGiver.ToString());
                SyncTickData.AppendSyncTickData(bill, __instance.billGiver);
                return false;
            }
            else
            {
                return true;
            }
        }

        [HarmonyPostfix]
        public static void Postfix(BillStack __instance, Bill bill)
        {
            ReverseBillTable.Associate(bill, __instance);
        }
    }

    [HarmonyPatch(typeof(BillStack))]
    [HarmonyPatch("Delete")]
    public class BillStackPatchDelete
    {
        [HarmonyPrefix]
        public static bool Delete(ref Bill bill, BillStack __instance)
        {
            if (!SyncTickData.AvoidLoop)
            {
                Utilities.RimLog.Message(">>>>>>>>>>>>> " + __instance.billGiver.ToString());
                SyncTickData.AppendSyncTickData(bill, __instance.billGiver);
                return false;
            }
            else
            {
                return true;
            }
        }

        [HarmonyPostfix]
        public static void Postfix(BillStack __instance, Bill bill)
        {
            ReverseBillTable.Associate(bill, __instance);
        }
    }*/

    /*
    [HarmonyPatch(typeof(RimWorld.BillStack))]
    [HarmonyPatch("AddBill")]
    public class BillStackPatch
    {
        [HarmonyPrefix]
        public static bool AddBill(ref Bill bill, BillStack __instance)
        {
            if (!SyncTickData.AvoidLoop)
            {
                Utilities.RimLog.Message(">>>>>>>>>>>>> " + __instance.billGiver.ToString());
                SyncTickData.AppendSyncTickData(bill, __instance.billGiver);
                return false;
            }
            else
            {
                return true;
            }
        }
    }*/


    [HarmonyPatch(typeof(RimWorld.Bill_Production))]
    [HarmonyPatch("DoConfigInterface")]
    public class Bill_production_patch
    {
        public class ctx
        {
            public int repeatCount;
            public int targetCount;
        }
        
        [HarmonyPrefix]
        public static bool Prefix(Bill_Production __instance, ref ctx __state)
        {
            __state = new ctx();
            __state.repeatCount = __instance.repeatCount;
            __state.targetCount = __instance.targetCount;
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(Bill_Production __instance, ctx __state, ref BillStack ___billStack)
        {
            if (__state.repeatCount != __instance.repeatCount)
            {
                int b = __instance.repeatCount;
                __instance.repeatCount = __state.repeatCount;
                BillRepeatModeUtilityPatch.SetBillRepeatCount(__instance, b);
            }

            if (__state.targetCount != __instance.targetCount)
            {
                int b = __instance.targetCount;
                __instance.targetCount = __state.targetCount;
                BillRepeatModeUtilityPatch.SetBillTargetCount(__instance, b);
            }
        }
    }
}
