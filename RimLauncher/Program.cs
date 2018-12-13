using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RimLauncher
{
    class Program
    {
        static Process p;

        [DllImport("user32.dll")]
        static extern int SetWindowText(IntPtr hWnd, string text);
        
        static void Main(string[] args)
        {
            p = Process.Start(new ProcessStartInfo() { FileName = @"G:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64.exe", Arguments = "network_launch" });
            ChildProcessTracker.AddProcess(p);
            System.Threading.Thread.Sleep(5000);
            SetWindowText(p.MainWindowHandle, "Rimworld launched by 6opoDuJIo launcher to handle OBS mess!");
            p.WaitForExit();
            
        }
    }
}
