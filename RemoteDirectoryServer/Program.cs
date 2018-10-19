using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using PirateRPC;
using System.Threading;
//NEVER run server with debugger attached!
namespace RemoteDirectoryServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int workers;
            int completions;
            ThreadPool.GetMinThreads(out workers, out completions);
            bool res = ThreadPool.SetMinThreads(4, 4);
            TcpListener lst = new TcpListener(IPAddress.Any, 12345);
            lst.Server.NoDelay = true;
            lst.Start();
            AsyncCallback acceptor = null;
            Console.WriteLine("RimAlong server version 0.0.0.1, codename \"shabby\", ready to rumble!");
            CooperateRim.SyncTickData.cliendID = 0;
            
            for (int i = 0; i < CooperateRim.TickManagerPatch.syncTickRoundOffset + 1; i++)
            {
                for (int l = 0; l < CooperateRim.SyncTickData.clientCount; l++)
                {
                    LocalDB.PushData(CooperateRim.TickManagerPatch.syncRoundLength * i, l, new CooperateRim.SyncTickData());
                }
            }

            lst.BeginAcceptTcpClient(acceptor = u =>
            {
                Console.WriteLine("new client");
                TcpClient tcc = (u.AsyncState as TcpListener).EndAcceptTcpClient(u);
                (u.AsyncState as TcpListener).BeginAcceptTcpClient(acceptor, u.AsyncState);
                NetworkStream ns = tcc.GetStream();
                NetDemo.allClients.AddLast(ns);

                for (; tcc.Connected ; )
                {
                    Thread.Sleep(50);
                    if (ns.DataAvailable)
                    {
                        PirateRPC.PirateRPC.ReceiveInvocation(ns);
                    }
                };

                Console.WriteLine("client lost");
            }, lst);
            
            for (; ; )
            {
            }
        }
    }
}
