using RimWorld;
using Verse;
using System;
using UnityEngine;
using RimWorld.Planet;
using Harmony;

namespace CooperateRim
{
    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
    public class pwnd
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            getValuePatch.GuardedPush();
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            getValuePatch.GuardedPush();
        }
    }

    [HarmonyPatch(typeof(PawnApparelGenerator), "GenerateStartingApparelFor")]
    public class generator_patch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(ThingIDMaker), "GiveIDTo")]
    public class ThingIDMakerPatch
    {
        public static bool stopID;

        [HarmonyPostfix]
        public static void Postfix(Thing t)
        {
            if (!stopID)
            {
                if (t is Pawn)
                {
                    CooperateRimming.Log("Made id for " + (t as Pawn) + " | " + Rand.Int + "|" + System.Threading.Thread.CurrentThread.ManagedThreadId);
                }
                else
                {
                    CooperateRimming.Log("Made id for " + (t) + " | " + Rand.Int + "|" + System.Threading.Thread.CurrentThread.ManagedThreadId);
                }
            }
        }
    }
    
    public partial class CooperateRimming
    {
        

        public class Dialog_Coop : Window
        {
            public override void DoWindowContents(Rect inRect)
            {
                int size = 52;
                Rect r = inRect;
                r.height = size - 1;
                r.width = 150;
                if (Widgets.ButtonText(r, "Connect to "))
                {
                    NetDemo.WaitForConnection(hostName);
                    GenerateWorld();
                }
                r.y += size;
                Widgets.ButtonText(r, "MEANINGLESS BUTTON"); r.y += size;
                hostName = Widgets.TextArea(r, hostName);
                //Widgets.Label(r, "hey, motherfuckers!");
            }

            static void ErrorHandler(System.Exception ex)
            {
                Log(ex.ToString());
            }

            static void GenerateMap()
            {
                if (Find.Root)
                {
                    //Find.Root.Start();
                }
                else {
                    Log("Null root");
                }
            }
        }
    }
}
