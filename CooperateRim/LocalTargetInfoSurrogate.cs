using CooperateRim.Utilities;
using System.Runtime.Serialization;
using Verse;

namespace CooperateRim
{
    public class LocalTargetInfoSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            LocalTargetInfo lti = (LocalTargetInfo)(obj);
            info.AddValue("lti.hasthing", lti.HasThing);

            if (lti.HasThing)
            {
                info.AddValue("lti.thing", (Thing)lti.Thing);
            }
            else
            {
                info.AddValue("lti.cell", lti.Cell);
            }
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            LocalTargetInfo lti;
            if (info.GetBoolean("lti.hasthing"))
            {
                object o = info.GetValue("lti.thing", typeof(Thing));
                
                lti = new LocalTargetInfo((Thing)o);
            }
            else
            {
                lti = new LocalTargetInfo((IntVec3)info.GetValue("lti.cell", typeof(IntVec3)));
            }
            
            return lti;
        }
    }
}
