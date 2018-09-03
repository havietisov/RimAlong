using RimWorld;
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
    public class SyncTickData : ISerializable
    {
        public class TemporaryJobData
        {
            public Pawn pawn;
            public Thing target;
            public bool forced;
            public Job __result;
        }

        public class FinalJobData
        {
            public TemporaryJobData tj;
            public IntVec3 cell;
            public WorkGiver giver;
        }

        List<Designation> designations = new List<Designation>();
        List<TemporaryJobData> jobData = new List<TemporaryJobData>();
        List<FinalJobData> jobsToSerialize = new List<FinalJobData>();

        public static bool IsDeserializing;

        static SyncTickData singleton = new SyncTickData();

        public static void AppendSyncTickData(Designation des)
        {
            singleton.designations.Add(des);
        }

        public static void AppendSyncTickData(TemporaryJobData j)
        {
            singleton.jobData.Add(j);
        }

        public SyncTickData()
        {

        }

        public static void FlushSyncTickData(int tickNum)
        {
            if (singleton.designations.Count > 0 || singleton.jobsToSerialize.Count > 0)
            {
                BinaryFormatter ser = new BinaryFormatter();
                var fs = System.IO.File.OpenWrite(@"D:\CoopReplays\_" + tickNum + ".xml");
                ser.Serialize(fs, singleton);
                fs.Flush();
                fs.Close();
                singleton = new SyncTickData();
            }
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

        public SyncTickData(SerializationInfo info, StreamingContext ctx)
        {
            {
                int size = info.GetInt32("designations.amount");

                for (int i = 0; i < size; i++)
                {
                    DesignationDef def = DefFromString(info.GetString("des.[" + i + "].def"));
                    int x = int.Parse(info.GetString("des.[" + i + "].target.cell.x"));
                    int y = int.Parse(info.GetString("des.[" + i + "].target.cell.y"));
                    int z = int.Parse(info.GetString("des.[" + i + "].target.cell.z"));
                    string thingID = info.GetString("des.[" + i + "].target.thingID");
                    Thing sdt = null;

                    foreach (Thing t in Find.CurrentMap.thingGrid.ThingsAt(new IntVec3(x, y, z)))
                    {
                        if (t.ThingID == thingID)
                        {
                            sdt = t;
                        }
                    }

                    designations.Add(new Designation(new LocalTargetInfo(sdt), def));
                }
            }
            
            {
                int size = int.Parse(info.GetString("jobDesc.amount"));
                CooperateRimming.Log("jobs to deserialize " + size);

                for (int i = 0; i < size; i++)
                {   
                    string pawnID = info.GetString("jobDesc.[" + i + "].pawnID");
                    string targetID = info.GetString("jobDesc.[" + i + "].targetID");
                    string forced = info.GetString("jobDesc.[" + i + "].forced");
                    string _x = info.GetString("jobDesc.[" + i + "].cell.x");
                    string _y = info.GetString("jobDesc.[" + i + "].cell.y");
                    string _z = info.GetString("jobDesc.[" + i + "].cell.z");
                    string jobdefName = info.GetString("jobDesc.[" + i + "].jobDefName");
                    string jobGiverClass = info.GetString("jobDesc.[" + i + "].jobGiverClass");
                    Pawn pawn = null;
                    IntVec3 cell = new IntVec3(int.Parse(_x), int.Parse(_y), int.Parse(_z));
                    Thing th = null;
                    WorkTypeDef workTypeDef = null;

                    foreach (var thing in Find.CurrentMap.thingGrid.ThingsAt(cell))
                    {
                        if (thing.ThingID == targetID)
                        {
                            th = thing;
                            break;
                        }
                    }

                    foreach (var _pawn in Find.CurrentMap.mapPawns.AllPawns)
                    {
                        if (_pawn.ThingID == pawnID)
                        {
                            pawn = _pawn;
                            break;
                        }
                    }

                    foreach (var __workTypeDef in DefDatabase<WorkTypeDef>.AllDefsListForReading)
                    {
                        CooperateRimming.Log("[" + __workTypeDef.defName + " : " + jobdefName + "]");

                        if (__workTypeDef.defName == jobdefName)
                        {
                            workTypeDef = __workTypeDef;
                            //break;
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
                        if (workGiver_Scanner.def.giverClass.ToString() == jobGiverClass)
                        {
                            Job job = workGiver_Scanner.JobOnThing(pawn, th, forced: bool.Parse(forced));
                            job.playerForced = true;
                            job.TryMakePreToilReservations(pawn, errorOnFailed: true);
                            CooperateRimming.Log("Ordered job : " + job + " : " + pawn.jobs.TryTakeOrderedJobPrioritizedWork(job, workGiver_Scanner, cell));
                            break;
                        }
                    }
                    CooperateRimming.Log("<<<<<<<<<<<<<<");

                    /*
                    IntVec3 clickCell = new IntVec3(int.Parse(cellx), int.Parse(celly), int.Parse(cellz));



                    foreach (Thing item in pawn.Map.thingGrid.ThingsAt(clickCell))
                    {

                    }*/

                    /*
                    foreach (var workTypeDef in DefDatabase<WorkTypeDef>.AllDefsListForReading)
                    {
                        if (workTypeDef.defName == jobdefName)
                        {
                            for (int k = 0; k < workTypeDef.workGiversByPriority.Count; k++)
                            {
                                WorkGiverDef workGiver = workTypeDef.workGiversByPriority[k];
                                WorkGiver_Scanner workGiver_Scanner = workGiver.Worker as WorkGiver_Scanner;

                                if (workGiver_Scanner.def.defName == workTypeDef.defName)
                                {
                                    Job job = workGiver_Scanner.JobOnThing(pawn, item, forced: true);
                                }
                            }
                        }
                    }*/
                }
            }
        }

        internal static void AllowJobAt(Job job, WorkGiver giver, IntVec3 cell)
        {
            TemporaryJobData _tj = singleton.jobData.Find(u => u != null && u.__result != null && u.__result.loadID == job.loadID);
            CooperateRimming.Log("gtype : " + giver.def.giverClass);
            if (_tj != null)
            {
                singleton.jobsToSerialize.Add(new FinalJobData() { cell = cell, giver = giver, tj = _tj });
            }
        }

        public static void Apply(int tickNum)
        {
            if (System.IO.File.Exists(@"D:\CoopReplays\_" + tickNum + ".xml"))
            {
                BinaryFormatter ser = new BinaryFormatter();
                var fs = System.IO.File.OpenRead(@"D:\CoopReplays\_" + tickNum + ".xml");
                SyncTickData sd = ser.Deserialize(fs) as SyncTickData;

                foreach (Designation des in sd.designations)
                {
                    Find.CurrentMap.designationManager.AddDesignation(des);
                }
            }
        }

        void Serialize(IntVec3 pos, SerializationInfo info, string prefix)
        {
            
        }

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
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //designations
            {
                info.AddValue("designations.amount", designations.Count.ToString());

                for (int i = 0; i < designations.Count; i++)
                {
                    info.AddValue("des.[" + i + "].def", designations[i].def.ToString());
                    Serialize(designations[i].target, info, "des.[" + i + "]");
                    /*
                    info.AddValue("des.[" + i + "].target.cell.x", designations[i].target.Cell.x.ToString());
                    info.AddValue("des.[" + i + "].target.cell.y", designations[i].target.Cell.y.ToString());
                    info.AddValue("des.[" + i + "].target.cell.z", designations[i].target.Cell.z.ToString());
                    info.AddValue("des.[" + i + "].target.thingId", designations[i].target.Thing.ThingID.ToString());*/
                }
            }

            //jobs
            {

                info.AddValue("jobDesc.amount", jobsToSerialize.Count);

                for (int i = 0; i < jobsToSerialize.Count; i++)
                {
                    {
                        info.AddValue("jobDesc.[" + i + "].pawnID", jobsToSerialize[i].tj.pawn.ThingID.ToString());
                        info.AddValue("jobDesc.[" + i + "].targetID", jobsToSerialize[i].tj.target.ThingID.ToString());
                        info.AddValue("jobDesc.[" + i + "].forced", jobsToSerialize[i].tj.forced.ToString());
                        info.AddValue("jobDesc.[" + i + "].cell.x", jobsToSerialize[i].cell.x.ToString());
                        info.AddValue("jobDesc.[" + i + "].cell.y", jobsToSerialize[i].cell.y.ToString());
                        info.AddValue("jobDesc.[" + i + "].cell.z", jobsToSerialize[i].cell.z.ToString());
                        info.AddValue("jobDesc.[" + i + "].jobDefName", jobsToSerialize[i].giver.def.workType.defName);
                        info.AddValue("jobDesc.[" + i + "].jobGiverClass", jobsToSerialize[i].giver.def.giverClass.ToString());
                    }
                    //info.AddValue("des.[" + i + "].playerForced", jobInfos[i].);
                    /*
                    info.AddValue("job.[" + i + "].jobdefname", jobInfos[i].jobDefName);
                    info.AddValue("job.[" + i + "].cell.x", jobInfos[i].cell.x);
                    info.AddValue("job.[" + i + "].cell.y", jobInfos[i].cell.y);
                    info.AddValue("job.[" + i + "].cell.z", jobInfos[i].cell.z);
                    info.AddValue("job.[" + i + "].pawnID", jobInfos[i].pawnID);*/
                }

            }
        }
    }
}
