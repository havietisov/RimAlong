using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using CooperateRim;
using RimWorld;
using System.Runtime.Serialization;

namespace ParrotPatchTest
{
    public class g2
    {
        public string ccc = "ccc";
    }

    public class g1
    {
        public g2 fld = new g2();

        public void Do() { }
    }


    class Program
    {
        delegate void testerPatch(tester __instance, int ___internal_value);

        static void DoSomething()
        {
            static_flag = !static_flag;
        }

        static bool static_flag;

        class g2_surrogate : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                info.AddValue("ccc", (obj as g2).ccc);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                var g2 = new g2();
                g2.ccc = info.GetString("ccc");
                return g2;
            }
        }

        class g1_surrogate : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                info.AddValue("g1", (obj as g1).fld);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                var @g1 = new g1();
                object o = info.GetValue("g1", typeof(object));
                g1.fld = (g2)info.GetValue("g1", typeof(g2));
                return @g1;
            }
        }

        static void Main(string[] args)
        {
            SerializationService.Initialize();
            ParrotWrapper.Initialize();
            SerializationService.AppendSurrogate(typeof(g1), new g1_surrogate());
            SerializationService.AppendSurrogate(typeof(g2), new g2_surrogate());
            ParrotWrapper.ParrotPatchExpressiontarget<Action<g1>>((__instance) => __instance.Do());
            new g1().Do();
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
            /*
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
            }*/


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
