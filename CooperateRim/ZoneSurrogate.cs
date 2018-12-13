using System.Runtime.Serialization;
using Verse;
using System.Linq;

namespace CooperateRim
{
    public class ZoneSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Zone z = (Zone)obj;
            info.AddValue("zone_id", z.ID);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            int id = info.GetInt32("zone_id");
            return Find.CurrentMap.zoneManager.AllZones.First(u => u.ID == id);
        }
    }
}
