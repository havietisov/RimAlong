using System.Runtime.Serialization;
using Verse;

namespace CooperateRim
{
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
            IntVec3 v = new IntVec3();
            v.x = info.GetInt32("xi");
            v.y = info.GetInt32("yi");
            v.z = info.GetInt32("zi");
            return v;
        }
    }
}
