using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using PirateRPC;
using System.Threading;

namespace RemoteDirectoryServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener lst = new TcpListener(IPAddress.Any, 12345);
            lst.Start();
            AsyncCallback acceptor = null;
            Console.WriteLine("RimAlong server version 0.0.0.1, codename \"shabby\", ready to rumble!");
            CooperateRim.SyncTickData.cliendID = 0;

            lst.BeginAcceptTcpClient(acceptor = u => 
            {
                Console.WriteLine("new client");
                TcpClient tcc = (u.AsyncState as TcpListener).EndAcceptTcpClient(u);
                (u.AsyncState as TcpListener).BeginAcceptTcpClient(acceptor, u.AsyncState);
                NetworkStream ns = tcc.GetStream();

                for (; tcc.Connected ; )
                {
                    PirateRPC.PirateRPC.ReceiveInvocation(ns);
                };

                Console.WriteLine("client lost");
            }, lst);
            
            for (; ; )
            {
                Thread.Sleep(100);
            }
        }
    }
}
