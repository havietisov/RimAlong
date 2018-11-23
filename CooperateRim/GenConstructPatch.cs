using CooperateRim.Utilities;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CooperateRim
{
    //
    [HarmonyPatch(typeof(GenConstruct))]
    [HarmonyPatch("PlaceBlueprintForBuild")]
    public class GenConstructPatch
    {
        [HarmonyPrefix]
        public static bool Patch()
        {
            RimLog.Message("++++++++=PlaceBlueprintForBuild");
            return true;
        }
    }
}
