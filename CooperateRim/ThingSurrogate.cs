using CooperateRim.Utilities;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Verse;

namespace CooperateRim
{
    public class ThingSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Thing p = (Thing)obj;
            info.AddValue("pawn_thingid", p.thingIDNumber);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            int idNumber = info.GetInt32("pawn_thingid");
            Thing t = ThingRegistry.tryGetThing(idNumber);

            if (t == null)
            {
                RimLog.Message("Could not locate a pawn with thingid " + idNumber);
            }
            return t;
        }
    }
}
