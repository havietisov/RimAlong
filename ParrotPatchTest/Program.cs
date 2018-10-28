using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using CooperateRim;
using RimWorld;

namespace ParrotPatchTest
{
    class Program
    {
        delegate void testerPatch(tester __instance, int ___internal_value);

        static void DoSomething()
        {
            static_flag = !static_flag;
        }

        static bool static_flag;

        static void Main(string[] args)
        {
            SerializationService.Initialize();
            ParrotWrapper.Initialize();
            
            MemberTracker<bool>.TrackPublicField<Func<bool>>(() => static_flag, u => static_flag = u);
            static_flag = true;
            //MemberTracker<bool>.ApplyChange(true, 0);

            ParrotWrapper.ParrotPatchExpressiontarget<Action<bool, int>>((newVal, index)=> MemberTracker<bool>.ApplyChange(newVal, index));
            MemberTracker<bool>.ApplyChange(false, 0);

            SyncTickData.AvoidLoop = true;
            byte[] b;
            var _dat = SerializationService.DeserializeFrom(b = SerializationService.Flush());

            foreach (var dat in _dat)
            {
                if (dat.methodContext > -1)
                {
                    ParrotWrapper.IndexedCall(dat.methodContext, dat.dataObjects.ToArray());
                }
            }

            
            //CooperateRim.ParrotWrapper.ParrotPatchExpressiontarget<Action<BillStack, Bill>>((__instance, bill) => __instance.AddBill(bill));
            /*
            CooperateRim.ParrotWrapper.ParrotPatchExpressiontarget<Action<tester, string>>((__instance, name_something) => __instance.DoSomething(name_something));
            tester t = new tester();
            t.DoSomething("1");

            SyncTickData.AvoidLoop = true;
            byte[] b;
            var _dat = SerializationService.DeserializeFrom(b = SerializationService.Flush());

            foreach (var dat in _dat)
            {
                ParrotWrapper.IndexedCall(dat.methodContext, dat.dataObjects.ToArray());
            }

            t.DoSomething("2");*/
        }
    }
}
