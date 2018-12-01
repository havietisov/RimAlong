using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Verse;

namespace CooperateRim
{
    class Bill_ProductionWithUft_surrogate : BillSurrogate
    {
        public new void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Bill_ProductionWithUft bpwu = (Bill_ProductionWithUft)obj;
            base.GetObjectData(obj, info, context);
            info.AddValue("uft", bpwu.BoundUft);
        }

        public new object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Bill_ProductionWithUft _obj = (Bill_ProductionWithUft)base.SetObjectData(obj, info, context, selector);
            UnfinishedThing uft = (UnfinishedThing)info.GetValue("uft", typeof(UnfinishedThing));

            if (_obj != null)
            {
                _obj.SetBoundUft(uft, true);
            }

            return _obj;
        }
    }
}
