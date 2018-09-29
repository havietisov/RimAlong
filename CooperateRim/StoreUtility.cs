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
    }


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


    [HarmonyPatch(typeof(StoreUtility))]
    [HarmonyPatch("TryFindBestBetterStoreCellForWorker")]
    class StoreUtilityPatch
    {
        static int counter = 0;

        [HarmonyPrefix]
        public static bool TryFindBestBetterStoreCellForWorker(Thing t, Pawn carrier, Map map, Faction faction, SlotGroup slotGroup, bool needAccurateResult, ref IntVec3 closestSlot, ref float closestDistSquared, ref StoragePriority foundPriority)
        {
            if (slotGroup.parent.Accepts(t))
            {
                IntVec3 a = carrier.Position ;// (!t.SpawnedOrAnyParentSpawned) ? carrier.Position : t.Position;
                
                List<IntVec3> cellsList = slotGroup.CellsList;
                int count = cellsList.Count;

                List<KeyValuePair<float, IntVec3>> dList = new List<KeyValuePair<float, IntVec3>>();

                for (int i = 0; i < count; i++)
                {
                    IntVec3 intVec = cellsList[i];
                    float dist = (float)(a - intVec).LengthHorizontalSquared;
                    if (StoreUtility.IsGoodStoreCell(intVec, map, t, carrier, faction))
                    {
                        closestSlot = intVec;
                        closestDistSquared = dist;
                        foundPriority = slotGroup.Settings.Priority;
                        dList.Add(new KeyValuePair<float, IntVec3>(dist, intVec));
                    }
                }

                /*
                float feature_space_dist = float.MaxValue;
                float minY = float.MaxValue;

                foreach (var dlm in dList)
                {
                    Vector3 feature_space_vector = new Vector3(dlm.Value.x, dlm.Value.y, dlm.Key);

                    if (feature_space_dist > feature_space_vector.sqrMagnitude && minY > feature_space_vector.y)
                    {
                        feature_space_dist = feature_space_vector.sqrMagnitude;
                        closestSlot = dlm.Value + a;
                        closestDistSquared = dlm.Key;
                        minY = feature_space_vector.y;
                    }
                }*/

                dList.Sort((u1, u2) => (int)(10000 * (u1.Key - u2.Key)));
                var minCol = new List<KeyValuePair<float, IntVec3>>(dList.FindAll(u => dList[0].Key == u.Key));

                CooperateRimming.Log(minCol.Count + "");

                minCol.Sort((u1, u2) => u1.Value.x - u2.Value.x);
                closestSlot = minCol[0].Value;
                closestDistSquared = minCol[0].Key;
                CooperateRimming.Log(closestSlot.ToString());
                carrier.Map.debugDrawer.FlashCell(closestSlot, 0.4f, "haul");
            }

            return false;
        }

        [HarmonyPostfix]
        public static void pf()
        {
        }
    }

}
