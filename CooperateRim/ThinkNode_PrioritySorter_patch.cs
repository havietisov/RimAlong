using Harmony;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace CooperateRim
{
    public class streamholder
    {
        static Dictionary<string, System.IO.StreamWriter> _sw = new Dictionary<string, System.IO.StreamWriter>();

        public static void WriteLine(string s, string filename)
        {
            if (!_sw.ContainsKey(filename))
            {
                //_sw.Add(filename, System.IO.File.AppendText(@"D:\CoopReplays\" + (filename) + SyncTickData.cliendID + ".txt"));
            }

            //_sw[filename].WriteLine(s);
            //_sw[filename].Flush();
        }
    }
    
}
