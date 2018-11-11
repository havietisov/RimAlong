using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace CooperateRim
{
    /*
    [HarmonyPatch(typeof(RCellFinder))]
    [HarmonyPatch("RandomWanderDestFor")]
    class CellFinderPatch
    {
        class __l1
        {
            public float radius;
            public IntVec3 root;

            public bool l1(Region reg)
            {
                return reg.extentsClose.ClosestDistSquaredTo(root) <= radius * radius;
            }
        }

        class __l2
        {
            public Pawn pawn;
            public Func<Pawn, IntVec3, IntVec3, bool> validator;
            public IntVec3 root;

            public bool l2(IntVec3 c)
            {
                var res = c.InBounds(pawn.Map) && pawn.CanReach(c, PathEndMode.OnCell, Danger.None) && !c.IsForbidden(pawn) && (validator == null || validator(pawn, c, root));
                return res;
            }
        }

        [HarmonyPrefix]
        public static bool RandomWanderDestFor(Pawn pawn, IntVec3 root, float radius, Func<Pawn, IntVec3, IntVec3, bool> validator, Danger maxDanger, ref IntVec3 __result, ref List<Region> ___regions)
        {
            
            var CanWanderToCell = typeof(RCellFinder).GetMethod("CanWanderToCell", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Random r = new System.Random(TickManagerPatch.nextSyncTickValue);

            bool dirx = TickManagerPatch.nextSyncTickValue % 2 == 0 ? true : false;
            bool diry = TickManagerPatch.nextSyncTickValue % 4 == 0 ? true : false;

            __result = pawn.Position + new IntVec3(dirx ? (int)radius : -(int)radius, 0, diry ? (int)radius : -(int)radius);
            CooperateRimming.Log("Wander to : " + pawn + " :: " + __result  + " {" + TickManagerPatch.nextSyncTickValue + "}");
            pawn.Map.debugDrawer.FlashCell(__result, 0.4f, "wander");
            return false;
        }
    }*/


    public class Utils
    {
        public int calcZOrder(ushort xPos, ushort yPos)
        {
            int[] MASKS = new [] { 0x55555555, 0x33333333, 0x0F0F0F0F, 0x00FF00FF };
            int[] SHIFTS= new [] { 1, 2, 4, 8 };

            long x = xPos;  // Interleave lower 16 bits of x and y, so the bits of x
            long y = yPos;  // are in the even positions and bits from y in the odd;

            x = (x | (x << SHIFTS[3])) & MASKS[3];
            x = (x | (x << SHIFTS[2])) & MASKS[2];
            x = (x | (x << SHIFTS[1])) & MASKS[1];
            x = (x | (x << SHIFTS[0])) & MASKS[0];

            y = (y | (y << SHIFTS[3])) & MASKS[3];
            y = (y | (y << SHIFTS[2])) & MASKS[2];
            y = (y | (y << SHIFTS[1])) & MASKS[1];
            y = (y | (y << SHIFTS[0])) & MASKS[0];

            long result = x | (y << 1);
            return (int)result;
        }
    }
    

}
