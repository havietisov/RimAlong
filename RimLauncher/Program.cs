using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimLauncher
{
    class Program
    {
        static Process p;

        static void Main(string[] args)
        {
            p = Process.Start(new ProcessStartInfo() { FileName = @"D:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64.exe", Arguments = "network_launch" });
            ChildProcessTracker.AddProcess(p);
            p.WaitForExit();
            
        }
    }
}
