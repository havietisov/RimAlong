using CooperateRim.Utilities;
using Harmony;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Verse;

namespace CooperateRim
{
    [HarmonyPatch(typeof(Verse.DesignationManager))]
    [HarmonyPatch("AddDesignation")]
    class DesignationManagerPatch
    {
        [HarmonyPrefix]
        public static bool AddDesignation(Designation newDes)
        {
            if (!SyncTickData.AvoidLoop)
            {
                StackTrace st = new StackTrace();
                bool hasDesignator = false;
                foreach (var frm in st.GetFrames())
                {
                    RimLog.Message("designator== is  " + frm.GetMethod().DeclaringType + " | " + frm.GetMethod().DeclaringType.IsSubclassOf(typeof(Designator)));
                    if (frm.GetMethod().DeclaringType.IsSubclassOf(typeof(Designator)))
                    {
                        RimLog.Message("designator is  " + frm.GetMethod().DeclaringType);
                        SyncTickData.AppendSyncTickData(newDes, frm.GetMethod().DeclaringType);
                        hasDesignator = true;
                        break;
                    }
                }

                if (!hasDesignator)
                {
                    RimLog.Message("no proper designator!");
                }
                
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
