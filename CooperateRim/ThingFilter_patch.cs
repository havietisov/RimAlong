using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Dialog_BillConfig), methodName: "DoWindowContents")]
    public class Dialog_billConfigPatch
    {
        public static IntVec3 pos;
        public static Bill_Production bill;
        public static bool valid = false;

        [HarmonyPrefix]
        public static bool Prefix(ref IntVec3 ___billGiverPos, Bill_Production ___bill)
        {
            pos = ___billGiverPos;
            bill = ___bill;
            valid = true;
            return true;
        }

        [HarmonyPostfix]
        public static void PostFix()
        {
            valid = false;
        }
    }

    [HarmonyPatch(typeof(ThingFilter), "SetAllow", new System.Type[] { typeof(ThingDef), typeof(bool) })]
    public class ThingFilter_patch
    {
        [HarmonyPrefix]
        public static bool SetAllow(ref ThingDef thingDef, ref bool allow)
        {
            if (!SyncTickData.AvoidLoop)
            {
                if (Dialog_billConfigPatch.valid)
                {
                    SyncTickData.AppendSyncTickDataThingFilterSetAllow(thingDef, allow, Dialog_billConfigPatch.pos, Dialog_billConfigPatch.bill);
                }
                else
                {
                    CooperateRimming.Log("set allow while config is invalid, wtf?");
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
            return false;
        }
    }
}
