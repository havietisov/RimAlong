using System.Runtime.Serialization;
using Verse;
using System.Linq;

namespace CooperateRim
{
    public class TerrainDefSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            info.AddValue("defname", ((TerrainDef)obj).defName);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            string s = info.GetString("defname");
            return DefDatabase<TerrainDef>.AllDefs.First(u=> u.defName == s);
        }
    }
}
