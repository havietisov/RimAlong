using System.Collections.Generic;
using System.Runtime.Serialization;
using Verse;

namespace CooperateRim
{
    public class IndexedPawnSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Thing p = (Thing)obj;
            info.AddValue("pawn_thingid", p.thingIDNumber);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            int idNumber = info.GetInt32("pawn_thingid");

            List<Thing>[] things = (List<Thing>[])Find.CurrentMap.thingGrid.GetType().GetField("thingGrid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(Find.CurrentMap.thingGrid);

            foreach (var tl in things)
            {
                foreach (var thing in tl)
                {
                    if (thing.thingIDNumber == idNumber)
                    {
                        return thing;
                    }
                }
            }

            CooperateRimming.Log("Could not locate a pawn with thingid " + idNumber);
            return null;
        }
    }
}
