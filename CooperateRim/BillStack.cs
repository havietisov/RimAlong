using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CooperateRim
{
    [HarmonyPatch(typeof(RimWorld.BillStack))]
    [HarmonyPatch("AddBill")]
    public class BillStackPatch
    {
        [HarmonyPrefix]
        public static bool AddBill(ref Bill bill, BillStack __instance)
        {
            if (!SyncTickData.AvoidLoop)
            {
                CooperateRimming.Log(">>>>>>>>>>>>> " + __instance.billGiver.ToString());
                SyncTickData.AppendSyncTickData(bill, __instance.billGiver);
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(RimWorld.Bill_Production))]
    [HarmonyPatch("DoConfigInterface")]
    public class Bill_production_patch
    {
        public class ctx
        {
            public bool hasValue;
            public BillRepeatModeDef temp_def;
        }

        static string def_name_t;

        public static Dictionary<Bill_Production, BillRepeatModeDef> kkk = new Dictionary<Bill_Production, BillRepeatModeDef>();

        [HarmonyPrefix]
        public static bool Prefix(Bill_Production __instance, ref ctx __state)
        {
            __state = new ctx();

            if(!kkk.ContainsKey(__instance))
            {
                kkk.Add(__instance, null);
            }

            //kkk[__instance] = __instance.repeatMode.defName;
            /*
            def_name_t = __instance.repeatMode.defName;
            __state.hasValue = true;*/
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(Bill_Production __instance, ctx __state, ref BillStack ___billStack)
        {
            if (!kkk.ContainsKey(__instance))
            {
                kkk[__instance] = __instance.repeatMode;
            }
            else
            {
                if (kkk[__instance] != null)
                {
                   // CooperateRimming.Log("DoConfInterface " + kkk[__instance].defName + " || " + __instance.repeatMode.defName);
                }
                    
                if (null == (kkk[__instance]))
                {
                    kkk[__instance] = __instance.repeatMode;
                    
                }

                if (kkk[__instance] != __instance.repeatMode)
                {
                    SyncTickData.AppendSyncTickData(__instance, ___billStack.billGiver, __instance.repeatMode);
                    __instance.repeatMode = kkk[__instance];
                }
            }
            /*
            if (__state!=null)
            {
                if (__instance.repeatMode.defName != __state.temp_def)
                {
                    CooperateRimming.Log("obtained bill state = " + __state.temp_def);
                    SyncTickData.AppendSyncTickData(__instance, ___billStack.billGiver, __instance.repeatMode);
                    //__instance.repeatMode = __state;
                }
            }*/
        }
    }
}
