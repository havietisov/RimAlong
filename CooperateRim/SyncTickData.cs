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
using CooperateRim.Utilities;

namespace CooperateRim
{
    public class WorkTypeDef_surrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            WorkTypeDef wd = (WorkTypeDef)obj;
            info.AddValue("defname", wd.defName);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            string defname = info.GetString("defname");
            return DefDatabase<WorkTypeDef>.AllDefs.First(u => u.defName == defname);
        }
    }

    [Serializable]
    public class SyncTickData : ISerializable
    {
        byte[] serializationServiceData = new byte[0];
        public int[] randomToVerify;
        public List<string> colonistJobsToVerify;
        
        public void DebugLog()
        {
            //NetDemo.log(nameof(toggleCommandIndexedCalls) + "::" + toggleCommandIndexedCalls.Count);
            //RimLog.Message();
        }
        
        public static int cliendID = -1;

        public static void SetClientID(int id)
        {
            RimLog.Message("RECEIVED CLIENT ID = " + id);
            cliendID = id;
        }

        public static bool IsDeserializing;
        public static bool AvoidLoop;

        static SyncTickData singleton = new SyncTickData();
        
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
                    
                    //RimLog.Message("Written : " + s);
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
                        singleton.randomToVerify = new int[] { Verse.Rand.Int };
                        RimLog.Message("Rand control context : " + RandContextCounter.currentContextName);
                        var colonists = Find.ColonistBar.GetColonistsInOrder();
                        singleton.colonistJobsToVerify = colonists == null ? new List<string> { } : colonists.ConvertAll<string>(u=> u.CurJobDef == null ? u.ThingID + "::" + "<null>" : u.ThingID + "::" + u.CurJobDef.defName);
                        singleton.serializationServiceData = SerializationService.Flush();
                        BinaryFormatter ser = new BinaryFormatter() { TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesWhenNeeded };
                        SyncTickData buffered = singleton;
                        MemoryStream fs = new MemoryStream();
                        singleton = new SyncTickData();
                        //RimLog.Message("sending data for tick " + tickNum);
                        NetDemo.PushStateToDirectory(cliendID, tickNum, buffered, 0);
                        fs.Close();
                        return true;
                    }
                    return false;
#endif
                }
                catch (Exception ee)
                {
                    RimLog.Message(ee.ToString());
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

            RimLog.Message("could not locate designation def : " + name);
            return null;
        }

        static void GetVal<T2>(ref List<T2> tVar, SerializationInfo info, string name)
        {
            tVar = (List<T2>)(info.GetValue(name, typeof(List<T2>)));
        }

        static void GetVal<T2>(ref T2[] tVar, SerializationInfo info, string name)
        {
            tVar = (T2[])(info.GetValue(name, typeof(T2[])));
        }

        public SyncTickData(SerializationInfo info, StreamingContext ctx)
        {
            GetVal(ref serializationServiceData, info, nameof(serializationServiceData));
            GetVal(ref randomToVerify, info, nameof(randomToVerify));
            GetVal(ref colonistJobsToVerify, info, nameof(colonistJobsToVerify));
        }

        public void AcceptResult()
        {
            int lockD = 0;

            try
            {
                Zone oldZone = Find.Selector.SelectedZone;
                
                lockD = 21;


                if (serializationServiceData.Length > 0)
                {
                    
                    foreach (var sd in SerializationService.DeserializeFrom(serializationServiceData))
                    {
                        if (sd.methodContext > -1)
                        {
                            AvoidLoop = true;
                            try
                            {
                                ParrotWrapper.IndexedCall(sd.methodContext, sd.dataObjects.ToArray());
                            }
                            catch (Exception ee)
                            {
                                RimLog.Message("Indexed call exception for " + sd.methodContext + "\r\n" + ee.ToString());

                                int i = 0;
                                foreach (object o in sd.dataObjects)
                                {
                                    RimLog.Message((i++) + " :." + o + ".:");
                                }
                            }
                            AvoidLoop = false;
                        }
                    }
                }

                lockD = 22;
                Find.CurrentMap.zoneManager.AllZones.ForEach(u => { u.Cells.GetEnumerator(); });

                Zone zz = Find.Selector.SelectedZone;

                if (zz != null)
                {
                    zz.Cells.GetEnumerator();//failsafe for release mode
                    if (oldZone != zz)
                    {
                        Find.Selector.SelectedZone = oldZone;
                    }
                }
            }
            catch (Exception ee)
            {
                RimLog.Message("sync tick data exception at stage " + lockD + "\r\n" + ee.ToString());
            }
            AvoidLoop = false;
        }
        
        
        public static void Apply(int tickNum)
        {
            //RimLog.Message("Applied frame " + tickNum);
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
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //serialization data
            {
                info.AddValue(nameof(serializationServiceData), serializationServiceData);
            }

            //control random numbers
            {
                info.AddValue(nameof(randomToVerify), randomToVerify);
            }

            //control pawn jobs
            {
                info.AddValue(nameof(colonistJobsToVerify), colonistJobsToVerify);
            }
        }
        
        static IEnumerable<T2> ConvertAll<T1, T2>(IEnumerable<T1> @this, Func<T1, T2> converter)
        {
            foreach (T1 v in @this)
            {
                yield return converter(v);
            }
        }
        
        internal static void AppendSyncTickDataFinalizeDesignationSucceeded(Designator instance)
        {
            throw new NotImplementedException();
        }
    }
}