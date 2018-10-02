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

                foreach (var frm in st.GetFrames())
                {
                    CooperateRimming.Log("designator== is  " + frm.GetMethod().DeclaringType + " | " + frm.GetMethod().DeclaringType.IsSubclassOf(typeof(Designator)));
                    if (frm.GetMethod().DeclaringType.IsSubclassOf(typeof(Designator)))
                    {
                        CooperateRimming.Log("designator is  " + frm.GetMethod().DeclaringType);
                        SyncTickData.AppendSyncTickData(newDes, frm.GetMethod().DeclaringType);
                        break;
                    }
                }
                
                CooperateRimming.Log("no proper designator!");
                
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
