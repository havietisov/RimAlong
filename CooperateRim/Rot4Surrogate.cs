using Verse;
using System.Runtime.Serialization;
using CooperateRim.Utilities;

namespace CooperateRim
{
    public class Rot4Surrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Rot4 r = (Rot4)obj;
            RimLog.Message("rit_int" + r.AsInt);
            info.AddValue("rot_int", r.AsInt);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Rot4 r = new Rot4();
            r.AsInt = ((int)info.GetValue("rot_int", typeof(int)));
            return r;
        }
    }
}
