//using CooperateRim.Utilities;
//using Harmony;
//using RimWorld;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using Verse;

//namespace CooperateRim
//{
//    /*
//    [HarmonyPatch(typeof(Designator_Build))]
//    [HarmonyPatch("DesignateSingleCell")]
//    public class Designator_build
//    {
//        [HarmonyPrefix]
//        public static bool Prefix(ref Designator __instance, ref IntVec3 c, ref BuildableDef ___entDef, ref Rot4 ___placingRot, ref ThingDef ___stuffDef)
//        {
//            RimLog.Message("Designator_Build designate single cell " + (___stuffDef == null ? "null" : ___stuffDef.ToString()) + " || " + (___stuffDef == null ? "null" : ___stuffDef.defName));
//            ThingDef td = ___stuffDef;

//            if (!SyncTickData.AvoidLoop)
//            {
//                //DefDatabase<ThingDef>.AllDefs
//                SyncTickData.AppendSyncTickDataDesignatorSingleCell(__instance, c, ___entDef, ___placingRot, ___stuffDef);
//                return false;
//            }
//            else
//            {
//                return true;
//            }
//        }
//    }*/
    
//    //[HarmonyPatch(typeof(Designator_ZoneDelete))]
//    //[HarmonyPatch("DesignateSingleCell")]
//    //class Designator_patch__
//    //{
//    //    [HarmonyPrefix]
//    //    public static bool DesignateSingleCell_1(ref Designator __instance, ref IntVec3 c)
//    //    {
//    //        RimLog.Message("DELETE : " + __instance.GetType().AssemblyQualifiedName + ".DesignateSingleCell");
//    //        if (!SyncTickData.AvoidLoop)
//    //        {
//    //            SyncTickData.AppendSyncTickData(__instance, c);
//    //            return false;
//    //        }
//    //        else
//    //        {
//    //            return true;
//    //        }
//    //    }
//    //}

//    //[HarmonyPatch(typeof(Designator_AreaBuildRoof))]
//    //[HarmonyPatch("DesignateSingleCell")]
//    //class Designator_patch
//    //{
//    //    //[HarmonyPrefix]
//    //    public static bool DesignateSingleCell_1(ref Designator __instance, ref IntVec3 c)
//    //    {
//    //        RimLog.Message(__instance.GetType().AssemblyQualifiedName + ".DesignateSingleCell");
//    //        if (!SyncTickData.AvoidLoop)
//    //        {
//    //            SyncTickData.AppendSyncTickData(__instance, c);
//    //            return false;
//    //        }
//    //        else
//    //        {
//    //            return true;
//    //        }
//    //    }

//    //    public static bool DesignateSingleCell_2(ref Designator __instance, ref IntVec3 loc)
//    //    {
//    //        RimLog.Message(__instance.GetType().AssemblyQualifiedName + ".DesignateSingleCell");
//    //        if (!SyncTickData.AvoidLoop)
//    //        {
//    //            SyncTickData.AppendSyncTickData(__instance, loc);
//    //            return false;
//    //        }
//    //        else
//    //        {
//    //            return true;
//    //        }
//    //    }
//        /*
//        public static void DesignateMultiCell_fix(Designator_ZoneAdd __instance, IEnumerable<IntVec3> cells, Type zoneTypeToPlace)
//        {
//            List<IntVec3> list = cells.ToList<IntVec3>();
//            Zone SelectedZone = null;
//            MethodInfo mi = __instance.GetType().GetMethod("MakeNewZone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

//            if (list.Count == 1)
//            {
//                Zone zone = __instance.Map.zoneManager.ZoneAt(list[0]);
//                if (zone != null)
//                {
//                    if (zone.GetType() == zoneTypeToPlace)
//                    {
//                        //__instance.SelectedZone = zone;
//                    }
//                    return;
//                }
//            }
//            if (SelectedZone == null)
//            {
//                Zone zone2 = null;
//                foreach (IntVec3 c3 in cells)
//                {
//                    Zone zone3 = __instance.Map.zoneManager.ZoneAt(c3);
//                    if (zone3 != null && zone3.GetType() == zoneTypeToPlace)
//                    {
//                        if (zone2 == null)
//                        {
//                            zone2 = zone3;
//                        }
//                        else if (zone3 != zone2)
//                        {
//                            zone2 = null;
//                            break;
//                        }
//                    }
//                }
//                SelectedZone = zone2;
//            }
//            list.RemoveAll((IntVec3 c) => __instance.Map.zoneManager.ZoneAt(c) != null);
//            if (list.Count == 0)
//            {
//                return;
//            }
//            if (TutorSystem.TutorialMode && !TutorSystem.AllowAction(new EventPack(__instance.TutorTagDesignate, list)))
//            {
//                return;
//            }
//            if (SelectedZone == null)
//            {
//                SelectedZone = (Zone)mi.Invoke(__instance, new object[] { });
//                SelectedZone.Map.zoneManager.RegisterZone(SelectedZone);
//                SelectedZone.AddCell(list[0]);
//                list.RemoveAt(0);
//            }
//            bool somethingSucceeded;
//            for (; ; )
//            {
//                somethingSucceeded = true;
//                int count = list.Count;
//                for (int i = list.Count - 1; i >= 0; i--)
//                {
//                    bool flag = false;
//                    for (int j = 0; j < 4; j++)
//                    {
//                        IntVec3 c2 = list[i] + GenAdj.CardinalDirections[j];
//                        if (c2.InBounds(__instance.Map))
//                        {
//                            if (__instance.Map.zoneManager.ZoneAt(c2) == SelectedZone)
//                            {
//                                flag = true;
//                                break;
//                            }
//                        }
//                    }
//                    if (flag)
//                    {
//                        SelectedZone.AddCell(list[i]);
//                        list.RemoveAt(i);
//                    }
//                }
//                if (list.Count == 0)
//                {
//                    break;
//                }
//                if (list.Count == count)
//                {
//                    SelectedZone = (Zone)mi.Invoke(__instance, new object[] { });
//                    __instance.Map.zoneManager.RegisterZone(SelectedZone);
//                    SelectedZone.AddCell(list[0]);
//                    list.RemoveAt(0);
//                }
//            }
//            SelectedZone.CheckContiguous();
//            __instance.Finalize(somethingSucceeded);
//            TutorSystem.Notify_Event(new EventPack(__instance.TutorTagDesignate, list));
//            List<IntVec3> cellList = SelectedZone.Cells;
//            cellList.GetEnumerator();
//        }*/
//        /*
//        public static bool DesignateMultiCell(ref Designator __instance, IEnumerable<IntVec3> cells)
//        {
//            if (__instance is Designator_Build)
//            {
//                return true;
//            }
//            RimLog.Message("DesignateMultiCell_1");
//            if (!SyncTickData.AvoidLoop)
//            {
//                SyncTickData.AppendSyncTickData(__instance, cells);
//                return false;
//            }
//            else
//            {
//                return true;
//            }
//        }*/
        
//        /*
//        public static bool DesignateMultiCell_for_zone_add(ref Designator_ZoneAdd __instance, IEnumerable<IntVec3> cells, Type ___zoneTypeToPlace)
//        {
//            RimLog.Message("DesignateMultiCell_1");
//            if (!SyncTickData.AvoidLoop)
//            {
//                SyncTickData.AppendSyncTickData(__instance, cells);
//                return false;
//            }
//            else
//            {
//                DesignateMultiCell_fix(__instance, cells, ___zoneTypeToPlace);
//                return false;
//            }
//        }*/

//        //public static bool FinalizeDesignationSucceeded(ref Designator __instance)
//        //{
//        //    RimLog.Message("FinalizeDesignationSucceeded");
//        //    return true;
//        //    /*
//        //    if (!SyncTickData.AvoidLoop)
//        //    {
//        //        SyncTickData.AppendSyncTickDataFinalizeDesignationSucceeded(__instance);
//        //        return false;
//        //    }
//        //    else
//        //    {
//        //        return true;
//        //    }*/
//        //}

//        //public static bool DesignateThing(Designator __instance, Thing t)
//        //{
//        //    RimLog.Message(__instance.GetType().AssemblyQualifiedName + ".DesignateThing");
//        //    if (!SyncTickData.AvoidLoop)
//        //    {
//        //        SyncTickData.AppendSyncTickDesignatorApplyToThing(__instance, t, t.Position);
//        //        return false;
//        //    }
//        //    else
//        //    {
//        //        return true;
//        //    }
//        //}
//    //}
//}
