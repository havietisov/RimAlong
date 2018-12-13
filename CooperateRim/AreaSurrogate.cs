using Verse;
using System.Runtime.Serialization;
using System.Linq;

namespace CooperateRim
{
    public class AreaSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Area a = (Area)obj;
            info.AddValue("area_id", a.ID);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            int id = info.GetInt32("area_id");
            return Find.CurrentMap.areaManager.AllAreas.First(u => u.ID == id);
        }
    }
}
