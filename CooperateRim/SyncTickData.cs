﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Verse;
using Verse.AI;

namespace CooperateRim
{
    [Serializable]
    public struct SVEC3
    {
        public SVEC3(IntVec3 i)
        {
            x = i.x;
            y = i.y;
            z = i.z;
        }

        public int x;
        public int y;
        public int z;

        public static implicit operator IntVec3(SVEC3 @this)
        {
            return new IntVec3(@this.x, @this.y, @this.z);
        }

        public static implicit operator SVEC3(IntVec3 @this)
        {
            return new SVEC3(@this);
        }
    }

    [Serializable]
    public class S_Pawn
    {
        public S_Pawn(Pawn p) 
        {
            ThingID = p.ThingID;
        }

        public string ThingID;

        public static implicit operator S_Pawn(Pawn @this)
        {
            return new S_Pawn(@this);
        }
    }

    [Serializable]
    public class S_WorkGiver
    {
        public S_WorkGiver(WorkGiver g)
        {
            jobGiverClass = g.def.giverClass.ToString();
            __jobDefName = g.def.workType.defName;
        }

        public string __jobDefName;
        public string jobGiverClass;

        public static implicit operator S_WorkGiver(WorkGiver @this)
        {
            return new S_WorkGiver(@this);
        }
    }

    [Serializable]
    public class S_WorkTypeDef
    {
        public S_WorkTypeDef(WorkTypeDef w)
        {
            defName = w.defName;
        }

        public string defName;

        public static implicit operator S_WorkTypeDef(WorkTypeDef @this)
        {
            return new S_WorkTypeDef(@this);
        }
    }

    [Serializable]
    public class S_Thing
    {
        public S_Thing(Thing t)
        {
            ThingID = t.ThingID;
        }

        public string ThingID;

        public static implicit operator S_Thing(Thing @this)
        {
            return new S_Thing(@this);
        }
    }

    [Serializable]
    public class S_LocalTargetInfo
    {
        public S_LocalTargetInfo(LocalTargetInfo t)
        {
            cell = t.Cell;
            thing = t.Thing;
        }

        public SVEC3 cell;
        public S_Thing thing;

        public static implicit operator S_LocalTargetInfo(LocalTargetInfo @this)
        {
            return new S_LocalTargetInfo(@this);
        }
    }

    [Serializable]
    public class S_Designation
    {
        public S_Designation(Designation d)
        {
            designationDef = d.def.ToString();
            target = d.target;
        }

        public string designationDef;
        public S_LocalTargetInfo target;

        public static implicit operator S_Designation(Designation @this)
        {
            return new S_Designation(@this);
        }
    }
    
    [Serializable]
    public class SyncTickData : ISerializable
    {
        public class TemporaryJobData
        {
            public S_Pawn pawn;
            public S_Thing target;
            public bool forced;
            public Job __result;
        }

        [Serializable]
        public class FinalJobData
        {
            //public TemporaryJobData tj;
            public SVEC3 cell;
            public S_WorkGiver giver;
            public S_Pawn pawn;
            public S_Thing target;
            public bool forced;
        }

        [Serializable]
        public class JobPriorityData
        {
            public S_WorkTypeDef w;
            public int priority;
            public S_Pawn p;
        }

        [Serializable]
        public class SerializableZoneData
        {
            public int zoneID;
            public string zoneName;
            public Type zoneType;
        }
        
        List<TemporaryJobData> jobData = new List<TemporaryJobData>();

        List<S_Designation> designations = new List<S_Designation>();
        List<FinalJobData> jobsToSerialize = new List<FinalJobData>();
        List<JobPriorityData> jobPriorities = new List<JobPriorityData>();
        Dictionary<int, List<SVEC3>> zoneCellsAdded = new Dictionary<int, List<SVEC3>>();
        Dictionary<int, List<SVEC3>> zoneCellsRemoved = new Dictionary<int, List<SVEC3>>();
        List<SerializableZoneData> zonesCreated = new List<SerializableZoneData>();

        public static bool IsDeserializing;
        public static bool AvoidLoop;

        static SyncTickData singleton = new SyncTickData();

        public static void AppendSyncTickData(Designation des)
        {
            singleton.designations.Add(des);
        }

        public static void AppendSyncTickData(int zoneID, string zoneName, Type zoneType)
        {
            singleton.zonesCreated.Add(new SerializableZoneData() { zoneID = zoneID, zoneName = zoneName, zoneType = zoneType });
        }

        public static void AppendSyncTickData(int zoneID, IntVec3 cell, bool addOrRemove)
        {
            Dictionary<int, List<SVEC3>> dict = addOrRemove ? singleton.zoneCellsAdded : singleton.zoneCellsRemoved;

            if (!dict.ContainsKey(zoneID))
            {
                dict.Add(zoneID, new List<SVEC3>());
            }

            dict[zoneID].Add(cell);
        }

        public static void AppendSyncTickData(TemporaryJobData j)
        {
            singleton.jobData.Add(j);
        }

        public static void AppendSyncTickData(WorkTypeDef w, int priority, Pawn p)
        {
            singleton.jobPriorities.Add(new JobPriorityData() { w = w, priority = priority, p = p });
        }

        public SyncTickData()
        {

        }

        public static void FlushSyncTickData(int tickNum)
        {
            CooperateRimming.Log(" +=========================== +");
            if (new System.Collections.ICollection[]
            {
                singleton.jobsToSerialize,
                singleton.jobPriorities,
                singleton.designations,
                singleton.zoneCellsAdded,
                singleton.zoneCellsRemoved,
                singleton.zonesCreated
            }.Any(u => { return u.Count > 0; }))
            {
                BinaryFormatter ser = new BinaryFormatter();
                var fs = System.IO.File.OpenWrite(@"D:\CoopReplays\_" + tickNum + ".xml");
                SyncTickData buffered = singleton;
                singleton = new SyncTickData();
                ser.Serialize(fs, buffered);
                fs.Flush();
                fs.Close();
            }
            CooperateRimming.Log(" +++++++++++++++++++++++++++++++++++");
        }

        static DesignationDef DefFromString(string name)
        {
            foreach (var fi in typeof(RimWorld.DesignationDefOf).GetFields())
            {
                if (fi.GetValue(null).ToString() == name)
                {
                    return fi.GetValue(null) as DesignationDef;
                }
            }

            return null;
        }

        static void GetVal<T2>(ref List<T2> tVar, SerializationInfo info, string name)
        {
            tVar = (List<T2>)(info.GetValue(name, typeof(List<T2>)));
        }

        public SyncTickData(SerializationInfo info, StreamingContext ctx)
        {
            GetVal(ref designations, info, nameof(designations));
            GetVal(ref jobsToSerialize, info, nameof(jobsToSerialize));
            GetVal(ref jobPriorities, info, nameof(jobPriorities));

            foreach (var des in designations)
            {
                Thing sdt = null;

                foreach (Thing t in Find.CurrentMap.thingGrid.ThingsAt(des.target.cell))
                {
                    if (t.ThingID == des.target.thing.ThingID)
                    {
                        sdt = t;
                    }
                }

                AvoidLoop = true;
                Find.CurrentMap.designationManager.AddDesignation(new Designation(new LocalTargetInfo(sdt), DefFromString(des.designationDef)));
                AvoidLoop = false;

                CooperateRimming.Log(des.designationDef);
            }

            foreach(var prior in jobPriorities)
            {
                WorkTypeDef _wtd = null;
                Pawn pawn = null;

                foreach (var __workTypeDef in DefDatabase<WorkTypeDef>.AllDefsListForReading)
                {
                    if (__workTypeDef.defName == prior.w.defName)
                    {
                        _wtd = __workTypeDef;
                    }
                }

                foreach (var _pawn in Find.CurrentMap.mapPawns.AllPawns)
                {
                    if (_pawn.ThingID == prior.p.ThingID)
                    {
                        pawn = _pawn;
                        break;
                    }
                }

                if (pawn == null)
                {
                    foreach (var _pawn in CooperateRimming.initialPawnList)
                    {
                        if (_pawn.ThingID == prior.p.ThingID)
                        {
                            pawn = _pawn;
                            break;
                        }
                    }
                }

                CooperateRimming.Log("Find.GameInitData : " + Find.GameInitData);
                CooperateRimming.Log("pawn priority : " + pawn);
                AvoidLoop = true;
                pawn.workSettings.SetPriority(_wtd, prior.priority);
                AvoidLoop = false;
            }

            foreach (var _job in jobsToSerialize)
            {
                Pawn pawn = null;
                Thing th = null;
                WorkTypeDef workTypeDef = null;

                foreach (var thing in Find.CurrentMap.thingGrid.ThingsAt(_job.cell))
                {
                    if (thing.ThingID == _job.target.ThingID)
                    {
                        th = thing;
                        break;
                    }
                }

                foreach (var _pawn in Find.CurrentMap.mapPawns.AllPawns)
                {
                    if (_pawn.ThingID == _job.pawn.ThingID)
                    {
                        pawn = _pawn;
                        break;
                    }
                }

                foreach (var __workTypeDef in DefDatabase<WorkTypeDef>.AllDefsListForReading)
                {
                    CooperateRimming.Log("[" + __workTypeDef.defName + " : " + _job.giver.__jobDefName + "]");

                    if (__workTypeDef.defName == _job.giver.__jobDefName)
                    {
                        workTypeDef = __workTypeDef;
                        break;
                    }
                }

                CooperateRimming.Log(">>>>>>>>>>>>>");
                CooperateRimming.Log("workTypeDef " + (workTypeDef == null ? "null" : workTypeDef.ToString()));
                CooperateRimming.Log("thing :" + th + "]");
                CooperateRimming.Log("pawn :" + pawn + "]");
                CooperateRimming.Log("givers count : " + workTypeDef.workGiversByPriority.Count);

                for (int k = 0; k < workTypeDef.workGiversByPriority.Count; k++)
                {
                    WorkGiverDef workGiver = workTypeDef.workGiversByPriority[k];
                    WorkGiver_Scanner workGiver_Scanner = workGiver.Worker as WorkGiver_Scanner;
                    CooperateRimming.Log(workGiver_Scanner.def.defName);
                    CooperateRimming.Log(workGiver_Scanner.def.giverClass + " <> " + _job.giver.jobGiverClass);
                    if (workGiver_Scanner.def.giverClass.ToString() == _job.giver.jobGiverClass.ToString())
                    {
                        Job job = workGiver_Scanner.JobOnThing(pawn, th, forced: _job.forced);
                        job.playerForced = true;
                        job.TryMakePreToilReservations(pawn, errorOnFailed: true);
                        AvoidLoop = true;
                        CooperateRimming.Log("Ordered job : " + job + " : " + pawn.jobs.TryTakeOrderedJobPrioritizedWork(job, workGiver_Scanner, _job.cell));
                        AvoidLoop = false;
                        break;
                    }
                }
            }
        }

        internal static void AllowJobAt(Job job, WorkGiver giver, IntVec3 cell)
        {
            TemporaryJobData _tj = singleton.jobData.Find(u => u != null && u.__result != null && u.__result.loadID == job.loadID);
            CooperateRimming.Log("gtype : " + giver.def.giverClass);
            if (_tj != null)
            {
                singleton.jobsToSerialize.Add(new FinalJobData() { cell = cell, giver = giver, forced = _tj.forced, pawn = _tj.pawn, target = _tj.target });
            }
        }

        public static void Apply(int tickNum)
        {
            if (System.IO.File.Exists(@"D:\CoopReplays\_" + tickNum + ".xml"))
            {
                BinaryFormatter ser = new BinaryFormatter();
                var fs = System.IO.File.OpenRead(@"D:\CoopReplays\_" + tickNum + ".xml");
                SyncTickData sd = ser.Deserialize(fs) as SyncTickData;

                /*
                foreach (Designation des in sd.designations)
                {
                    AvoidLoop = true;
                    Find.CurrentMap.designationManager.AddDesignation(des);
                    AvoidLoop = false;
                }*/
            }
        }

        void Serialize(IntVec3 pos, SerializationInfo info, string prefix)
        {
            
        }

        /*
        void Serialize(LocalTargetInfo lti, SerializationInfo info, string prefix)
        {
            info.AddValue(prefix + ".valid", lti != null);

            if (lti != null)
            {
                info.AddValue(prefix + ".target.cell.x", lti.Cell.x.ToString());
                info.AddValue(prefix + ".target.cell.y", lti.Cell.y.ToString());
                info.AddValue(prefix + ".target.cell.z", lti.Cell.z.ToString());
                info.AddValue(prefix + ".target.thingID", lti.Thing.ThingID);
            }
        }*/

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //designations
            {
                info.AddValue("designations", designations);
            }

            //jobs
            {

                foreach (var j in jobsToSerialize)
                {
                    CooperateRimming.Log("++++ : " + j.giver.__jobDefName);
                }
                info.AddValue("jobsToSerialize", jobsToSerialize);
            }

            //job priorities
            {
                info.AddValue("jobPriorities", jobPriorities);
            }

            //added cells
            {
                info.AddValue("cells_added", zoneCellsAdded);
            }
            /*
            //removed_cells
            {
                info.AddValue("cells_removed", zoneCellsRemoved);
            }

            //zonesCreated
            {
                info.AddValue("zonesCreated", zonesCreated);
            }*/
        }
    }
}