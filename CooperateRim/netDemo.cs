using CooperateRim;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

public class NetDemo
{
    public static Action<string> log = u => { new Action(Console.WriteLine).BeginInvoke(k => { Console.WriteLine(u); }, u); };
    public static List<Action> dispatchedActions = new List<Action>();
    public static int dispatchedActionCounter;
    static TcpClient tc;
    static NetworkStream ns;
    static int streamLocker;

    static void Log(string s)
    {
        log(s);
    }
    
    public static void WaitForConnection()
    {
        tc = new TcpClient("127.0.0.1", 12345);
        ns = tc.GetStream();


        Thread t = new Thread(() => 
        {
            for (; Interlocked.CompareExchange(ref streamLocker, 1, 0) == 0;) { }
            
            try
            {
                PirateRPC.PirateRPC.SendInvocation(ns, u =>
                {
                    int cid = SyncTickData.cliendID;
                    Interlocked.Increment(ref SyncTickData.cliendID);
                    Log("sending client id " + cid);

                    PirateRPC.PirateRPC.SendInvocation(u, k => 
                    {
                        SyncTickData.SetClientID(cid);
                    });
                });
            }
            catch (Exception ee)
            {
                log(ee.ToString());
            }

            Interlocked.Decrement(ref streamLocker);

            for (int i =0; ; i++ )
            {
                if (ns.DataAvailable)
                {
                    PirateRPC.PirateRPC.ReceiveInvocation(ns);
                }
                else
                {
                    Thread.Sleep(50);
                }

                /*
                Thread.Sleep(100);
                Log("ack " + i);
                for (; streamLocker > 0;) { }
                Interlocked.Increment(ref streamLocker);

                try
                {
                    PirateRPC.PirateRPC.ReceiveInvocation(ns);
                }
                catch (Exception ee)
                {
                    log(ee.ToString());
                }

                Interlocked.Decrement(ref streamLocker);*/
            };
        });

        t.Start();
    }

    public static bool HasAllDataForFrame(int frameID)
    {
        for (; dispatchedActionCounter > 0;) { Thread.SpinWait(1000); }
        Interlocked.Increment(ref dispatchedActionCounter);
        bool res = dispatchedActions.Count > 0;
        Interlocked.Decrement(ref dispatchedActionCounter);
        return res;
    }
    
    public static void setupCallbacks()
    {
        LocalDB.log = log;
        LocalDB.dispatcher = u => 
        {
            for (; dispatchedActionCounter > 0;) { Thread.SpinWait(1000); }
            Interlocked.Increment(ref dispatchedActionCounter);
            dispatchedActions.Add(u);
            Interlocked.Decrement(ref dispatchedActionCounter);
        };
    }
    
    public static void SendStateRequest(int frameID, int sourceID)
    {
        PirateRPC.PirateRPC.SendInvocation(ns, u => { LocalDB.NotifyClientNeedData(frameID, sourceID, SyncTickData.clientCount, u); });
    }
    
    public static void PushStateToDirectory(int sourceID, int tickID, SyncTickData data, int channelID)
    {
        PirateRPC.PirateRPC.SendInvocation(ns, u => 
        {
            LocalDB.PushData(tickID, sourceID, data);
        });
    }
}
