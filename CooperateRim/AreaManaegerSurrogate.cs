using Verse;
using System.Runtime.Serialization;

namespace CooperateRim
{
    public class AreaManaegerSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            return Find.CurrentMap.areaManager;
        }
    }
}
