using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using CooperateRim;
using System.Net.Sockets;
using Verse;

public class LocalDB
{
    public static Action<string> log = Console.WriteLine;
    static Dictionary<int, SyncTickData[]> data = new Dictionary<int, SyncTickData[]>();
    public static List<KeyValuePair<int, SyncTickData>> clientLocalStorage = new List<KeyValuePair<int, SyncTickData>>();
    public static Action OnApply;
    public static Dictionary<int, int> playerStateTable = new Dictionary<int, int>();

    public static void AddPlayerState(int ind)
    {
        playerStateTable.Add(ind, -100);
    }

    public static void SetOnApply(Action a)
    {
        OnApply = a;
    }

    public static string GetStringFor(int tickID, int playerID)
    {
        return "player : " + playerID + ", tick : " + tickID;
    }
    
    public static void PushData(int tickID, int playerID, SyncTickData sd)
    {
        lock (data)
        {
            if (!data.ContainsKey(tickID))
            {
                data.Add(tickID, new SyncTickData[SyncTickData.clientCount]);
            }

            if (data[tickID][playerID] == null)
            {
                log("player state : " + GetStringFor(tickID, playerID));
                data[tickID][playerID] = sd;
            }
        }
    }

    static int minPlayerTick = 99999;

    static int GetSTICKV()
    {
        return TickManagerPatch.nextProcessionTick;
    }

    public static void NotifyClientNeedData(int tickID, int clientID, int clientCount, Stream stream)
    {
        log("client notification : " + GetStringFor(tickID, clientID));
        

        if (!playerStateTable.ContainsKey(clientID))
        {
            AddPlayerState(clientID);
        }

        foreach (var a in playerStateTable)
        {
            minPlayerTick = a.Value;
            break;
        }

        foreach (var a in playerStateTable)
        {
            minPlayerTick = Math.Min(minPlayerTick, a.Value);
        }
        
    }
        
        
    public static bool HasFullData(int tickID, int clientCount)
    {
        lock (data)
        {
            if (!data.ContainsKey(tickID) || data[tickID].Any(u => u == null))
            {
                return false;
            }

            return true;
        }
    }

    public static Action<Stream> GetCallback(SyncTickData[] ds)
    {
        DateTime dt = DateTime.UtcNow;
        return u => { TickManagerPatch.SetCachedData(ds); /*CooperateRimming.Log("Message delivery took " + (DateTime.UtcNow - dt).TotalMilliseconds.ToString());*/ };
    }

    enum desyncReason
    {
        Rng,
        Jobs,
        None
    }

    public static void TryDistributeData(int tickID)
    {
        List<SyncTickData> sdl = new List<SyncTickData>();

        DateTime dt = DateTime.Now;
        if (HasFullData(tickID, SyncTickData.clientCount)/* && playerStateTable[clientID] < tickID*/)
        {
            NetDemo.log("dictionary lookup took " + (DateTime.Now - dt).TotalMilliseconds);
            sdl = new List<SyncTickData>(data[tickID]);
            int? randomV = null;
            List<string> jobsToVerify = null;
            desyncReason desyncReason = desyncReason.None;
            
            foreach (var __ns in NetDemo.allClients)
            {
                foreach (var a in sdl)
                {
                    {
                        if (!randomV.HasValue)
                        {
                            randomV = a.randomToVerify[0];
                        }
                        else
                        {
                            NetDemo.log(randomV.Value + " == " + a.randomToVerify[0]);

                            if (randomV.Value != a.randomToVerify[0])
                            {
                                desyncReason = desyncReason.Rng;
                            }
                        }

                        if (jobsToVerify == null)
                        {
                            jobsToVerify = a.colonistJobsToVerify;
                        }
                        else
                        {
                            if (jobsToVerify.Count != a.colonistJobsToVerify.Count || !jobsToVerify.All(__data => a.colonistJobsToVerify.Any( uu => uu.SequenceEqual(__data))))
                            {
                                if (jobsToVerify.Count != a.colonistJobsToVerify.Count)
                                {
                                    NetDemo.log("JOBS COUNT IS DIFFERENT!" + jobsToVerify.Count + " != " + a.colonistJobsToVerify.Count);
                                }
                                else
                                {
                                    string ds = "";

                                    for (int i =0; i < jobsToVerify.Count; i++)
                                    {
                                        ds += ("JOBSDUMP : " + jobsToVerify[i] + " != " + a.colonistJobsToVerify[i]) + "\r\n";
                                    }
                                    
                                    NetDemo.log(ds);
                                }

                                desyncReason = desyncReason.Jobs;
                                //throw new Exception("Session jobs desynced!");
                            }
                        }
                    }
                    a.DebugLog();
                }

                DateTime _dt = DateTime.Now;
                switch (desyncReason)
                {
                    case desyncReason.None:
                        PirateRPC.PirateRPC.SendInvocation(__ns, GetCallback(sdl.ToArray()));
                        break;
                    case desyncReason.Jobs:
                        PirateRPC.PirateRPC.SendInvocation(__ns, uuu => { Messages.Message("Session desynchronized! Reason : different colonist jobs", RimWorld.MessageTypeDefOf.ThreatBig, true); });
                        break;
                    case desyncReason.Rng:
                        NetDemo.log("Session desynchronized! Reason : different control random numbers");
                        PirateRPC.PirateRPC.SendInvocation(__ns, uuu => { Messages.Message("Session desynchronized! Reason : different control random numbers", RimWorld.MessageTypeDefOf.ThreatBig, true);  });
                        break;
                }
                NetDemo.log("invocation took " + (DateTime.Now - dt).TotalMilliseconds);
            }

            //data.Remove(tickID);
        }

    }
}

