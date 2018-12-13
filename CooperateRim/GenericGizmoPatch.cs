using CooperateRim.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace CooperateRim
{
    public class GenericGizmoMethods : common_patch_fields
    {
        public static void CallByIndex(Thing inst, int index)
        {
            int c = 0;
            use_native = true;
            try
            {
                foreach (Gizmo g in inst.GetGizmos())
                {
                    if (c++ == index)
                    {
                        if (g is Command_Toggle)
                        {
                            (g as Command_Toggle).toggleAction();
                        }

                        if (g is Command_Action)
                        {
                            (g as Command_Action).action();
                        }
                    }
                }
            }
            finally
            {
                use_native = false;
            }
        }
    }

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
                        (g as Command_Toggle).toggleAction = () => { GenericGizmoMethods.CallByIndex(__instance, ii); };
                    }

                    if (g is Command_Action)
                    {
                        int ii = i;
                        (g as Command_Action).action = () => { GenericGizmoMethods.CallByIndex(__instance, ii); };
                    }
                    
                    glist.Add(g);
                    i++;
                }

                __result = glist;
            }
        }
    }
}
