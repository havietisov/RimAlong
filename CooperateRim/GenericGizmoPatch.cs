using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CooperateRim
{
    public class GenericGizmoPatch
    {
        public static void Postfix(Thing __instance, ref IEnumerable<Gizmo> __result)
        {
            if (!SyncTickData.AvoidLoop)
            {
                List<Gizmo> glist = new List<Gizmo>();
                int i = 0;

                foreach (Gizmo g in __result)
                {
                    if (g is Command_Toggle)
                    {
                        int ii = i;
                        (g as Command_Toggle).toggleAction = () => { SyncTickData.AppendSyncTickDataCommand_toggle_call_by_index(__instance, ii); };
                    }

                    if (g is Command_Action)
                    {
                        int ii = i;
                        (g as Command_Action).action = () => { SyncTickData.AppendSyncTickDataCommand_toggle_call_by_index(__instance, ii); };
                    }
                    
                    glist.Add(g);
                    i++;
                }

                __result = glist;
            }
        }
    }
}
