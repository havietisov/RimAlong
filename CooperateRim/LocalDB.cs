using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using CooperateRim;


public class LocalDB
{
    public static Action<string> log = Console.WriteLine;
    static Dictionary<int, SyncTickData[]> data = new Dictionary<int, SyncTickData[]>();
    public static List<KeyValuePair<int, SyncTickData>> clientLocalStorage = new List<KeyValuePair<int, SyncTickData>>();
    public static Action OnApply;
    public static Dictionary<int, int> playerStateTable = new Dictionary<int, int>();

    public static void AddPlayerState(int ind)
    {
        playerStateTable.Add(ind, 0);
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


    public static void NotifyClientNeedData(int tickID, int clientID, int clientCount, Stream stream)
    {
        log("client notification : " + GetStringFor(tickID, clientID));
        List<SyncTickData> sdl = new List<SyncTickData>();

        if (!playerStateTable.ContainsKey(clientID))
        {
            AddPlayerState(clientID);
        }

        if (HasFullData(tickID, clientCount) && playerStateTable[clientID] < tickID)
        {
            sdl = new List<SyncTickData>(data[tickID]);

            foreach (var a in sdl)
            {
                a.DebugLog();
                a.AcceptResult();
            }

            PirateRPC.PirateRPC.SendInvocation(stream, s =>
            {
                CooperateRimming.Log("invocation callback received for tick " + tickID + ", local tick " + Verse.Find.TickManager.TicksGame );
                
                {
                    foreach (var a in sdl)
                    {
                        a.DebugLog();
                        a.AcceptResult();
                    }
                }
            });

            playerStateTable[clientID] = tickID;
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
}

