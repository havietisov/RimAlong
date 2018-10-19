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
    public static int dispatchedActionCounter;
    static TcpClient tc;
    static NetworkStream ns;
    public static LinkedList<NetworkStream> allClients = new LinkedList<NetworkStream>();
    static int streamLocker;

    static void Log(string s)
    {
        log(s);
    }
    
    public static void WaitForConnection(string host)
    {
        tc = new TcpClient();
        tc.Client.NoDelay = true;
        tc.Connect(host, 12345);
        ns = tc.GetStream();

        
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

        Thread t = new Thread( ()=> 
        {
            for (;  ; )
            {
                PirateRPC.PirateRPC.ReceiveInvocation(ns);
            }
        } );

        t.Start();
    }

    public static bool HasAllDataForFrame(int frameID)
    {
        return ns.DataAvailable;// ns.DataAvailable;
    }

    public static void Receive()
    {
       // PirateRPC.PirateRPC.ReceiveInvocation(ns);
    }

    public static void setupCallbacks()
    {
        LocalDB.log = log;
    }
    
    public static void SendStateRequest(int frameID, int sourceID)
    {
        int fid = frameID;
        PirateRPC.PirateRPC.SendInvocation(ns, u => { LocalDB.NotifyClientNeedData(fid, sourceID, SyncTickData.clientCount, u); });
    }
    
    public static void PushStateToDirectory(int sourceID, int tickID, SyncTickData data, int channelID)
    {
        PirateRPC.PirateRPC.SendInvocation(ns, u => 
        {
            LocalDB.PushData(tickID, sourceID, data);
            LocalDB.TryDistributeData(tickID);
        });
    }
}
