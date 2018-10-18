using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using CooperateRim;
using System.Net.Sockets;

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
        if (!data.ContainsKey(tickID) || data[tickID].Any(u => u == null))
        {
            return false;
        }
            
        return true;
    }

    public static Action<Stream> GetCallback(SyncTickData[] ds)
    {
        return u => { TickManagerPatch.SetCachedData(ds); };
    }

    public static void TryDistributeData(int tickID)
    {
        List<SyncTickData> sdl = new List<SyncTickData>();

        if (HasFullData(tickID, SyncTickData.clientCount)/* && playerStateTable[clientID] < tickID*/)
        {
            sdl = new List<SyncTickData>(data[tickID]);

            foreach (var __ns in NetDemo.allClients)
            {
                foreach (var a in sdl)
                {
                    a.DebugLog();
                }

                PirateRPC.PirateRPC.SendInvocation(__ns, GetCallback(sdl.ToArray()));

            }
        }
    }
}

