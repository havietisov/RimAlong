using System.Runtime.Serialization;
using UnityEngine;

namespace CooperateRim
{
    public class Vector3Surrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector3 v = (Vector3)obj;
            info.AddValue("xf", v.x);
            info.AddValue("yf", v.y);
            info.AddValue("zf", v.z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector3 v = new Vector3();
            v.x = info.GetSingle("xf");
            v.y = info.GetSingle("yf");
            v.z = info.GetSingle("zf");
            return v;
        }
    }
}
