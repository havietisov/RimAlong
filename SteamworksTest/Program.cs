using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
//watch this : https://github.com/rlabrecque/Steamworks.NET-Test/blob/master/Assets/Scripts/SteamNetworkingTest.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using CooperateRim;

namespace SteamworksTest
{
    static class Program
    {
        public static void Main()
        {
            NetDemo.setupCallbacks();
            Console.WriteLine("running remote directory host");

            for (; ; )
            {
                Thread.Sleep(10);
                
            }
        }
    }
}

