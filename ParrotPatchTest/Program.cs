using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using CooperateRim;

namespace ParrotPatchTest
{

    class Program
    {

        delegate void testerPatch(tester __instance, int ___internal_value);

        static void Main(string[] args)
        {
            CooperateRim.ParrotWrapper.ParrotPatchExpressiontarget<Action<tester, int, string>>((__instance, ___internal_value, name_something) => __instance.DoSomething(name_something));
            tester t = new tester();
            t.DoSomething("1");
            SyncTickData.AvoidLoop = true;
            t.DoSomething("2");
        }
    }
}
