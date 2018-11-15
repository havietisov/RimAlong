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
            CooperateRimming.Log("bill stack surrogate obj " + (obj == null ? "null" : "not null"));
            CooperateRimming.Log("looking for issuer at " + pos);
            List<Thing>[] things = (List<Thing>[])Find.CurrentMap.thingGrid.GetType().GetField("thingGrid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(Find.CurrentMap.thingGrid);

            foreach(var aa in things)
            {
                foreach (var issuer in aa)
                {
                    if (issuer.Position == pos)
                    {
                        CooperateRimming.Log(issuer.ThingID + " :+: " + billGiverID);
                    }
                    if (issuer.ThingID == billGiverID)
                    {
                        CooperateRimming.Log(issuer.ThingID + " :: " + billGiverID);
                        CooperateRimming.Log("returning billstack ? " + ((issuer as IBillGiver).BillStack == null ? "null" : "not null"));
                        return (issuer as IBillGiver).BillStack;
                    }
                }
            }

            CooperateRimming.Log("could not locate bill giver");
            return null;
        }
    }
}
