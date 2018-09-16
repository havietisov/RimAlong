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
using System.Linq;

namespace CooperateRim
{
    [Serializable]
    public struct SRot4
    {
        public SRot4(Rot4 r)
        {
            intValue = r.AsInt;
        }

        public int intValue;

        public static implicit operator SRot4(Rot4 @this)
        {
            return new SRot4(@this);
        }

        public static implicit operator Rot4(SRot4 @this)
        {
            return new Rot4(@this.intValue);
        }
    }

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
            public string zoneType;
        }


        [Serializable]
        public class DesignatorCellCall
        {
            public SVEC3 cell;
            public string designatorType;
        }

        [Serializable]
        public class DesignatorMultiCellCall
        {
            public List<SVEC3> cells;
            public string designatorType;
        }

        [Serializable]
        public class DesignatorSingleCellCall
        {
            public SVEC3 cell;
            public string designatorType;
            public string thingDefName;
            public SRot4 rot;
            public string StuffDefName;
        }

        [Serializable]
        public class ForbiddenCallData
        {
            public string thingID;
            public bool value;
        }

        [Serializable]
        public class Designator_area_call_data
        {
            public string typeID;
            public SVEC3 cell;
        }

        List<TemporaryJobData> jobData = new List<TemporaryJobData>();

        List<S_Designation> designations = new List<S_Designation>();
        List<FinalJobData> jobsToSerialize = new List<FinalJobData>();
        List<JobPriorityData> jobPriorities = new List<JobPriorityData>();
        List<DesignatorCellCall> designatorCellCalls = new List<DesignatorCellCall>();
        List<DesignatorMultiCellCall> designatorMultiCellCalls = new List<DesignatorMultiCellCall>();
        List<DesignatorSingleCellCall> designatorSingleCellCalls = new List<DesignatorSingleCellCall>();
        List<ForbiddenCallData> ForbiddenCallDataCall = new List<ForbiddenCallData>();
        List<Designator_area_call_data> designatorAreaCallData = new List<Designator_area_call_data>();

        public static int clientCount = 2;
        public static string cliendID = "1";

        public static bool IsDeserializing;
        public static bool AvoidLoop;

        static SyncTickData singleton = new SyncTickData();

        public static void CompForbiddableSetForbiddenCall(string thingID, bool value)
        {
            singleton.ForbiddenCallDataCall.Add(new ForbiddenCallData() { thingID = thingID, value = value });
        }

        public static void AppendSyncTickData(Designation des)
        {
            singleton.designations.Add(des);
        }
        
        public static void AppendSyncTickData(TemporaryJobData j)
        {
            if (j.__result != null)
            {
                CooperateRimming.Log("new sync tick temporaty job defname : " + j.__result.def.defName);
                singleton.jobData.Add(j);
            }
            else
            {
                CooperateRimming.Log("temp job data is null");
            }
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
            /*if (new System.Collections.ICollection[]
            {
                singleton.jobsToSerialize,
                singleton.jobPriorities,
                singleton.designations,
                singleton.designatorCellCalls,
                singleton.designatorMultiCellCalls,
                singleton.ForbiddenCallDataCall
            }.Any(u => { return u.Count > 0; }))*/
            {
                try
                {
                    string s = @"D:\CoopReplays\_" + tickNum + "client_" + cliendID + ".xml";
                    
                    //CooperateRimming.Log("Written : " + s);
                    BinaryFormatter ser = new BinaryFormatter();
                    var fs = System.IO.File.OpenWrite(s);
                    SyncTickData buffered = singleton;
                    singleton = new SyncTickData();
                    ser.Serialize(fs, buffered);
                    fs.Flush();
                    fs.Close();
                    System.IO.File.WriteAllText(s + ".sync", "");
                }
                catch (Exception ee)
                {
                    CooperateRimming.Log(ee.ToString());
                }
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

        static void GetVal<T2>(ref List<T2> tVar, SerializationInfo info, string name)
        {
            tVar = (List<T2>)(info.GetValue(name, typeof(List<T2>)));
        }
        
        public SyncTickData(SerializationInfo info, StreamingContext ctx)
        {
            GetVal(ref designations, info, nameof(designations));
            GetVal(ref jobsToSerialize, info, nameof(jobsToSerialize));
            GetVal(ref jobPriorities, info, nameof(jobPriorities));
            GetVal(ref designatorCellCalls, info, nameof(designatorCellCalls));
            GetVal(ref designatorMultiCellCalls, info, nameof(designatorMultiCellCalls));
            GetVal(ref ForbiddenCallDataCall, info, nameof(ForbiddenCallDataCall));
            GetVal(ref designatorSingleCellCalls, info, nameof(designatorSingleCellCalls));
            GetVal(ref designatorAreaCallData, info, nameof(designatorAreaCallData));
            
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

                //CooperateRimming.Log(des.designationDef);
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

                //CooperateRimming.Log("Find.GameInitData : " + Find.GameInitData);
                //CooperateRimming.Log("pawn priority : " + pawn);
                AvoidLoop = true;
                pawn.workSettings.SetPriority(_wtd, prior.priority);
                AvoidLoop = false;
            }

            CooperateRimming.Log("Deserialized jobs :  " + jobsToSerialize.Count);

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

            foreach (var s in designatorCellCalls)
            {
                AvoidLoop = true;
                ((Designator)(typeof(DesignatorUtility).GetMethod(nameof(DesignatorUtility.FindAllowedDesignator)).MakeGenericMethod(Type.GetType(s.designatorType)).Invoke(null, null))).DesignateSingleCell(s.cell);
                //Find.ReverseDesignatorDatabase.AllDesignators.All( u => { CooperateRimming.Log(u.GetType().AssemblyQualifiedName + " == " + s.designatorType); return true; });
                //Find.ReverseDesignatorDatabase.AllDesignators.Find(u => u.GetType().AssemblyQualifiedName == s.designatorType).DesignateMultiCell(ConvertAll<SVEC3, IntVec3>(s.cells, u => (IntVec3)u));
                AvoidLoop = false;
            }

            foreach (DesignatorMultiCellCall s in designatorMultiCellCalls)
            {
                AvoidLoop = true;
                ((Designator)(typeof(DesignatorUtility).GetMethod(nameof(DesignatorUtility.FindAllowedDesignator)).MakeGenericMethod(Type.GetType(s.designatorType)).Invoke(null, null))).DesignateMultiCell(ConvertAll<SVEC3, IntVec3>(s.cells, u => (IntVec3)u));
                //Find.ReverseDesignatorDatabase.AllDesignators.All( u => { CooperateRimming.Log(u.GetType().AssemblyQualifiedName + " == " + s.designatorType); return true; });
                //Find.ReverseDesignatorDatabase.AllDesignators.Find(u => u.GetType().AssemblyQualifiedName == s.designatorType).DesignateMultiCell(ConvertAll<SVEC3, IntVec3>(s.cells, u => (IntVec3)u));
                AvoidLoop = false;
            }
            
            foreach (var s in designatorSingleCellCalls)
            {
                {
                    List<DesignationCategoryDef> allDefsListForReading = DefDatabase<DesignationCategoryDef>.AllDefsListForReading;
                    for (int i = 0; i < allDefsListForReading.Count; i++)
                    {
                        List<Designator> allResolvedDesignators = allDefsListForReading[i].AllResolvedDesignators;
                        for (int j = 0; j < allResolvedDesignators.Count; j++)
                        {
                            {
                                AvoidLoop = true;
                                Designator_Build dsf = allResolvedDesignators[j] as Designator_Build;

                                if (null != dsf)
                                {
                                    if (dsf.PlacingDef.defName == s.thingDefName)
                                    {
                                        dsf.GetType().GetField("placingRot", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(dsf, (Rot4)s.rot);
                                        dsf.SetStuffDef(DefDatabase<ThingDef>.AllDefs.First(u => u.defName == s.StuffDefName));
                                        dsf.DesignateSingleCell(s.cell);
                                    }
                                }

                                AvoidLoop = false;
                            }
                        }
                    }
                }
            }

            foreach (var s in ForbiddenCallDataCall)
            {
                Thing th = null;

                foreach (var thing in Find.CurrentMap.spawnedThings)
                {
                    if (thing.ThingID == s.thingID)
                    {
                        th = thing;
                        break;
                    }
                }

                AvoidLoop = true;
                //((ThingWithComps)(th)).GetComp<CompForbiddable>();
                ForbidUtility.SetForbidden(th, s.value, true);
                //Find.ReverseDesignatorDatabase.AllDesignators.Find(u => u.GetType().AssemblyQualifiedName == s.designatorType).DesignateThing(th);
                AvoidLoop = false;
            }
        }

        internal static void AllowJobAt(Job job, WorkGiver giver, IntVec3 cell)
        {
            TemporaryJobData _tj = singleton.jobData.Find(u => u != null && u.__result != null && u.__result.def.defName == job.def.defName);
            CooperateRimming.Log("gtype : " + giver.def.giverClass + ":" + job.def.defName);
            if (_tj != null)
            {
                CooperateRimming.Log("found temporary job to add : " + _tj.__result.def.defName);
                singleton.jobsToSerialize.Add(new FinalJobData() { cell = cell, giver = giver, forced = _tj.forced, pawn = _tj.pawn, target = _tj.target });
            }
        }

        public static IEnumerable<string> tickFileNames(int ticknum)
        {
            for (int i = 0; i < clientCount; i++)
            {
                yield return @"D:\CoopReplays\_" + ticknum + "client_" + i + ".xml";
            }
        }

        public static void Apply(int tickNum)
        {
            //CooperateRimming.Log("Applied frame " + tickNum);
            foreach (string s in tickFileNames(tickNum))
            {
                
                if (System.IO.File.Exists(s))
                {
                    Rand.PushState(0);
                    BinaryFormatter ser = new BinaryFormatter();
                    var fs = System.IO.File.OpenRead(s);
                    SyncTickData sd = ser.Deserialize(fs) as SyncTickData;

                    /*
                    foreach (Designation des in sd.designations)
                    {
                        AvoidLoop = true;
                        Find.CurrentMap.designationManager.AddDesignation(des);
                        AvoidLoop = false;
                    }*/
                    Rand.PopState();
                }
                
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
                info.AddValue(nameof(jobsToSerialize), jobsToSerialize);
            }

            //job priorities
            {
                info.AddValue("jobPriorities", jobPriorities);
            }

            //designator cell calls
            {
                info.AddValue("designatorCellCalls", designatorCellCalls);
            }

            //designator multicellcalls
            {
                info.AddValue(nameof(designatorMultiCellCalls), designatorMultiCellCalls);
            }

            //designator designate thing calls
            {
                info.AddValue(nameof(ForbiddenCallDataCall), ForbiddenCallDataCall);
            }

            //designator single cell for some types of jobs
            {
                info.AddValue(nameof(designatorSingleCellCalls), designatorSingleCellCalls);
            }
            //designator area (build roof) calls
            {
                info.AddValue(nameof(designatorAreaCallData), designatorAreaCallData);
            }
        }
        
        internal static void AppendSyncTickData(Designator instance, IntVec3 cell)
        {
            singleton.designatorCellCalls.Add(new DesignatorCellCall() { cell = cell, designatorType = instance.GetType().AssemblyQualifiedName });    
        }

        static IEnumerable<T2> ConvertAll<T1, T2>(IEnumerable<T1> @this, Func<T1, T2> converter)
        {
            foreach (T1 v in @this)
            {
                yield return converter(v);
            }
        }

        internal static void AppendSyncTickData(Designator instance, IEnumerable<IntVec3> cells)
        {
            List<SVEC3> ss = new List<SVEC3>();
            ss.AddRange(ConvertAll<IntVec3, SVEC3>(cells, u => (SVEC3)u));
            singleton.designatorMultiCellCalls.Add(new DesignatorMultiCellCall() { cells = ss, designatorType = instance.GetType().AssemblyQualifiedName });
        }

        internal static void AppendSyncTickDataDesignatorSingleCell(Designator instance, IntVec3 cell, BuildableDef bdef, Rot4 rot, ThingDef stuffDef)
        {
            singleton.designatorSingleCellCalls.Add(new DesignatorSingleCellCall() { cell = cell, designatorType = instance.GetType().AssemblyQualifiedName, thingDefName = bdef.defName, rot = rot, StuffDefName = stuffDef.defName });
        }

        internal static void AppendSyncTickDataArea(Designator_AreaBuildRoof instance, IntVec3 c)
        {
            singleton.designatorCellCalls.Add(new DesignatorCellCall { designatorType = instance.GetType().AssemblyQualifiedName, cell = c });
        }
    }
}
