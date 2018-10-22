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
using System.Reflection;
using System.IO;
using System.Xml.Linq;

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

        public override string ToString()
        {
            return "(" + x + ", " + y + ", " + z + ")";
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
            ThingID = t == null ? "<INVALID>" : t.ThingID;
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
            hasThing = t.HasThing;
        }

        public SVEC3 cell;
        public S_Thing thing;
        public bool hasThing;

        public static implicit operator S_LocalTargetInfo(LocalTargetInfo @this)
        {
            return @this != null ? new S_LocalTargetInfo(@this) : null;
        }
    }

    [Serializable]
    public class S_Designation
    {
        public S_Designation(Designation d, Type t)
        {
            designationDef = d.def.ToString();
            target = d.target;
            targetType = d.def.targetType;
            typeName = t.AssemblyQualifiedName;
        }

        public string designationDef;
        public string typeName;
        public S_LocalTargetInfo target;
        public TargetType targetType;
    }

    [Serializable]
    public class SyncTickData : ISerializable
    {
        public class TemporaryJobData
        {
            public S_Pawn pawn;
            public S_Thing targetThing;
            public S_LocalTargetInfo jobTargetA;
            public S_LocalTargetInfo jobTargetB;
            public S_LocalTargetInfo jobTargetC;
            public bool forced;
            public Job __result;
        }

        [Serializable]
        public class FinalJobData
        {
            //public TemporaryJobData tj;
            public SVEC3 cell;
            public LocomotionUrgency locomotionUrgency;
            public HaulMode haulMode;
            public S_Pawn pawn;
            public string jobDef;
            public S_LocalTargetInfo jobTargetA;
            public S_LocalTargetInfo jobTargetB;
            public S_LocalTargetInfo jobTargetC;
            public bool playerForced;
            public JobTag tag;
            public bool ignoreDesignations;
            public bool ignoreForbidden;
            public bool ignoreAssignment;
            public bool haulDroppedApparel;
            public bool restUntilHealed;
            public bool forceSleep;
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

        [Serializable]
        public class R_BILL
        {
            public string recipeDefName;
            public string billGiverName;
            public S_Thing targetThing;

            public static implicit operator R_BILL(Bill bill)
            {
                return new R_BILL() { recipeDefName = bill.recipe.defName, billGiverName = bill.billStack.billGiver.ToString(), targetThing = Find.Selector.SingleSelectedThing };
            }
        }

        [Serializable]
        public class BILL_REPEAT_CHANGES
        {
            public string modeDefName;
            public R_BILL owner;
        }

        [Serializable]
        public class ThingFilterSetAllowCall
        {
            public string thingDef;
            public bool val;
            public SVEC3 giverPos;
            public R_BILL bill;
        }
        
        [Serializable]
        public class DesignatorHuntThing
        {
            public string designatorType;
            public S_Thing thing;
            public SVEC3 pos;
        }

        [Serializable]
        public class DesignatorApplyToThing
        {
            public string designatorType;
            public S_Thing thing;
            public SVEC3 pos;
        }

        [Serializable]
        public class TRAP_AUTO_REARM_SET
        {
            public S_Thing trap;
            public bool val;
        }

        [Serializable]
        public class SyncThingFieldCommand
        {
            public S_Thing thing;
            public string typename;
            public string fieldname;
            public BindingFlags fieldFlags;
            public object value;
        }


        [Serializable]
        public class PawnDrafted
        {
            public S_Thing pawn;
            public bool value;
        }

        [Serializable]
        public class COMMAND_TOGGLE_INDEXED_CALLS
        {
            public S_Thing thing;
            public SVEC3 location;
            public int gizmo_index;
        }

        [Serializable]
        public class Zone_set_plant
        {
            public string plantDef;
            public SerializableZoneData zoneData;
        }

        [Serializable]
        public class ThingFilterSetWorkerCall
        {
            public S_Thing worker;
            public SVEC3 giverPos;
            public R_BILL bill;
        }

        [Serializable]
        public class IndexedBillConfigRestrictionOptionCall
        {
            public S_Thing pawn;
            public SVEC3 giverPos;
            public R_BILL bill;
            public int restrictOptionIndex;
        }

        [Serializable]
        public class ThingFilterDelta
        {
            public string thingDefName;
            public bool value;
            public S_Thing context;
            public int[] zone;
        };

        [Serializable]
        public class ThingFilterDeltaSpecial
        {
            public string specialThingDefName;
            public bool value;
            public S_Thing context;
            public int[] zone;
        };

        List<TemporaryJobData> jobData = new List<TemporaryJobData>();

        List<S_Designation> designations = new List<S_Designation>();
        List<FinalJobData> jobsToSerialize = new List<FinalJobData>();
        List<FinalJobData> jobs2toSerialize = new List<FinalJobData>();
        List<JobPriorityData> jobPriorities = new List<JobPriorityData>();
        List<DesignatorCellCall> designatorCellCalls = new List<DesignatorCellCall>();
        List<DesignatorMultiCellCall> designatorMultiCellCalls = new List<DesignatorMultiCellCall>();
        List<DesignatorSingleCellCall> designatorSingleCellCalls = new List<DesignatorSingleCellCall>();
        List<ForbiddenCallData> ForbiddenCallDataCall = new List<ForbiddenCallData>();
        List<Designator_area_call_data> designatorAreaCallData = new List<Designator_area_call_data>();
        List<R_BILL> bills = new List<R_BILL>();
        List<BILL_REPEAT_CHANGES> bill_repeat_commands = new List<BILL_REPEAT_CHANGES>();
        List<ThingFilterSetAllowCall> thingFilterSetAllowCalls = new List<ThingFilterSetAllowCall>();
        List<ThingFilterSetWorkerCall> thingFilterSetWorkerCalls = new List<ThingFilterSetWorkerCall>();
        List<DesignatorHuntThing> designatorHuntThingCalls = new List<DesignatorHuntThing>();
        List<DesignatorApplyToThing> designatorApplyToThingCalls = new List<DesignatorApplyToThing>();
        List<TRAP_AUTO_REARM_SET> trapAutoRearms = new List<TRAP_AUTO_REARM_SET>();
        List<SyncThingFieldCommand> syncFieldCommands = new List<SyncThingFieldCommand>();
        List<PawnDrafted> pawnDrafts = new List<PawnDrafted>();
        List<COMMAND_TOGGLE_INDEXED_CALLS> toggleCommandIndexedCalls = new List<COMMAND_TOGGLE_INDEXED_CALLS>();
        List<Zone_set_plant> setZonePlants = new List<Zone_set_plant>();
        List<SerializableZoneData> deleteZones = new List<SerializableZoneData>();
        List<IndexedBillConfigRestrictionOptionCall> indexedBillConfigRestrictionOptionCalls = new List<IndexedBillConfigRestrictionOptionCall>();
        List<ThingFilterDelta> thingFiltersData = new List<ThingFilterDelta>();
        List<ThingFilterDeltaSpecial> thingFiltersDataSpecial = new List<ThingFilterDeltaSpecial>();

        class DirtyThingFilter
        {
            public ThingFilter filter;
            public SVEC3 giverPos;
            public Bill bill;
        }

        public void DebugLog()
        {
            NetDemo.log(nameof(toggleCommandIndexedCalls) + "::" + toggleCommandIndexedCalls.Count);
            //CooperateRimming.Log();
        }

        List<string> researches = new List<string>();

        public static int clientCount = 1;
        public static int cliendID = -1;

        public static void SetClientID(int id)
        {
            CooperateRimming.Log("RECEIVED CLIENT ID = " + id);
            cliendID = id;
        }

        public static bool IsDeserializing;
        public static bool AvoidLoop;

        static SyncTickData singleton = new SyncTickData();

        public static void CompForbiddableSetForbiddenCall(string thingID, bool value)
        {
            singleton.ForbiddenCallDataCall.Add(new ForbiddenCallData() { thingID = thingID, value = value });
        }

        public static void AppendSyncTickDesignatorApplyToThing(Designator d, Thing t, IntVec3 pos)
        {
            singleton.designatorApplyToThingCalls.Add(new DesignatorApplyToThing() { designatorType = d.GetType().AssemblyQualifiedName, thing = t, pos = pos });
        }

        public static void AppendSyncTickDesignatePrey(Designator_Hunt d, Thing t, IntVec3 pos)
        {
            CooperateRimming.Log(t.PositionHeld + " |||" + pos);
            //singleton.designatorHuntThingCalls.Add(new DesignatorHuntThing() { designatorType = d.GetType().AssemblyQualifiedName, thing = t, pos = pos });
        }

        public static void AppendSyncFieldForThingCommand(Thing t, FieldInfo fi, BindingFlags flag, object value)
        {
            singleton.syncFieldCommands.Add(new SyncThingFieldCommand() { fieldFlags = flag, fieldname = fi.Name, typename = t.GetType().AssemblyQualifiedName, thing = t, value = value });
        }

        public static void AppendSyncTickData(Building_Trap instt, bool val)
        {
            singleton.trapAutoRearms.Add(new TRAP_AUTO_REARM_SET() { trap = instt, val = val });
        }

        public static void AppendSyncTickData(Zone_Growing __instance, ThingDef plantDef)
        {
            singleton.setZonePlants.Add(new Zone_set_plant() { plantDef = plantDef.defName, zoneData = new SerializableZoneData() { zoneID = __instance.ID } });
        }

        public static void AppendSyncTickData(Bill b, IBillGiver giver, BillRepeatModeDef def)
        {
            R_BILL bb = new R_BILL() { recipeDefName = b.recipe.defName, billGiverName = giver.ToString(), targetThing = Find.Selector.SingleSelectedThing };
            singleton.bill_repeat_commands.Add(new BILL_REPEAT_CHANGES() { modeDefName = def.defName, owner = bb });
        }

        public static void AppendSyncTickData(Bill b, IBillGiver giver)
        {
            singleton.bills.Add(new R_BILL() { recipeDefName = b.recipe.defName, billGiverName = giver.ToString(), targetThing = Find.Selector.SingleSelectedThing });
        }

        public static void AppendSyncTickData(Designation des, System.Type designator)
        {
            CooperateRimming.Log("XXXXXXXXXXXXXx ++ " + des.def.defName);
            singleton.designations.Add(new S_Designation(des, designator));
        }

        public static void AppendSyncTickData(ResearchProjectDef research)
        {

            singleton.researches.Add(research.defName);
        }

        public static void AppendSyncTickData(TemporaryJobData j)
        {
            if (j.__result != null)
            {
                //CooperateRimming.Log("new sync tick temporaty job defname : " + j.__result.def.defName);
                singleton.jobData.Add(j);
            }
            else
            {
                //CooperateRimming.Log("temp job data is null");
            }
        }

        public static void AppendSyncTickData(WorkTypeDef w, int priority, Pawn p)
        {
            singleton.jobPriorities.Add(new JobPriorityData() { w = w, priority = priority, p = p });
        }

        public SyncTickData()
        {

        }

        public static bool FlushSyncTickData(int tickNum)
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
#if FILE_TRANSFER
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
#else

                    if (cliendID > -1)
                    {
                        BinaryFormatter ser = new BinaryFormatter();
                        SyncTickData buffered = singleton;
                        MemoryStream fs = new MemoryStream();
                        singleton = new SyncTickData();
                        CooperateRimming.Log("sending data for tick " + tickNum);
                        NetDemo.PushStateToDirectory(cliendID, tickNum, buffered, 0);
                        fs.Close();
                        return true;
                    }
                    return false;
#endif
                }
                catch (Exception ee)
                {
                    CooperateRimming.Log(ee.ToString());
                    return false;
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

            CooperateRimming.Log("could not locate designation def : " + name);
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
            GetVal(ref researches, info, nameof(researches));
            GetVal(ref bills, info, nameof(bills));
            GetVal(ref bill_repeat_commands, info, nameof(bill_repeat_commands));
            GetVal(ref thingFilterSetAllowCalls, info, nameof(thingFilterSetAllowCalls));
            GetVal(ref designatorApplyToThingCalls, info, nameof(designatorApplyToThingCalls));
            GetVal(ref trapAutoRearms, info, nameof(trapAutoRearms));
            GetVal(ref syncFieldCommands, info, nameof(syncFieldCommands));
            GetVal(ref pawnDrafts, info, nameof(pawnDrafts));
            GetVal(ref toggleCommandIndexedCalls, info, nameof(toggleCommandIndexedCalls));
            GetVal(ref setZonePlants, info, nameof(setZonePlants));
            GetVal(ref deleteZones, info, nameof(deleteZones));
            GetVal(ref thingFilterSetWorkerCalls, info, nameof(thingFilterSetWorkerCalls));
            GetVal(ref indexedBillConfigRestrictionOptionCalls, info, nameof(indexedBillConfigRestrictionOptionCalls));
            GetVal(ref thingFiltersData, info, nameof(thingFiltersData));
            GetVal(ref thingFiltersDataSpecial, info, nameof(thingFiltersDataSpecial));
        }

        public void AcceptResult()
        {
            int lockD = 0;

            try
            {

                //CooperateRimming.Log("deserialized designations : " + designations.Count);
                lockD = 1;
                foreach (var des in designations)
                {
                    Thing sdt = null;


                    AvoidLoop = true;
                    switch (des.targetType)
                    {
                        case TargetType.Cell:
                            {
                                var ddee = ((Designator)(typeof(DesignatorUtility).GetMethod(nameof(DesignatorUtility.FindAllowedDesignator)).MakeGenericMethod(Type.GetType(des.typeName)).Invoke(null, null)));

                                CooperateRimming.Log("dess : " + ddee);
                                var ddes = Find.CurrentMap.designationManager.DesignationAt(des.target.cell, DefFromString(des.designationDef));

                                foreach (var v in Find.CurrentMap.designationManager.AllDesignationsAt(des.target.cell))
                                {
                                    CooperateRimming.Log(v.ToString());
                                }
                                //this should be replaced with DesignateSingleCell for mining case!
                                Find.CurrentMap.designationManager.AddDesignation(new Designation((IntVec3)des.target.cell, DefFromString(des.designationDef)));
                            }
                            break;

                        case TargetType.Thing:
                            {
                                foreach (Thing t in Find.CurrentMap.thingGrid.ThingsAt(des.target.cell))
                                {
                                    if (t.ThingID == des.target.thing.ThingID)
                                    {
                                        sdt = t;
                                    }
                                }

                                Find.CurrentMap.designationManager.AddDesignation(new Designation(new LocalTargetInfo(sdt), DefFromString(des.designationDef)));
                            }
                            break;
                    }

                    AvoidLoop = false;
                    CooperateRimming.Log(des.designationDef);
                }

                lockD = 2;
                foreach (var prior in jobPriorities)
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

                //CooperateRimming.Log("Deserialized jobs :  " + jobsToSerialize.Count);

                List<Thing>[] things = (List<Thing>[])Find.CurrentMap.thingGrid.GetType().GetField("thingGrid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(Find.CurrentMap.thingGrid);

                //CooperateRimming.Log("thinglist : " + things);

                /*
                foreach (var trapI in trapAutoRearms)
                {
                    Thing trap = things.Where(u => u.Count != 0).First(u => u.Any(uu => uu.ThingID == trapI.trap.ThingID)).First(u => u.ThingID == trapI.trap.ThingID);

                    CooperateRimming.Log("Trap ID : " + trap);

                    if (trap != null && trap is Building_Trap)
                    {
                        AvoidLoop = true;
                        BuildingTrapPatch.SetAutoRearmValue(trap as Building_Trap, trapI.val);
                        AvoidLoop = false;
                    }
                }*/

                lockD = 3;
                foreach (var zi in deleteZones)
                {
                    AvoidLoop = true;
                    Find.CurrentMap.zoneManager.AllZones.First(u => u.ID == zi.zoneID).Delete();
                    AvoidLoop = false;
                }

                lockD = 4;
                foreach (var fieldInfo in syncFieldCommands)
                {
                    Thing t = things.Where(u => u.Count != 0).First(u => u.Any(uu => uu.ThingID == fieldInfo.thing.ThingID)).First(u => u.ThingID == fieldInfo.thing.ThingID);

                    if (t != null)
                    {
                        AvoidLoop = true;
                        t.GetType().GetField(fieldInfo.fieldname, fieldInfo.fieldFlags).SetValue(t, fieldInfo.value);
                        AvoidLoop = false;
                    }
                }

                lockD = 5;
                foreach (var draftInfo in pawnDrafts)
                {
                    Thing t = things.Where(u => u.Count != 0).First(u => u.Any(uu => uu.ThingID == draftInfo.pawn.ThingID)).First(u => u.ThingID == draftInfo.pawn.ThingID);

                    if (t != null && t is Pawn)
                    {
                        AvoidLoop = true;
                        (t as Pawn).drafter.Drafted = draftInfo.value;
                        AvoidLoop = false;
                    }
                }

                lockD = 6;
                foreach (var _bill in bills)
                {
                    Thing issuer = things.Where(u => u.Count != 0).First(u => u.Any(uu => uu.ThingID == _bill.targetThing.ThingID)).First(u => u.ThingID == _bill.targetThing.ThingID);

                    CooperateRimming.Log("job issuer : " + (issuer == null ? "null" : issuer.ToString()));

                    foreach (var rec in issuer.def.AllRecipes)
                    {
                        if (rec.defName == _bill.recipeDefName)
                        {
                            AvoidLoop = true;
                            (issuer as IBillGiver).BillStack.AddBill(BillUtility.MakeNewBill(rec));
                            AvoidLoop = false;
                            break;
                        }
                    }
                }

                lockD = 7;
                foreach (var comm in bill_repeat_commands)
                {
                    var _bill = comm.owner;
                    Thing issuer = things.Where(u => u.Count != 0).First(u => u.Any(uu => uu.ThingID == _bill.targetThing.ThingID)).First(u => u.ThingID == _bill.targetThing.ThingID);

                    CooperateRimming.Log("job issuer : " + (issuer == null ? "null" : issuer.ToString()));

                    foreach (var rec in issuer.def.AllRecipes)
                    {
                        if (rec.defName == _bill.recipeDefName)
                        {
                            AvoidLoop = true;
                            foreach (var ___bill in (issuer as IBillGiver).BillStack.Bills)
                            {
                                if (___bill is Bill_Production && ___bill.recipe.defName == _bill.recipeDefName)
                                {
                                    (___bill as Bill_Production).repeatMode = (BillRepeatModeDef)typeof(BillRepeatModeDefOf).GetFields().First(u => (u.GetValue(null) as BillRepeatModeDef).defName == comm.modeDefName).GetValue(null);
                                }
                            }
                            AvoidLoop = false;
                            break;
                        }
                    }
                }

                lockD = 8;
                foreach (var a in thingFilterSetAllowCalls)
                {
                    var _bill = a.bill;
                    Thing issuer = Find.CurrentMap.thingGrid.ThingsListAt(a.giverPos).First(u => u.ThingID == _bill.targetThing.ThingID);
                    CooperateRimming.Log("thing filter issuer : " + (issuer == null ? "null" : issuer.ToString()));

                    foreach (var rec in issuer.def.AllRecipes)
                    {
                        if (rec.defName == _bill.recipeDefName)
                        {
                            AvoidLoop = true;
                            foreach (var ___bill in (issuer as IBillGiver).BillStack.Bills)
                            {
                                if (___bill is Bill_Production && ___bill.recipe.defName == _bill.recipeDefName)
                                {
                                    (___bill as Bill_Production).ingredientFilter.SetAllow(DefDatabase<ThingDef>.AllDefsListForReading.First(u => u.defName == a.thingDef), a.val);
                                }
                            }
                            AvoidLoop = false;
                            break;
                        }
                    }
                }

                lockD = 9;
                foreach (var _job in jobsToSerialize)
                {
                    Pawn pawn = null;
                    JobDef workTypeDef = null;

                    foreach (var _pawn in Find.CurrentMap.mapPawns.AllPawns)
                    {
                        if (_pawn.ThingID == _job.pawn.ThingID)
                        {
                            pawn = _pawn;
                            break;
                        }
                    }

                    foreach (var __workTypeDef in typeof(JobDefOf).GetFields())
                    {
                        CooperateRimming.Log("[" + (__workTypeDef.GetValue(null) as JobDef).defName + " : " + _job.jobDef + "]");

                        if ((__workTypeDef.GetValue(null) as JobDef).defName == _job.jobDef)
                        {
                            workTypeDef = (__workTypeDef.GetValue(null) as JobDef);
                            break;
                        }
                    }

                    CooperateRimming.Log(">>>>>>>>>>>>>");
                    CooperateRimming.Log("workTypeDef " + (workTypeDef == null ? "null" : workTypeDef.ToString()));
                    CooperateRimming.Log("pawn :" + pawn + "]");

                    Job job = new Job(workTypeDef);

                    try
                    {
                        if (_job.jobTargetA != null)
                        {
                            if (_job.jobTargetA.hasThing)
                            {
                                job.targetA = things.Where(u => u.Count != 0).First(u => u.Any(uu => uu.ThingID == _job.jobTargetA.thing.ThingID)).First(u => u.ThingID == _job.jobTargetA.thing.ThingID);
                            }
                            else
                            {
                                job.targetA = (IntVec3)_job.jobTargetA.cell;
                            }
                        }
                    }
                    catch (Exception ee)
                    {

                    }

                    try
                    {
                        if (_job.jobTargetB != null)
                        {
                            if (_job.jobTargetA.hasThing)
                            {
                                job.targetB = things.Where(u => u.Count != 0).First(u => u.Any(uu => uu.ThingID == _job.jobTargetB.thing.ThingID)).First(u => u.ThingID == _job.jobTargetB.thing.ThingID);
                            }
                            else
                            {
                                job.targetB = (IntVec3)_job.jobTargetB.cell;
                            }
                        }
                    }
                    catch (Exception ee)
                    {

                    }

                    try
                    {
                        if (_job.jobTargetC != null)
                        {
                            if (_job.jobTargetA.hasThing)
                            {
                                job.targetC = things.Where(u => u.Count != 0).First(u => u.Any(uu => uu.ThingID == _job.jobTargetC.thing.ThingID)).First(u => u.ThingID == _job.jobTargetC.thing.ThingID);
                            }
                            else
                            {
                                job.targetC = (IntVec3)_job.jobTargetC.cell;
                            }
                        }
                    }
                    catch (Exception ee)
                    {

                    }

                    SyncTickData.AvoidLoop = true;
                    pawn.jobs.TryTakeOrderedJob(job, _job.tag);
                    SyncTickData.AvoidLoop = false;
                }

                lockD = 10;
                foreach (var s in designatorCellCalls)
                {
                    AvoidLoop = true;
                    CooperateRimming.Log(s.designatorType);
                    ((Designator)(typeof(DesignatorUtility).GetMethod(nameof(DesignatorUtility.FindAllowedDesignator)).MakeGenericMethod(Type.GetType(s.designatorType)).Invoke(null, null))).DesignateSingleCell(s.cell);
                    //Find.ReverseDesignatorDatabase.AllDesignators.All( u => { CooperateRimming.Log(u.GetType().AssemblyQualifiedName + " == " + s.designatorType); return true; });
                    //Find.ReverseDesignatorDatabase.AllDesignators.Find(u => u.GetType().AssemblyQualifiedName == s.designatorType).DesignateMultiCell(ConvertAll<SVEC3, IntVec3>(s.cells, u => (IntVec3)u));
                    AvoidLoop = false;
                }

                lockD = 11;
                foreach (DesignatorMultiCellCall s in designatorMultiCellCalls)
                {
                    AvoidLoop = true;
                    ((Designator)(typeof(DesignatorUtility).GetMethod(nameof(DesignatorUtility.FindAllowedDesignator)).MakeGenericMethod(Type.GetType(s.designatorType)).Invoke(null, null))).DesignateMultiCell(ConvertAll<SVEC3, IntVec3>(s.cells, u => (IntVec3)u));
                    //Find.ReverseDesignatorDatabase.AllDesignators.All( u => { CooperateRimming.Log(u.GetType().AssemblyQualifiedName + " == " + s.designatorType); return true; });
                    //Find.ReverseDesignatorDatabase.AllDesignators.Find(u => u.GetType().AssemblyQualifiedName == s.designatorType).DesignateMultiCell(ConvertAll<SVEC3, IntVec3>(s.cells, u => (IntVec3)u));
                    AvoidLoop = false;
                }

                lockD = 12;
                foreach (DesignatorApplyToThing s in designatorApplyToThingCalls)
                {
                    AvoidLoop = true;
                    ((Designator)(typeof(DesignatorUtility).GetMethod(nameof(DesignatorUtility.FindAllowedDesignator)).MakeGenericMethod(Type.GetType(s.designatorType)).Invoke(null, null))).DesignateThing(Find.CurrentMap.thingGrid.ThingsAt(s.pos).First(u => u.ThingID == s.thing.ThingID));
                    //Find.ReverseDesignatorDatabase.AllDesignators.All( u => { CooperateRimming.Log(u.GetType().AssemblyQualifiedName + " == " + s.designatorType); return true; });
                    //Find.ReverseDesignatorDatabase.AllDesignators.Find(u => u.GetType().AssemblyQualifiedName == s.designatorType).DesignateMultiCell(ConvertAll<SVEC3, IntVec3>(s.cells, u => (IntVec3)u));
                    AvoidLoop = false;
                }

                lockD = 13;
                foreach (COMMAND_TOGGLE_INDEXED_CALLS call in toggleCommandIndexedCalls)
                {
                    AvoidLoop = true;
                    Thing issuer = things.Where(u => u.Count != 0).First(u => u.Any(uu => uu.ThingID == call.thing.ThingID)).First(u => u.ThingID == call.thing.ThingID);

                    //Thing issuer = Find.CurrentMap.thingGrid.ThingsListAt(call.location).First(u => u.ThingID == call.thing.ThingID);

                    IEnumerator<Gizmo> gizmoI = issuer.GetGizmos().GetEnumerator();
                    CooperateRimming.Log("COMMAND_TOGGLE_INDEXED_CALLS::index  = " + call.gizmo_index);

                    //if (call.location != issuer.Position)
                    {
                        CooperateRimming.Log("positions for COMMAND_TOGGLE_INDEXED_CALLS location:" + call.location + " |position| " + issuer.Position + " |position held| " + issuer.PositionHeld);
                    }

                    //CooperateRimming.Log("COMMAND_TOGGLE_INDEXED_CALLS::Position " + );
                    for (int i = 0; i <= call.gizmo_index; i++)
                    {
                        if (!gizmoI.MoveNext())
                        {
                            break;
                        }
                        else
                        {
                            CooperateRimming.Log("COMMAND_TOGGLE_INDEXED_CALLS::cycle  = " + i);
                            if (i == call.gizmo_index)
                            {
                                if (gizmoI.Current is Command_Toggle)
                                {
                                    (gizmoI.Current as Command_Toggle).toggleAction();
                                }

                                if (gizmoI.Current is Command_Action)
                                {
                                    (gizmoI.Current as Command_Action).action();
                                }
                            }
                        }
                    }

                    AvoidLoop = false;
                }

                lockD = 14;
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
                                            if (s.StuffDefName != null)
                                            {
                                                dsf.SetStuffDef(DefDatabase<ThingDef>.AllDefs.First(u => u.defName == s.StuffDefName));
                                            }
                                            dsf.DesignateSingleCell(s.cell);
                                        }
                                    }

                                    AvoidLoop = false;
                                }
                            }
                        }
                    }
                }

                lockD = 15;
                foreach (var s in setZonePlants)
                {
                    SyncTickData.AvoidLoop = true;

                    foreach (var z in Find.CurrentMap.zoneManager.AllZones)
                    {
                        CooperateRimming.Log(z.ID + " ++ " + s.zoneData.zoneID);

                        if (z.ID == s.zoneData.zoneID)
                        {
                            if (z != null && z is Zone_Growing)
                            {
                                (z as Zone_Growing).SetPlantDefToGrow(DefDatabase<ThingDef>.AllDefs.First(u => u.defName == s.plantDef));
                            }
                            else
                            {
                                if (z == null)
                                {
                                    CooperateRimming.Log("fukken zero!");
                                }
                                else
                                {
                                    CooperateRimming.Log("z is not growing");
                                }
                            }
                        }
                    }

                    AvoidLoop = false;
                }

                lockD = 16;
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

                lockD = 17;
                foreach (string s in researches)
                {
                    TickManagerPatch.cachedRDef = DefDatabase<ResearchProjectDef>.AllDefsListForReading.Find(u => u.defName == s);
                    Find.ResearchManager.currentProj = TickManagerPatch.cachedRDef;
                }

                lockD = 18;
                {
                    foreach (IndexedBillConfigRestrictionOptionCall call in indexedBillConfigRestrictionOptionCalls)
                    {
                        AvoidLoop = true;
                        Thing issuer = Find.CurrentMap.thingGrid.ThingsListAt(call.giverPos).First(u => u.ThingID == call.bill.targetThing.ThingID);

                        CooperateRimming.Log("IndexedBillConfigRestrictionOptionCall::index " + call.restrictOptionIndex);

                        foreach (var rec in issuer.def.AllRecipes)
                        {
                            CooperateRimming.Log("IndexedBillConfigRestrictionOptionCall::recipe " + rec.defName + " == " + call.bill.recipeDefName);

                            if (rec.defName == call.bill.recipeDefName)
                            {
                                AvoidLoop = true;
                                foreach (var ___bill in (issuer as IBillGiver).BillStack.Bills)
                                {
                                    CooperateRimming.Log("IndexedBillConfigRestrictionOptionCall::bill " + ___bill.recipe.defName);

                                    if (___bill is Bill_Production && ___bill.recipe.defName == call.bill.recipeDefName)
                                    {
                                        Dialog_BillConfig dialog = new Dialog_BillConfig(___bill as Bill_Production, call.giverPos);
                                        CooperateRimming.Log("IndexedBillConfigRestrictionOptionCall::option <-1>");
                                        int index = 0;
                                        foreach (var element in (IEnumerable < Widgets.DropdownMenuElement<Pawn>>)dialog.GetType().GetMethod("GeneratePawnRestrictionOptions", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(dialog, new object[] { }))
                                        {
                                            CooperateRimming.Log("IndexedBillConfigRestrictionOptionCall::option " + index);

                                            if (index == call.restrictOptionIndex)
                                            {
                                                ___bill.pawnRestriction = element.payload;
                                                CooperateRimming.Log(">>>>>>>>>>>>> element called!");
                                            }

                                            index++;
                                        }
                                        CooperateRimming.Log("IndexedBillConfigRestrictionOptionCall::option <~~~>");
                                    }
                                }
                            }
                        }

                        AvoidLoop = false;
                    }
                }
                lockD = 19;

                foreach (var filterData in thingFiltersData)
                {
                    AvoidLoop = true;
                    IStoreSettingsParent storeSettings = null;
                    if (filterData.zone != null)
                    {
                        Zone z = Find.CurrentMap.zoneManager.AllZones.First(u => u.ID == filterData.zone[0]);
                        if (z as IStoreSettingsParent != null)
                        {
                            storeSettings = z as IStoreSettingsParent;
                        }
                    }

                    if (storeSettings == null)
                    {
                        Thing issuer = things.Where(u => u.Count != 0).First(u => u.Any(uu => uu.ThingID == filterData.context.ThingID)).First(u => u.ThingID == filterData.context.ThingID);
                        storeSettings = issuer as IStoreSettingsParent;
                    }

                    if (storeSettings != null)
                    {
                        storeSettings.GetStoreSettings().filter.SetAllow(DefDatabase<ThingDef>.GetNamed(filterData.thingDefName, true), filterData.value);
                    }

                    AvoidLoop = false;
                }

                lockD = 20;

                foreach (var filterData in thingFiltersDataSpecial)
                {
                    AvoidLoop = true;
                    IStoreSettingsParent storeSettings = null;
                    if (filterData.zone != null)
                    {
                        Zone z = Find.CurrentMap.zoneManager.AllZones.First(u => u.ID == filterData.zone[0]);
                        if (z as IStoreSettingsParent != null)
                        {
                            storeSettings = z as IStoreSettingsParent;
                        }
                    }

                    if (storeSettings == null)
                    {
                        Thing issuer = things.Where(u => u.Count != 0).First(u => u.Any(uu => uu.ThingID == filterData.context.ThingID)).First(u => u.ThingID == filterData.context.ThingID);
                        storeSettings = issuer as IStoreSettingsParent;
                    }

                    if (storeSettings != null)
                    {
                        storeSettings.GetStoreSettings().filter.SetAllow(DefDatabase<SpecialThingFilterDef>.GetNamed(filterData.specialThingDefName, true), filterData.value);
                    }

                    AvoidLoop = false;
                }

                lockD = 21;
                Bill_production_patch.kkk.Clear();
            }
            catch (Exception ee)
            {
                CooperateRimming.Log("sync tick data exception at stage " + lockD + "\r\n" + ee.ToString());
            }

            AvoidLoop = false;
        }

        internal static void AllowJobAt(Job job, Pawn pawn, JobTag tag, IntVec3 cell)
        {
            string s = "";

            foreach (var ss in typeof(Job).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                s += "[" + ss.Name + " :: " + ss.GetValue(job) + "]\r\n";
            }
            CooperateRimming.Log(s);
            //CooperateRimming.Log(job.ToString() + " |1| " + job.playerForced + " |2| " + job.forceSleep + " |3| " + job.restUntilHealed + " |4| " + job.haulDroppedApparel + " |5| " + job.ignoreDesignations + " |6| " + job.ignoreJoyTimeAssignment + " |7| " + job.ignoreForbidden + " |8| " + job.locomotionUrgency + " |9| " + job.haulMode + " |10| " + cell + " |11| " + job.def.defName + " |12| " + pawn + " |13| " + job.targetA + " |14| " + job.targetB + " |15| " + job.targetC);
            {
                singleton.jobsToSerialize.Add(new FinalJobData() { playerForced = job.playerForced, forceSleep = job.forceSleep, restUntilHealed = job.restUntilHealed, haulDroppedApparel = job.haulDroppedApparel, ignoreDesignations = job.ignoreDesignations, ignoreAssignment = job.ignoreJoyTimeAssignment, ignoreForbidden = job.ignoreForbidden, locomotionUrgency = job.locomotionUrgency, haulMode = job.haulMode, cell = cell, jobDef = job.def.defName, pawn = pawn, jobTargetA = job.targetA, jobTargetB = job.targetB, jobTargetC = job.targetC });
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
#if FILE_TRANSFER
            foreach (string s in tickFileNames(tickNum))
            {
                
                if (System.IO.File.Exists(s))
                {
                    //Rand.pushstate(0);
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
                    //Rand.PopState();
                }
                
            }
    
#else

#endif
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
                info.AddValue(nameof(designations), designations);
            }

            //jobs
            {
                foreach (var j in jobsToSerialize)
                {
                    CooperateRimming.Log("++++ : " + j.jobDef);
                }
                info.AddValue(nameof(jobsToSerialize), jobsToSerialize);
            }

            //job priorities
            {
                info.AddValue(nameof(jobPriorities), jobPriorities);
            }

            //designator cell calls
            {
                info.AddValue(nameof(designatorCellCalls), designatorCellCalls);
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

            //researches
            {
                info.AddValue(nameof(researches), researches);
            }

            //bills
            {
                info.AddValue(nameof(bills), bills);
            }

            //bill repeat modes
            {
                info.AddValue(nameof(bill_repeat_commands), bill_repeat_commands);
            }

            //thingfilter commands
            {
                info.AddValue(nameof(thingFilterSetAllowCalls), thingFilterSetAllowCalls);
            }

            //designator apply thing calls
            {
                info.AddValue(nameof(designatorApplyToThingCalls), designatorApplyToThingCalls);
            }

            //trap auto rearm value 
            {
                info.AddValue(nameof(trapAutoRearms), trapAutoRearms);
            }

            //direct field sync
            {
                info.AddValue(nameof(syncFieldCommands), syncFieldCommands);
            }

            //pawn draft sync
            {
                info.AddValue(nameof(pawnDrafts), pawnDrafts);
            }

            //command toggle sync
            {
                info.AddValue(nameof(toggleCommandIndexedCalls), toggleCommandIndexedCalls);
            }

            //set zone plants 
            {
                info.AddValue(nameof(setZonePlants), setZonePlants);
            }

            //deleted zones
            {
                info.AddValue(nameof(deleteZones), deleteZones);
            }

            //set worker calls
            {
                info.AddValue(nameof(thingFilterSetWorkerCalls), thingFilterSetWorkerCalls);
            }

            //indexed bill config calls
            {
                info.AddValue(nameof(indexedBillConfigRestrictionOptionCalls), indexedBillConfigRestrictionOptionCalls);
            }

            //thingfilters to sync
            {
                info.AddValue(nameof(thingFiltersData), thingFiltersData);
            }

            //special thingfilters to sync
            {
                info.AddValue(nameof(thingFiltersDataSpecial), thingFiltersDataSpecial);
            }
        }

        public static void AppendSyncTickDataCommand_toggle_call_by_index(Thing t, int number)
        {
            CooperateRimming.Log(t + " || " + number);
            singleton.toggleCommandIndexedCalls.Add(new COMMAND_TOGGLE_INDEXED_CALLS() { thing = t, location = t.Position, gizmo_index = number });
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
            singleton.designatorSingleCellCalls.Add(new DesignatorSingleCellCall() { cell = cell, designatorType = instance.GetType().AssemblyQualifiedName, thingDefName = bdef.defName, rot = rot, StuffDefName = stuffDef == null ? null : stuffDef.defName });
        }

        internal static void AppendSyncTickDataArea(Designator_Area instance, IntVec3 c)
        {
            singleton.designatorCellCalls.Add(new DesignatorCellCall { designatorType = instance.GetType().AssemblyQualifiedName, cell = c });
        }

        public static void AppendSyncTickDataThingFilterSetAllow(ThingDef thingDef, bool allow, IntVec3 pos, Bill_Production bill)
        {
            singleton.thingFilterSetAllowCalls.Add(new ThingFilterSetAllowCall() { thingDef = thingDef.defName, val = allow, giverPos = pos, bill = bill });
        }

        public static void AppendSyncTickDataSetWorker(Thing worker, IntVec3 pos, Bill_Production bill)
        {
            singleton.thingFilterSetWorkerCalls.Add(new ThingFilterSetWorkerCall() { worker = worker, giverPos = pos, bill = bill });
        }
        
        internal static void AppendSyncTickDataPawnDrafted(Pawn pawn, bool value)
        {
            singleton.pawnDrafts.Add(new PawnDrafted() { pawn = pawn, value = value });
        }

        internal static void AppendSyncTickDataFinalizeDesignationSucceeded(Designator instance)
        {
            throw new NotImplementedException();
        }

        internal static void AppendSyncTickDataDeleteZone(Zone instance)
        {
            CooperateRimming.Log(instance.ToString());
            singleton.deleteZones.Add(new SerializableZoneData() { zoneID = instance.ID });
        }

        internal static void AppendSyncTickDataIndexedBillConfigRestrictionOptionCall(Pawn p, int index, Bill_Production bill, IntVec3 billGiverPos)
        {
            singleton.indexedBillConfigRestrictionOptionCalls.Add(new IndexedBillConfigRestrictionOptionCall() { bill = bill, giverPos = billGiverPos, pawn = p, restrictOptionIndex = index });
        }

        internal static void AppendSyncTickDataDeltaFilter(ThingDef thing, Thing singleSelectedThing, Zone selectedZone, bool value)
        {
            singleton.thingFiltersData.Add(new ThingFilterDelta() { context = singleSelectedThing, zone = selectedZone == null ? null : new int[] { selectedZone.ID }, value = value, thingDefName = thing.defName });
        }

        internal static void AppendSyncTickDataDeltaFilterSpecial(SpecialThingFilterDef thingDef, Thing singleSelectedThing, Zone selectedZone, bool value)
        {
            singleton.thingFiltersDataSpecial.Add(new ThingFilterDeltaSpecial() { context = singleSelectedThing, zone = selectedZone == null ? null : new int[] { selectedZone.ID }, value = value, specialThingDefName = thingDef.defName });
        }
    }
}