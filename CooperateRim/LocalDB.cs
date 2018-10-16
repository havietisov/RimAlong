using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace CooperateRim
{
    public class LocalDB
    {
        public static Action<string> log = Console.WriteLine;
        static Dictionary<string, SyncTickData> data = new Dictionary<string, SyncTickData>();
        public static Action<Action> dispatcher = u => { };
        
        public static string GetStringFor(int tickID, int playerID)
        {
            return "player : " + playerID + ", tick : " + tickID;
        }

        public static void PushData(int tickID, int playerID, SyncTickData sd)
        {
            if (!data.ContainsKey(GetStringFor(tickID, playerID)))
            {
                log("new : " + GetStringFor(tickID, playerID));
                data.Add(GetStringFor(tickID, playerID), sd);
            }
        }

        static void Dispatch(Action a)
        {
            dispatcher(a);
        }

        public static void NotifyClientNeedData(int tickID, int clientID, int clientCount, Stream stream)
        {
            log("client notification : " + GetStringFor(tickID, clientID));

            if (HasFullData(clientID, clientCount))
            {
                List<SyncTickData> sdl = new List<SyncTickData>();

                for (int i = 0; i < clientCount; i++)
                {
                    sdl.Add(data[GetStringFor(tickID, i)]);
                }

                PirateRPC.PirateRPC.SendInvocation(stream, s =>
                {
                    CooperateRimming.Log("invocation callback received ");
                    Dispatch(() => sdl.ForEach(kk => kk.AcceptResult()));
                });
            }
        }
        
        
        public static bool HasFullData(int tickID, int clientCount)
        {
            for (int i = 0; i < clientCount; i++)
            {
                if (!data.ContainsKey(GetStringFor(tickID, i)))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
