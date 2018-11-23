using RimWorld;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Verse;

namespace CooperateRim
{
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
            Utilities.RimLog.Message("bill stack surrogate obj " + (obj == null ? "null" : "not null"));
            Utilities.RimLog.Message("looking for issuer at " + pos);
            List<Thing>[] things = (List<Thing>[])Find.CurrentMap.thingGrid.GetType().GetField("thingGrid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(Find.CurrentMap.thingGrid);

            foreach(var aa in things)
            {
                foreach (var issuer in aa)
                {
                    if (issuer.Position == pos)
                    {
                        Utilities.RimLog.Message(issuer.ThingID + " :+: " + billGiverID);
                    }
                    if (issuer.ThingID == billGiverID)
                    {
                        Utilities.RimLog.Message(issuer.ThingID + " :: " + billGiverID);
                        Utilities.RimLog.Message("returning billstack ? " + ((issuer as IBillGiver).BillStack == null ? "null" : "not null"));
                        return (issuer as IBillGiver).BillStack;
                    }
                }
            }

            Utilities.RimLog.Message("could not locate bill giver");
            return null;
        }
    }
}
