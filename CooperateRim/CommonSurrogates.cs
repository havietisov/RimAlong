using CooperateRim.Utilities;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Verse;

namespace CooperateRim
{
    public class TesterSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            tester t = (tester)obj;
            info.AddValue(nameof(tester.internal_value), t.internal_value);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            tester t = (tester)obj;
            t.internal_value = info.GetInt32(nameof(tester.internal_value));
            return t;
        }
    }
    
    public class IntVec3Surrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            IntVec3 v = (IntVec3)obj;
            info.AddValue("xi", v.x);
            info.AddValue("yi", v.y);
            info.AddValue("zi", v.z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            IntVec3 v = (IntVec3)obj;
            v.x = info.GetInt32("xi");
            v.x = info.GetInt32("yi");
            v.x = info.GetInt32("zi");
            return v;
        }
    }
    
    public class BillProductionSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Bill_Production bp = (Bill_Production)obj;
            info.AddValue("recipedef", bp.recipe.defName);
            info.AddValue("loadID", bp.GetUniqueLoadID());
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            /*
            ISurrogateSelector _selector;
            ISerializationSurrogate billmaker = selector.GetSurrogate(typeof(Bill), context, out _selector);
            Bill b = (Bill)billmaker.SetObjectData(obj, info, context, _selector);
            return (Bill_Production)b;*/
            string recipeDefName = info.GetString("recipedef");
            string loadID = info.GetString("loadID");

            foreach (var b in BillUtility.GlobalBills())
            {
                if (b.GetUniqueLoadID() == loadID)
                {
                    return b;
                }
            }

            foreach (var rec in DefDatabase<RecipeDef>.AllDefsListForReading)
            {
                if (rec.defName == recipeDefName)
                {
                    RimLog.Message("bill production restored via recipedef!");
                    Bill b = BillUtility.MakeNewBill(rec);
                    return b;
                }
            }
            
            RimLog.Message("could not make bill_production!");
            return null;
        }
    }

    public class BillStackSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            BillStack bs = (BillStack)obj;
            Thing t = bs.billGiver as Thing;
            IntVec3 thingPos = t.PositionHeld;
            info.AddValue("billgiverID", t.ThingID);
            info.AddValue("thingPos", thingPos);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            string billGiverID = info.GetString("billgiverID");
            IntVec3 pos = (IntVec3)info.GetValue("thingPos", typeof(IntVec3));
            RimLog.Message("bill stack surrogate obj " + (obj == null ? "null" : "not null"));
            RimLog.Message("looking for issuer at " + pos);
            List<Thing>[] things = (List<Thing>[])Find.CurrentMap.thingGrid.GetType().GetField("thingGrid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(Find.CurrentMap.thingGrid);

            foreach(var aa in things)
            {
                foreach (var issuer in aa)
                {
                    if (issuer.Position == pos)
                    {
                        RimLog.Message(issuer.ThingID + " :+: " + billGiverID);
                    }
                    if (issuer.ThingID == billGiverID)
                    {
                        RimLog.Message(issuer.ThingID + " :: " + billGiverID);
                        RimLog.Message("returning billstack ? " + ((issuer as IBillGiver).BillStack == null ? "null" : "not null"));
                        return (issuer as IBillGiver).BillStack;
                    }
                }
            }

            RimLog.Message("could not locate bill giver");
            return null;
        }
    }

    public class BillSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Bill b = (Bill)obj;
            BillStack st = b.billStack;
            info.AddValue("bill_stack", st);
            info.AddValue("recipedef", b.recipe.defName);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            string recipeDefName = info.GetString("recipedef");
            BillStack st = (BillStack)info.GetValue("bill_stack", typeof(BillStack));
            RimLog.Message("bill lacks pawn restriction in surrogate!");
            RimLog.Message("billstack is null ? " + (st == null ? "yes" : "no"));
            RimLog.Message("bill giver as thing : " + (st.billGiver as Thing));
            RimLog.Message("bill giver def as thing : " + (st.billGiver as Thing).def);

            foreach (var rec in (st.billGiver as Thing).def.recipes)
            {
                if (rec.defName == recipeDefName)
                {
                    return BillUtility.MakeNewBill(rec);
                }
            }

            RimLog.Message("could not make bill!");
            return null;
        }
    }
}
